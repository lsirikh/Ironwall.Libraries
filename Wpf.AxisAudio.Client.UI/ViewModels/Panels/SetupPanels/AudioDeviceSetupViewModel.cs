using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Framework.Services;
using System.Diagnostics;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Common.Models;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using System.Linq;
using System;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Client.UI.Models;
using System.Windows.Documents;
using System.Collections.Generic;
using Ironwall.Libraries.Base.Services;
using Wpf.AxisAudio.Common;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/19/2023 5:44:52 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioDeviceSetupViewModel :BaseDataGridPanel<AudioViewModel>
    {

        #region - Ctors -
        public AudioDeviceSetupViewModel(IEventAggregator eventAggregator
                                        , ILogService log
                                        , AudioDbService dbService
                                        , AudioProvider audioProvider
                                        , AxisApiService apiService
                                        , AudioSymbolProvider audioSymbolProvider
                                        , AudioGroupViewModelProvider audioGroupViewModelProvider
                                        )
                                        : base(eventAggregator, log) 
        {
            Names = new ObservableCollection<string>();
            _dbService = dbService;
            _provider = audioProvider;
            _apiService = apiService;
            _audioSymbolProvider = audioSymbolProvider;
            _audioGroupViewModelProvider = audioGroupViewModelProvider;
        }

        #endregion
        #region - Implementation of Interface -
        /// <summary>
        /// 삭제 시, ViewModelProvider에서 삭제를 하고,
        /// ViewModelProvider_CollectionChanged는 연동된 그룹을 삭제할 수 있는 로직을
        /// 포함하고 있다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in ViewModelProvider.ToList())
                {
                    if (item.IsSelected)
                        ViewModelProvider.Remove(item);
                }
                await SelectAll(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickDeleteButton)} in {ClassName}) : {ex.Message}");
            }
        }

        /// <summary>
        /// AudioModel을 ViewModelProvider를 통해서 추가하고, 
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

                var audio = new AudioModel(id + 1, "None", "", "", "", 80);
                
                var viewModel = new AudioViewModel(audio);
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
        /// 
        /// [Deprecated]
        /// 데이터 Reload 시, AudioGroup 쪽의 변경되었지만, 저장되지 않은 데이터와
        /// 새로 Reload된, AudioModel 사이의 불일치가 발생할 수 있기 때문에
        /// 반드시 연관된 모델들의 Provider는 재갱신(즉, Reload)이 필요하다.
        /// 상호간의 강한 Dependency 때문에 이러한 복잡한 로직이 필요하게 된다.
        /// 다양한 시나리오를 통해 테스트 한 결과 서로 Dependency가 묶인 상황에서는
        /// Fetch와 Save를 함께 하는 것이 유용하다.
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
                    var provider = IoC.Get<AudioViewModelProvider>();
                    await provider.Initialize();

                    SetNames(_audioSymbolProvider.Select(entity => entity.NameDevice).ToList());

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (AudioViewModel item in provider)
                        {
                            ViewModelProvider.Add(item);

                            //디버깅용
                            Debug.WriteLine($"[오디오모델] " +
                                $"ID : {item.Id}, " +
                                $"Groups : {item.Groups?.Count()}, " +
                                $"DeviceName : {item.DeviceName}, " +
                                $"Password : {item.Password}, " +
                                $"IpAddress : {item.IpAddress}, " +
                                $"Port : {item.Port}, " +
                                $"Mode : {item.Mode}");
                            if(item.Groups != null)
                            {
                                foreach (var group in item.Groups) 
                                {
                                    Debug.WriteLine($"[오디오모델-오디오그룹] " +
                                        $"ID : {group.Id}, " +
                                        $"GroupNumber : {group.GroupNumber}, " +
                                        $"GroupName : {group.GroupName}");
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
                    foreach (AudioViewModel newItem in e.NewItems)
                    {
                        _provider.Add(newItem.Model as AudioModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.ViewModel as AudioModels);
                        var groups = _audioGroupViewModelProvider
                            .Where(entity => newItem.Groups.Any(innerEntity => innerEntity == entity.Model));
                        foreach (var group in groups)
                        {
                            group.AudioModels.Remove(newItem.Model as AudioModel);
                        }

                        _provider.Remove(newItem.Model as AudioModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.ViewModel as AudioModels);
                        _provider.Remove(oldItem.Model as AudioModel);
                    }
                    foreach (AudioViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.ViewModel as AudioModels);
                        _provider.Add(newItem.Model as AudioModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    _provider.Clear();
                    foreach (AudioViewModel item in ViewModelProvider)
                    {
                        _provider.Add(item.Model as AudioModel);
                    }
                    break;
            }
        }

        public void SetNames(List<string> names)
        {
            DispatcherService.Invoke((System.Action)(() =>
            {
                Names.Clear();
                foreach (var item in names)
                {
                    Names.Add(item);
                }
            }));
           
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<string> Names { get; set; }
        #endregion
        #region - Attributes -
        private AudioDbService _dbService;
        private AudioProvider _provider;
        private AxisApiService _apiService;
        private AudioSymbolProvider _audioSymbolProvider;
        private AudioGroupViewModelProvider _audioGroupViewModelProvider;
        #endregion
    }
}
