using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.CameraOnvif.Models;
using Ironwall.Libraries.Devices.Providers;
using System;
using System.Diagnostics;
using System.Linq;
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

    public sealed class OnvifProvider : OnvifBaseProvider
    {

        #region - Ctors -
        public OnvifProvider(CameraDeviceProvider provider)
        {
            ClassName = nameof(OnvifProvider);
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
            return Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Provider_Initialize)}({ClassName}) was executed!!!");
                    Clear();
                    foreach (var item in _provider.ToList())
                    {
                        if (!(item is ICameraDeviceModel model)) continue;

                        Add(new OnvifModel(model));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

            });
        }

        private Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    //Debug.WriteLine($"[{model.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                    Add(new OnvifModel(item as ICameraDeviceModel));
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> Provider_Delete(IBaseDeviceModel item)
        {
            bool ret = false;
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t == item as ICameraDeviceModel).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        Remove(searchedItem);
                        ret = true;
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
        private CameraDeviceProvider _provider;
        #endregion
    }
}
