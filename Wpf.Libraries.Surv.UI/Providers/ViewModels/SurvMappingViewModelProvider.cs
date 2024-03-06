using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.UI.ViewModels;

namespace Wpf.Libraries.Surv.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 2:09:36 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvMappingViewModelProvider
        : EntityCollectionProvider<SurvMappingViewModel>, ILoadable
    {

        #region - Ctors -
        public SurvMappingViewModelProvider(SurvMappingModelProvider provider)
        {
            _provider = provider;
            _provider.CollectionEntity.CollectionChanged += CollectionEntity_CollectionChanged;
        }

        #endregion
        #region - Implementation of Interface -

        public Task<bool> Initialize(CancellationToken token = default)
        {
            try
            {
                Clear();
                foreach (var item in _provider)
                {
                    var eventModel = GetSurvEvent(item.EventId);
                    var viewModel = new SurvMappingViewModel(item, eventModel);
                    Add(viewModel);
                }

                return Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Raised exception in {nameof(Initialize)} : {ex.Message} ");
                return Task.FromResult(false);
            }
        }

        public void Uninitialize()
        {
            _provider.CollectionEntity.CollectionChanged -= CollectionEntity_CollectionChanged;
            Clear();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private ISurvEventModel GetSurvEvent(int evetId)
        {

            var eventProvider = IoC.Get<SurvEventModelProvider>();
            return eventProvider.Where(entity => entity.Id == evetId).FirstOrDefault();
        }
        private async void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (SurvMappingModel newItem in e.NewItems)
                    {
                        var eventModel = GetSurvEvent(newItem.EventId);
                        var viewModel = new SurvMappingViewModel(newItem, eventModel);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvMappingModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvMappingModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (SurvMappingModel newItem in e.NewItems)
                    {
                        var eventModel = GetSurvEvent(newItem.EventId);
                        var viewModel = new SurvMappingViewModel(newItem, eventModel);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (SurvMappingModel newItem in _provider.ToList())
                    {
                        var eventModel = GetSurvEvent(newItem.EventId);
                        var viewModel = new SurvMappingViewModel(newItem, eventModel);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
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
        private SurvMappingModelProvider _provider;
        #endregion
    }
}
