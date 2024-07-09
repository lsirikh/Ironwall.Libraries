using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Providers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics;

namespace Ironwall.Libraries.Events.Providers.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 3:09:56 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class WrapperEventProvider<T> : BaseProvider<T>, ILoadable where T : IMetaEventModel
    {

        #region - Ctors -
        public WrapperEventProvider(EventProvider provider)
        {
            ClassName = nameof(WrapperEventProvider<T>);
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
                foreach (T item in _provider.OfType<T>().ToList())
                {
                    //var instance = (T)Activator.CreateInstance(typeof(T), new object[] { item });
                    Add(item);
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
                    foreach (T newItem in e.NewItems.OfType<T>().ToList())
                    {
                        Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (T oldItem in e.OldItems.OfType<T>().ToList())
                    {
                        var instance = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        Remove(instance);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    int index = 0;
                    foreach (T oldItem in e.OldItems.OfType<T>().ToList())
                    {
                        var instance = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        var entity = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
                        index = CollectionEntity.IndexOf(entity);
                        Remove(instance);
                    }
                    foreach (T newItem in e.NewItems.OfType<T>().ToList())
                    {
                        Add(newItem, index);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (T newItem in _provider.OfType<T>().ToList())
                    {
                        Add(newItem);
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
        private EventProvider _provider;
        #endregion

    }
}