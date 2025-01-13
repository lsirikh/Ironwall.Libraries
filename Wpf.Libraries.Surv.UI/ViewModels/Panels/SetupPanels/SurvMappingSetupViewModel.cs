using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.Common.Services;
using System.Linq;
using Wpf.Libraries.Surv.UI.Providers.ViewModels;
using Ironwall.Framework.Services;
using Wpf.Libraries.Surv.Common.Sdk;

namespace Wpf.Libraries.Surv.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:00:30 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvMappingSetupViewModel : BaseDataGridPanel<SurvMappingViewModel>
    {
        #region - Ctors -
        public SurvMappingSetupViewModel(ILogService log
                                        , IEventAggregator eventAggregator
                                        , SurvDbService dbService
                                        , SurvApiService apiService
                                        , SurvEventModelProvider survEventModelProvider
                                        , SurvMappingModelProvider survMappingModelProvider)
                                        : base(eventAggregator, log)
        {
            _dbService = dbService;
            _apiService = apiService;
            _provider = survMappingModelProvider;
            SurvEventModelProvider = survEventModelProvider;
            ViewModelProvider = new ObservableCollection<SurvMappingViewModel>();
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
                int id = 0;
                if (ViewModelProvider.Count > 0)
                    id = ViewModelProvider.Max(entity => entity.Id);

                var model = new SurvMappingModel(id + 1, 0, "", 0);

                var viewModel = new SurvMappingViewModel(model, null);
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
                await _dbService.FetchSurvMappingModel();
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
                await _dbService.InsertSurvMappingModel();
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
                    var provider = IoC.Get<SurvMappingViewModelProvider>();

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
                    foreach (SurvMappingViewModel newItem in e.NewItems)
                    {
                        _provider.Add(newItem.Model as SurvMappingModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvMappingViewModel newItem in e.OldItems)
                    {
                        //provider.Remove(newItem.Model as AudioModel);
                        _provider.Remove(newItem.Model as SurvMappingModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvMappingViewModel oldItem in e.OldItems)
                    {
                        //provider.Remove(oldItem.Model as AudioModel);
                        _provider.Remove(oldItem.Model as SurvMappingModel);
                    }
                    foreach (SurvMappingViewModel newItem in e.NewItems)
                    {
                        //provider.Add(newItem.Model as AudioModel);
                        _provider.Add(newItem.Model as SurvMappingModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    _provider.Clear();
                    foreach (SurvMappingViewModel item in ViewModelProvider)
                    {
                        _provider.Add(item.Model as SurvMappingModel);
                    }
                    break;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private SurvDbService _dbService;
        private SurvApiService _apiService;
        private SurvMappingModelProvider _provider;
        public SurvEventModelProvider SurvEventModelProvider { get; }
        #endregion
    }
}
