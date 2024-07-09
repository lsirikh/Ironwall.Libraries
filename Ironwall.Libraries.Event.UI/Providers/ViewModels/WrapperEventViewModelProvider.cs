using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Providers;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 3:27:25 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class WrapperEventViewModelProvider<T, P> : BaseProvider<P>, ILoadable
        where T : IMetaEventModel 
        where P : IMetaEventViewModel
    {

        #region - Ctors -
        public WrapperEventViewModelProvider(EventProvider provider)
        {
            ClassName = nameof(WrapperEventViewModelProvider<T, P>);
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
            catch (Exception)
            {
                throw;
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
            try
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
            catch (Exception)
            {

                throw;
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