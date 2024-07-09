using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.CameraOnvif.Models;
using Ironwall.Libraries.Devices.Providers;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.CameraOnvif.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 2:48:47 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifProvider : BaseProvider<IOnvifModel>, ILoadable
    {

        #region - Ctors -
        public OnvifProvider(CameraDeviceProvider provider)
        {
            ClassName = nameof(OnvifProvider);
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
                foreach (var item in _provider.OfType<ICameraDeviceModel>().ToList())
                {
                    var model = new OnvifModel(item);
                    Add(model);
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
                    foreach (CameraDeviceModel newItem in e.NewItems)
                    {
                        var item = new OnvifModel(newItem);
                        Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (CameraDeviceModel oldItem in e.OldItems)
                    {
                        var item = CollectionEntity.Where(entity => entity.CameraDeviceModel.Id == oldItem.Id).FirstOrDefault();
                        Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    int index = 0;
                    foreach (CameraDeviceModel oldItem in e.OldItems)
                    {
                        var item = CollectionEntity.Where(entity => entity.CameraDeviceModel.Id == oldItem.Id).FirstOrDefault();
                        index = CollectionEntity.IndexOf(item);
                        Remove(item);
                    }
                    foreach (CameraDeviceModel newItem in e.NewItems)
                    {
                        var item = new OnvifModel(newItem);
                        Add(item, index);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (var newItem in _provider.OfType<ICameraDeviceModel>().ToList())
                    {
                        var item = new OnvifModel(newItem);
                        Add(item);
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
        private CameraDeviceProvider _provider;
        #endregion
    }
}
