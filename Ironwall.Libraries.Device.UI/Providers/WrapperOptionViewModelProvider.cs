using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Devices.Providers.Models;
using System.Linq;
using Ironwall.Libraries.Device.UI.ViewModels;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/17/2024 7:16:16 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class WrapperOptionViewModelProvider<T, P> : BaseProvider<P>, ILoadable where T : IBaseOptionModel where P : ICameraOptionViewModel
    {

        #region - Ctors -
        public WrapperOptionViewModelProvider(CameraOptionProvider provider)
        {
            ClassName = nameof(WrapperOptionViewModelProvider<T, P>);
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
                    var instance = (P)Activator.CreateInstance(typeof(P), new object[] { item });
                    Add(instance);
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
                        var instance = (P)Activator.CreateInstance(typeof(P), new object[] { newItem });
                        Add(instance);
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
                        var instance = (P)Activator.CreateInstance(typeof(P), new object[] { newItem });
                        Add(instance, index);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (T newItem in _provider.OfType<T>().ToList())
                    {
                        var instance = (P)Activator.CreateInstance(typeof(P), new object[] { newItem });
                        Add(instance);
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