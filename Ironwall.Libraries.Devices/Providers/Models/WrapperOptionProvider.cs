using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.DataProviders;

namespace Ironwall.Libraries.Devices.Providers.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/17/2024 4:50:18 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class WrapperOptionProvider<T> : BaseProvider<T>, ILoadable where T : IBaseOptionModel
    {

        #region - Ctors -
        public WrapperOptionProvider(CameraOptionProvider provider)
        {
            ClassName = nameof(WrapperOptionProvider<T>);
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
                foreach (var item in _provider.OfType<T>().ToList())
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
        private CameraOptionProvider _provider;
        #endregion

    }
}