using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Common.Models;
using static Dapper.SqlMapper;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/20/2023 10:30:59 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupSetupViewModel : BaseDataGridPanel<AudioGroupViewModel>
    {
        

        #region - Ctors -
        public AudioGroupSetupViewModel(IEventAggregator eventAggregator
                                        , ILogService log
                                        , AudioDbService dbService
                                        , AxisApiService apiService
                                        , AudioGroupProvider audioGroupProvider
                                        , AudioViewModelProvider audioProvider
                                        , AudioSensorViewModelProvider audioSensorProvider
            )
                                        : base(eventAggregator)
        {
            _dbService = dbService;
            _log = log;
            _apiService = apiService;
            _audioGroupProvider = audioGroupProvider;
            _audioViewModelProvider = audioProvider;
            _audioSensorViewModelProvider = audioSensorProvider;
        }
        #endregion
        #region - Implementation of Interface -
        /// <summary>
        /// 삭제 시, GroupProvider쪽 삭제는 당연하고,
        /// AudioViewModelProvider를 통한 AudioViewModel(AudioModel연계) 하여 속한 Group을 제거한다.
        /// AudioSensorViewModelProvider를 통한 AudioSensorViewModel(AudioSensorModel연계)하여 
        /// 해당 그룹에 매칭된 AudioSensorViewModel을 삭제한다(AudioSensorModel연계 삭제됨).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach(var groupViewModel in ViewModelProvider.ToList())
                {
                    if (groupViewModel.IsSelected)
                    {
                        ViewModelProvider.Remove(groupViewModel);

                        foreach (var audioModel in _audioViewModelProvider
                            .Where(entity => entity.Groups
                            .Any(inner => inner.GroupNumber == groupViewModel.GroupNumber))
                            .ToList())
                        {
                            audioModel.Groups.Remove(groupViewModel.Model as AudioGroupModel);
                        }
                        
                        foreach (var item in _audioSensorViewModelProvider
                            .Where(entity => entity.Group
                            .Equals(groupViewModel.Model))
                            .ToList())
                        {
                            _audioSensorViewModelProvider.Remove(item);
                        }

                        groupViewModel.Model.AudioModels.Clear();
                        groupViewModel.Model.SensorModels.Clear();
                        groupViewModel.Model = null;
                    }
                }
                
                await SelectAll(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickDeleteButton)} in {ClassName}): {ex.Message}", true);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// AudioGroupModel을 ViewModelProvider를 통해서 추가하고, 
        /// 연동된 Provider로 함께 추가하게 된다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = 0;
                if (ViewModelProvider.Count > 0)
                    id = ViewModelProvider.Max(entity => entity.Id);

                int group = 0;
                if (ViewModelProvider.Count > 0)
                    group = ViewModelProvider.Max(entity => entity.GroupNumber);

                var audioGroup = new AudioGroupModel(id + 1, group + 1, "None");

                var viewModel = new AudioGroupViewModel(audioGroup);
                await viewModel.ActivateAsync();
                ViewModelProvider.Add(viewModel);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickInsertButton)} in {ClassName}): {ex.Message}");
            }
        }

        /// <summary>
        /// 데이터 Reload 시, 모든 인스턴스에는 각각 연동된 인스턴스로 인해
        /// Dependency가 발생한다. 따라서 모두 다 갱신을 하여 전체 Sync를 맞춘다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickReloadButton(object sender, RoutedEventArgs e)
        {
            ButtonEnableControl(false, true, false);
            try
            {
                ///그룹 Reload 시, 기존의 저장하지 않고 작업한 작업물을 모두 지워야함.
                ///1. GroupProvider를 Fetch한다.
                ///2. SensorProvider도 Fetch한다. 
                ///3. AudioProvider도 Fetch한다.


                await _dbService.FetchAudioGroupModel();
                await _dbService.FetchAudioModel();
                await _dbService.FetchAudioSensorModel();

                await _dbService.CreateAudioGroupModel();
                await DataInitialize(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickReloadButton)} in {ClassName}): {ex.Message}");
            }
            finally
            {
                ButtonAllEnable();
            }
        }

        /// <summary>
        /// 데이터 Save 시, 모든 Provider를 함께 저장하여 전체적인 Sync를 맞춘다.
        /// 또한, DataInitialize를 수행하여, 변경된 내용을 반영하여 새로 ViewModelProvider을
        /// 만들어 준다. 아울러 두 LookupTable도 새로 갱신해야한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            ButtonEnableControl(false, false, true);
            try
            {
                await _dbService.InsertAudioGroupModel();
                await _dbService.InsertAudioModel();
                await _dbService.InsertAudioSensorModel();
                await DataInitialize(_cancellationTokenSource.Token);
                _apiService.CreateLookupTable();
                _apiService.CreateVisualLookupTable();
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickSaveButton)} in {ClassName}): {ex.Message}");
            }
            finally
            {
                ButtonAllEnable();
            }

        }
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            _ = DataInitialize(_cancellationTokenSource.Token);
        }
        protected override async Task Uninitialize()
        {
            ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
            await base.Uninitialize();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task DataInitialize(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    IsVisible = false;
                    await Task.Delay(1000, cancellationToken);

                    if (cancellationToken.IsCancellationRequested) new TaskCanceledException("Task was cancelled!");
                    
                    //ViewModelProvider Setting
                    var provider = IoC.Get<AudioGroupViewModelProvider>();
                    await provider.Initialize();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (AudioGroupViewModel item in provider)
                        {
                            ViewModelProvider.Add(item);

                            //디버깅용
                            Debug.WriteLine($"[오디오그룹] " +
                                $"ID : {item.Id}, " +
                                $"GroupNumber : {item.GroupNumber}, " +
                                $"GroupName : {item.GroupName}");

                            if (item.AudioModels != null)
                            {
                                foreach (var audio in item.AudioModels)
                                {
                                    Debug.WriteLine($"[오디오그룹-오디오모델] " +
                                        $"ID : {audio.Id}, " +
                                        $"Groups : {audio.Groups?.Count()}, " +
                                        $"DeviceName : {audio.DeviceName}, " +
                                        $"Password : {audio.Password}, " +
                                        $"IpAddress : {audio.IpAddress}, " +
                                        $"Port : {audio.Port}, " +
                                        $"Mode : {audio.Mode}");
                                }
                            }

                            if (item.AudioSensorModels != null)
                            {
                                foreach (var sensor in item.AudioSensorModels)
                                {
                                    Debug.WriteLine($"[오디오그룹-오디오센서] " +
                                        $"ID : {sensor.Id}, " +
                                        $"Group : {sensor.Group.GroupName}, " +
                                        $"DeviceName : {sensor.DeviceName}, " +
                                        $"ControllerId : {sensor.ControllerId}, " +
                                        $"SensorId : {sensor.SensorId}, " +
                                        $"DeviceType : {sensor.DeviceType}");
                                }
                            }
                        }

                    }));
                    ViewModelProvider.CollectionChanged += ViewModelProvider_CollectionChanged;
                    NotifyOfPropertyChange(() => ViewModelProvider);
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)} in {ClassName}) : {ex.Message}");
                }

            });
        }

        private async void ViewModelProvider_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (AudioGroupViewModel newItem in e.NewItems)
                    {
                        _audioGroupProvider.Add(newItem.Model as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioGroupViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.ViewModel as AudioModels);
                        _audioGroupProvider.Remove(newItem.Model as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioGroupViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.ViewModel as AudioModels);
                        _audioGroupProvider.Remove(oldItem.Model as AudioGroupModel);
                    }
                    foreach (AudioGroupViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.ViewModel as AudioModels);
                        _audioGroupProvider.Add(newItem.Model as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    await DataInitialize();
                    break;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private AudioDbService _dbService;
        private ILogService _log;
        private AxisApiService _apiService;
        private AudioGroupProvider _audioGroupProvider;
        //private AudioProvider _audioProvider;
        private AudioViewModelProvider _audioViewModelProvider;
        //private AudioSensorProvider _audioSensorProvider;
        private AudioSensorViewModelProvider _audioSensorViewModelProvider;
        #endregion
    }
}
