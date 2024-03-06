using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Windows;
using Wpf.Libraries.Surv.Common.Models;
using Ironwall.Libraries.Base.Services;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.Common.Services;
using System.Collections.ObjectModel;
using System.Threading;
using Ironwall.Framework.Services;
using Wpf.Libraries.Surv.UI.Providers.ViewModels;
using System.Diagnostics;
using Wpf.Libraries.Surv.UI.Models;
using Wpf.Libraries.Surv.Common.Sdk;

namespace Wpf.Libraries.Surv.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 8:53:01 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvSensorSetupViewModel : BaseDataGridPanel<SurvSensorViewModel>
                                            , IHandle<RefreshSurvSensorSetupMessageModel>
    {
        

        #region - Ctors -
        public SurvSensorSetupViewModel(ILogService log
                                        , IEventAggregator eventAggregator
                                        , SurvApiService apiService
                                        , SurvDbService dbService
                                        , SurvSensorViewModelProvider viewModelProvider
                                        , SurvSensorModelProvider survSensorModelProvider)
                                        : base(eventAggregator)
        {
            _log = log;
            _dbService = dbService;
            _provider = survSensorModelProvider;
            _apiService = apiService;
            _viewModelProvider = viewModelProvider;
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
                        ViewModelProvider.Remove(viewModel);
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
                await _eventAggregator.PublishOnUIThreadAsync(new OpenSurvSensorMatchingDialogMessageModel());
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
                await _dbService.FetchSurvSensorModel();
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
                await _dbService.InsertSurvSensorModel();
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

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (var item in _viewModelProvider)
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
                    foreach (SurvSensorViewModel newItem in e.NewItems)
                    {
                        _provider.Add(newItem.Model as SurvSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvSensorViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.Model as AudioModel);
                        _provider.Remove(newItem.Model as SurvSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvSensorViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.Model as AudioModel);
                        _provider.Remove(oldItem.Model as SurvSensorModel);
                    }
                    foreach (SurvSensorViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.Model as AudioModel);
                        _provider.Add(newItem.Model as SurvSensorModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    _provider.Clear();
                    foreach (SurvSensorViewModel item in ViewModelProvider)
                    {
                        _provider.Add(item.Model as SurvSensorModel);
                    }
                    break;
            }
        }

        #endregion
        #region - IHanldes -
        public async Task HandleAsync(RefreshSurvSensorSetupMessageModel message, CancellationToken cancellationToken)
        {
            await DataInitialize(cancellationToken);
        }
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private SurvDbService _dbService;
        private SurvSensorModelProvider _provider;
        private SurvApiService _apiService;
        private SurvSensorViewModelProvider _viewModelProvider;
        #endregion
    }
}
