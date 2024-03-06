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
    public sealed class ControllerDeviceProvider
        : DeviceBaseProvider 
    {
        #region - Ctors -
        public ControllerDeviceProvider(DeviceProvider provider)
        {
            ClassName = nameof(ControllerDeviceProvider);
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
            //var isValid = false;
            return Task.Run(() => 
            { 
                try
                {
                    Clear();
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Provider_Initialize)}({ClassName}) was executed!!!");
                    //var provider = _provider.ToList();
                    foreach (BaseDeviceModel item in _provider
                    .Where(entity => entity.DeviceType == (int)EnumDeviceType.Controller)
                    .ToList())
                    {
                        if (!(item is IControllerDeviceModel model)) continue;

                        //isValid = true;
                        Add(model);
                    }
                    return Task.FromResult(true);   
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return Task.FromResult(false);
                }
            });
            

            //if (isValid)
            //    return await Finished();
            //else
            //    return false;
        }

        private async Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            bool ret = false;
            try
            {
                if (item.DeviceType == (int)EnumDeviceType.Controller)
                {
                    if (!(item is IControllerDeviceModel model)) return false;

                    Debug.WriteLine($"[{model.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                    ret = await InsertedItem(model);
                }

                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return ret;
            }
            
        }

        private Task<bool> Provider_Delete(IBaseDeviceModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                {
                    Remove(searchedItem);
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
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
