using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Devices.Providers.Models;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/6/2023 9:08:01 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MappingViewModelProvider : BaseProvider<CameraMappingViewModel>, ILoadable
    {
        #region - Ctors -
        public MappingViewModelProvider(CameraMappingProvider provider)
        {
            ClassName = nameof(MappingViewModelProvider);
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
                foreach (var item in _provider.ToList())
                {
                    Add(new CameraMappingViewModel(item));
                }

                return Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Raised exception in {nameof(Initialize)} of {ClassName}: {ex.Message} ");
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
        private void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (CameraMappingModel newItem in e.NewItems)
                    {
                        Add(new CameraMappingViewModel(newItem));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (CameraMappingModel oldItem in e.OldItems)
                    {
                        var instance = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        Remove(instance);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    int index = 0;
                    foreach (CameraMappingModel oldItem in e.OldItems)
                    {
                        var instance = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        var entity = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        index = CollectionEntity.IndexOf(entity);
                        Remove(instance);
                    }
                    foreach (CameraMappingModel newItem in e.NewItems)
                    {
                        Add(new CameraMappingViewModel(newItem), index);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (CameraMappingModel newItem in _provider.ToList())
                    {
                        Add(new CameraMappingViewModel(newItem));
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
        private CameraMappingProvider _provider;
        #endregion
    }
}
