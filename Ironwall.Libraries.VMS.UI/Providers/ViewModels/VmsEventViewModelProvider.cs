using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Vms;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.UI.ViewModels;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Sensorway.Events.Base.Models;

namespace Ironwall.Libraries.VMS.UI.Providers.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 8:49:57 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsEventViewModelProvider : EntityCollectionProvider<VmsEventViewModel>, ILoadable
    {
        #region - Ctors -
        public VmsEventViewModelProvider(VmsEventProvider provider)
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
                    var viewModel = new VmsEventViewModel(item);
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
        private async void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (EventModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new VmsEventViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (EventModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (EventModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (EventModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new VmsEventViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (EventModel newItem in _provider.ToList())
                    {
                        var viewModel = new VmsEventViewModel(newItem);
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
        private VmsEventProvider _provider;
        #endregion
    }
}