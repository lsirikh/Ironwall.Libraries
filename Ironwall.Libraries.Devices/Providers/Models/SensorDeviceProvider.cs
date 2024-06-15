using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Ironwall.Libraries.Devices.Providers
{
    public class SensorDeviceProvider 
        : DeviceProvider 
    {
        #region - Ctors -
        public SensorDeviceProvider(DeviceProvider provider)
        {
            ClassName = nameof(SensorDeviceProvider);
            _provider = provider;
            _provider.Refresh += Provider_Initialize;
            _provider.Inserted += Provider_Insert;
            _provider.Deleted += Provider_Delete;
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
            return Task.Run(async () =>
            {
                var isValid = false;
                try
                {
                    Clear();
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Provider_Initialize)}({ClassName}) was executed!!!");
                    foreach (BaseDeviceModel item in _provider
                    .Where(entity => 
                    entity.DeviceType == EnumDeviceType.Multi
                     || entity.DeviceType == EnumDeviceType.Fence
                     || entity.DeviceType == EnumDeviceType.Contact
                     || entity.DeviceType == EnumDeviceType.Laser
                     || entity.DeviceType == EnumDeviceType.Underground
                     || entity.DeviceType == EnumDeviceType.PIR
                     || entity.DeviceType == EnumDeviceType.IoController
                     )
                    .ToList())
                    {
                        if (!(item is ISensorDeviceModel model)) continue;

                        isValid = true;
                        Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

                if (isValid)
                    return await Finished();
                else
                    return false;
            });
        }
        private Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    if (item.DeviceType == EnumDeviceType.Multi
                     || item.DeviceType == EnumDeviceType.Fence
                     || item.DeviceType == EnumDeviceType.Contact
                     || item.DeviceType == EnumDeviceType.Laser
                     || item.DeviceType == EnumDeviceType.Underground
                     || item.DeviceType == EnumDeviceType.PIR
                     || item.DeviceType == EnumDeviceType.IoController)
                    {
                        if (!(item is ISensorDeviceModel model)) return false;

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
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        Remove(searchedItem);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private DeviceProvider _provider;
        #endregion
    }
}
