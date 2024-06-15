using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Providers
{
    public sealed class CameraDeviceProvider
        : DeviceBaseProvider
    {
        #region - Ctors -
        public CameraDeviceProvider(DeviceProvider provider)
        {
            ClassName = nameof(CameraDeviceProvider);
            _deviceProvider = provider;
            _deviceProvider.Refresh += Provider_Initialize;
            _deviceProvider.Inserted += Provider_Insert;
            _deviceProvider.Deleted += Provider_Delete;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task<bool> Provider_Initialize()
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Provider_Initialize)}({ClassName}) was executed!!!");
                    Clear();
                    foreach (BaseDeviceModel item in _deviceProvider
                    .Where(entity => entity.DeviceType == EnumDeviceType.IpCamera)
                    .ToList())
                    {
                        if (!(item is ICameraDeviceModel model)) continue;

                        Add(model);
                    }

                    ret = await Finished();
                    return ret;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                
            });
        }

        private Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    if (item.DeviceType == EnumDeviceType.IpCamera)
                    {
                        if (!(item is ICameraDeviceModel model)) return false;

                        Debug.WriteLine($"[{model.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                        ret = await InsertedItem(model);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }

        private Task<bool> Provider_Delete(IBaseDeviceModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        ret = await DeletedItem(searchedItem);
                    }
                    return ret;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                    return ret;
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private DeviceProvider _deviceProvider;
        #endregion
    }
}
