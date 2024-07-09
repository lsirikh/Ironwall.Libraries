using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.CameraOnvif.Providers;
using Ironwall.Libraries.CameraOnvif.Services;
using Ironwall.Libraries.Devices.Providers.Models;
using System.Threading;

namespace Ironwall.Libraries.CameraOnvif.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 2:22:46 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifModel : IOnvifModel
    {

        #region - Ctors -
        public OnvifModel()
        {

        }

        public OnvifModel(ICameraDeviceModel cameraDeviceModel
                        , OnvifControl onvifControl = null
                        //, CameraMappingProvider mappingProvider = null
                        , PtzPresetProvider pTZPresetProvider = null
                        )
        {
            CameraDeviceModel = cameraDeviceModel;
            OnvifControl = onvifControl;
            //MappingProvider = mappingProvider;
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
        public CancellationTokenSource Cts { get; set; }
        public ICameraDeviceModel CameraDeviceModel { get; set; }
        public OnvifControl OnvifControl { get; set; }
        public PtzPresetProvider PtzPresetProvider { get; set; }
        public bool IsMoving { get; set; } // Moving   :true
                                    // Idle     :false
        #endregion
        #region - Attributes -
        #endregion
    }
}
