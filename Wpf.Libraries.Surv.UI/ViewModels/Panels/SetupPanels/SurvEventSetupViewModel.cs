using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Windows;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.Common.Services;
using Wpf.Libraries.Surv.UI.Providers.ViewModels;
using Ironwall.Framework.Services;
using Wpf.Libraries.Surv.Common.Sdk;

namespace Wpf.Libraries.Surv.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 10:59:16 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvEventSetupViewModel : BaseDataGridPanel<SurvEventViewModel>
    {
        #region - Ctors -
        public SurvEventSetupViewModel(ILogService log
                                    , IEventAggregator eventAggregator
                                    , SurvApiService apiService
                                    , SurvDbService dbService
                                    , SurvEventModelProvider survEventModelProvider
                                    , SurvApiModelProvider survApiModelProvider
                                    , SurvCameraModelProvider survCameraModelProvider
                                    , SurvEventViewModelProvider survEventViewModelProvider)
                                    : base(eventAggregator, log)
        {
            _dbService = dbService;
            _provider = survEventModelProvider;
            _apiService = apiService;
            ApiModelProvider = survApiModelProvider;
            _cameraModelProvider = survCameraModelProvider;
            _survEventViewModelProvider = survEventViewModelProvider;
        }

        #endregion
        #region - Implementation of Interface -
        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e) 
        {
            try
            {
                foreach (var viewModel in ViewModelProvider.ToList())
                {
                    if (viewModel.IsSelected)
                    {
                        var cameraModel = _cameraModelProvider
                            .Where(entity=> entity == viewModel.Camera).FirstOrDefault();
                        if (cameraModel != null)
                        {
                            _cameraModelProvider.Remove(cameraModel);
                        }

                        ViewModelProvider.Remove(viewModel);
                    }
                }
                await SelectAll(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickDeleteButton)} in {ClassName}): {ex.Message}");
            }
        }

        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            try
            {
                int camid = 0;
                if (_cameraModelProvider.Count > 0)
                    camid = _cameraModelProvider.Max(entity => entity.Id);

                var camera = new SurvCameraModel(camid + 1, "", "", 80, "", "", 0, "");
                _cameraModelProvider.Add(camera);

                int evntid = 0;
                if (ViewModelProvider.Count > 0)
                    evntid = ViewModelProvider.Max(entity => entity.Id);

                var model = new SurvEventModel(evntid + 1, 0, "", cameraId:camera.Id);

                var viewModel = new SurvEventViewModel(model, null, camera);
                await viewModel.ActivateAsync();
                ViewModelProvider.Add(viewModel);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickInsertButton)} in {ClassName}): {ex.Message}");
            }
        }

        public override async void OnClickReloadButton(object sender, RoutedEventArgs e)
        {
            ButtonEnableControl(false, true, false);
            try
            {
                await _dbService.FetchSurvCameraModel();
                await _dbService.FetchSurvEventModel();
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

        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            ButtonEnableControl(false, false, true);
            try
            {
                await _dbService.InsertSurvEventModel();
                await _dbService.InsertSurvCameraModel();
                await DataInitialize(_cancellationTokenSource.Token);
                _apiService.CreateLookupTable();
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
                    var provider = IoC.Get<SurvEventViewModelProvider>();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (var item in provider)
                        {
                            ViewModelProvider.Add(item);
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
                    foreach (SurvEventViewModel newItem in e.NewItems)
                    {
                        _provider.Add(newItem.Model as SurvEventModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvEventViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.Model as AudioModel);
                        _provider.Remove(newItem.Model as SurvEventModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvEventViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.Model as AudioModel);
                        _provider.Remove(oldItem.Model as SurvEventModel);
                    }
                    foreach (SurvEventViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.Model as AudioModel);
                        _provider.Add(newItem.Model as SurvEventModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    _provider.Clear();
                    foreach (SurvEventViewModel item in ViewModelProvider)
                    {
                        _provider.Add(item.Model as SurvEventModel);
                    }
                    break;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SurvApiModelProvider ApiModelProvider { get; }
        #endregion
        #region - Attributes -
        private SurvDbService _dbService;
        private SurvEventModelProvider _provider;
        private SurvApiService _apiService;
        private SurvCameraModelProvider _cameraModelProvider;
        private SurvEventViewModelProvider _survEventViewModelProvider;
        #endregion
    }
}
