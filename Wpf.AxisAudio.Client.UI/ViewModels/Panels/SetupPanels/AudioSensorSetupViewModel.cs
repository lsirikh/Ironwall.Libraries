using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Common.Models;
using System.Linq;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using System.Windows.Controls;
using Wpf.AxisAudio.Client.UI.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 1:35:58 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSensorSetupViewModel : BaseDataGridPanel<AudioSensorViewModel>
                                        , IHandle<RefreshAudioSensorSetupMessageModel>
    {
        #region - Ctors -
        public AudioSensorSetupViewModel(ILogService log
                                        , IEventAggregator eventAggregator
                                        , AxisApiService apiService
                                        , AudioDbService dbService
                                        , AudioSensorProvider audioSensorProvider
                                        , AudioGroupViewModelProvider audioGroupViewModelProvider)
                                        : base(eventAggregator, log)
        {
            _apiService = apiService;
            _dbService = dbService;
            _provider = audioSensorProvider;
            _audioGroupViewModelProvider = audioGroupViewModelProvider;
        }
        #endregion
        #region - Implementation of Interface -
        /// <summary>
        /// 데이터 삭제시, ViewModelProvider에서 데이터를 삭제하고,
        /// 그 연동된 Provider에서도 데이터를 같이 삭제를 해주고, 또한
        /// GroupProvider에 등록된 SensorModel들도 삭제해준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var isRemoved = false;
                foreach (var viewModel in ViewModelProvider.ToList())
                {
                    if (viewModel.IsSelected)
                    {
                        ViewModelProvider.Remove(viewModel);
                        isRemoved = true;
                        
                        var groupProvider = _audioGroupViewModelProvider
                            .OfType<AudioGroupViewModel>()
                            .Where(entity => entity.AudioSensorModels
                            .Contains(viewModel.Model)).ToList();

                        if (groupProvider == null) continue;
                        foreach (var item in groupProvider)
                        {
                            item.AudioSensorModels.Remove(viewModel.Model as AudioSensorModel);
                        }
                    }
                }

                if (isRemoved) 
                    await DataInitialize();

                await SelectAll(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickDeleteButton)} in {ClassName}): {ex.Message}");
            }
        }

        /// <summary>
        /// 추가 시, OpenAudioSensorMatchingDialogMessageModel를 통해서 Dialog를 띄우고,
        /// 그걸 통해서 입력작을 수행한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            try
            {
                await _eventAggregator.PublishOnUIThreadAsync(new OpenAudioSensorMatchingDialogMessageModel());
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickInsertButton)} in {ClassName}): {ex.Message}");
            }
        }

        /// <summary>
        /// 데이터 Reload 시, AudioSensorModel은 Group을 1:1 Mapping하고 있기 때문에
        /// 특별히 AudioGroup쪽의 재갱신은 필요하지 않다. 
        /// 상호간의 Dependency가 약한 편이다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickReloadButton(object sender, RoutedEventArgs e)
        {
            ButtonEnableControl(false, true, false);
            try
            {
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
        /// 데이터 Save 시, AudioSensorModel만 신경쓰면된다.
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
            _ = DataInitialize(_pCancellationTokenSource.Token);
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
                    var provider = IoC.Get<AudioSensorViewModelProvider>();
                    await provider.Initialize();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (var item in provider)
                        {
                            ViewModelProvider.Add(item);
                            Debug.WriteLine($"[오디오그룹-오디오센서] " +
                                        $"ID : {item.Id}, " +
                                        $"Group : {item.Group?.GroupName}, " +
                                        $"DeviceName : {item.DeviceName}, " +
                                        $"ControllerId : {item.ControllerId}, " +
                                        $"SensorId : {item.SensorId}, " +
                                        $"DeviceType : {item.DeviceType}");
                        }

                    }));
                    ViewModelProvider.CollectionChanged += ViewModelProvider_CollectionChanged;
                    NotifyOfPropertyChange(() => ViewModelProvider);
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)}) : {ex.Message}");
                }

            });
        }

        private void ViewModelProvider_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (AudioSensorViewModel newItem in e.NewItems)
                    {
                        _provider.Add(newItem.Model as AudioSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioSensorViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.ViewModel as AudioModels);
                        _provider.Remove(newItem.Model as AudioSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioSensorViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.ViewModel as AudioModels);
                        _provider.Remove(oldItem.Model as AudioSensorModel);
                    }
                    foreach (AudioSensorViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.ViewModel as AudioModels);
                        _provider.Add(newItem.Model as AudioSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    _provider.Clear();
                    foreach (AudioSensorViewModel item in ViewModelProvider)
                    {
                        _provider.Add(item.Model as AudioSensorModel);
                    }
                    break;
            }
        }

        #endregion
        #region - IHanldes -
        public async Task HandleAsync(RefreshAudioSensorSetupMessageModel message, CancellationToken cancellationToken)
        {
            await DataInitialize(cancellationToken);
        }
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private AxisApiService _apiService;
        private AudioDbService _dbService;
        private AudioSensorProvider _provider;
        private AudioGroupViewModelProvider _audioGroupViewModelProvider;
        #endregion
    }
}
