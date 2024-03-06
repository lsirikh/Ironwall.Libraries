using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using Ironwall.Libraries.Onvif.DataProviders;
using OnvifControl.Libraries.Onvif.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.Models
{
    public class OnvifModel : IOnvifModel
    {
        #region - Ctors -
        public OnvifModel()
        {

        }
        public OnvifModel(
            ICameraDeviceModel cameraDeviceModel
            , IOnvifControlService onvifControlService = null
            , PTZPresetProvider pTZPresetProvider = null)
        {
            CameraDeviceModel = cameraDeviceModel;
            OnvifControlService = onvifControlService;
            PtzPresetProvider = pTZPresetProvider;
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
        public IOnvifControlService OnvifControlService { get; set; }
        public PTZPresetProvider PtzPresetProvider { get; set; }
        public bool IsMoving { get; set; } // Moving   :true
                                           // Idle     :false
        #endregion
        #region - Attributes -
        #endregion
    }
}
