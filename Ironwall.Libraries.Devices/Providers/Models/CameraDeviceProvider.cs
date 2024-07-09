using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Providers
{
    public sealed class CameraDeviceProvider : WrapperDeviceProvider<CameraDeviceModel>
    {
        #region - Ctors -
        public CameraDeviceProvider(DeviceProvider provider) : base(provider)
        {
            ClassName = nameof(CameraDeviceProvider);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }



    //    : BaseProvider<ICameraDeviceModel>, ILoadable
    //{
    //    #region - Ctors -
    //    public CameraDeviceProvider(DeviceProvider provider)
    //    {
    //        ClassName = nameof(CameraDeviceProvider);
    //        _provider = provider;
    //        _provider.CollectionEntity.CollectionChanged += CollectionEntity_CollectionChanged;
    //    }
    //    #endregion
    //    #region - Implementation of Interface -
    //    public Task<bool> Initialize(CancellationToken token = default)
    //    {
    //        try
    //        {
    //            Clear();
    //            foreach (var item in _provider.OfType<ICameraDeviceModel>().ToList())
    //            {
    //                var model = new CameraDeviceModel(item);
    //                Add(model);
    //            }

    //            return Task.FromResult(true);
    //        }
    //        catch (System.Exception ex)
    //        {
    //            Debug.WriteLine($"Raised exception in {nameof(Initialize)} of {ClassName}: {ex.Message} ");
    //            return Task.FromResult(false);
    //        }
    //    }

    //    public void Uninitialize()
    //    {
    //        _provider.CollectionEntity.CollectionChanged -= CollectionEntity_CollectionChanged;
    //        Clear();
    //    }
    //    #endregion
    //    #region - Overrides -
    //    #endregion
    //    #region - Binding Methods -
    //    #endregion
    //    #region - Processes -
    //    private void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        switch (e.Action)
    //        {
    //            case NotifyCollectionChangedAction.Add:
    //                // New items added
    //                foreach (var newItem in e.NewItems.OfType<ICameraDeviceModel>().ToList())
    //                {
    //                    Add(newItem);
    //                }
    //                break;

    //            case NotifyCollectionChangedAction.Remove:
    //                // Items removed
    //                foreach (var oldItem in e.OldItems.OfType<ICameraDeviceModel>().ToList())
    //                {
    //                    var entity = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
    //                    Remove(entity);
    //                }
    //                break;

    //            case NotifyCollectionChangedAction.Replace:
    //                // Some items replaced
    //                int index = 0;
    //                foreach (var oldItem in e.OldItems.OfType<ICameraDeviceModel>().ToList())
    //                {
    //                    var entity = CollectionEntity.Where(entity => entity.Id == oldItem.Id).FirstOrDefault();
    //                    index = CollectionEntity.IndexOf(entity);
    //                    Remove(entity);
    //                }
    //                foreach (var newItem in e.NewItems.OfType<ICameraDeviceModel>().ToList())
    //                {
    //                    Add(newItem, index);
    //                }
    //                break;

    //            case NotifyCollectionChangedAction.Reset:
    //                // The whole list is refreshed
    //                CollectionEntity.Clear();
    //                foreach (var newItem in _provider.OfType<ICameraDeviceModel>().ToList())
    //                {
    //                    Add(newItem);
    //                }
    //                break;
    //        }
    //    }
    //    #endregion
    //    #region - IHanldes -
    //    #endregion
    //    #region - Properties -
    //    #endregion
    //    #region - Attributes -
    //    private DeviceProvider _provider;
    //    #endregion
    //}
}
