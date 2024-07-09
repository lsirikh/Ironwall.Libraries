using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:36:36 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraDeviceViewModel : DeviceViewModel, ICameraDeviceViewModel
    {

        #region - Ctors -
        public CameraDeviceViewModel()
        {
            _model = new CameraDeviceModel();
        }
        
        public CameraDeviceViewModel(ICameraDeviceModel model) : base(model)
        {
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
        public string IpAddress
        {
            get { return (_model as ICameraDeviceModel).IpAddress; }
            set
            {
                (_model as ICameraDeviceModel).IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get { return (_model as ICameraDeviceModel).Port; }
            set
            {
                (_model as ICameraDeviceModel).Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }

        public string UserName
        {
            get { return (_model as ICameraDeviceModel).UserName; }
            set
            {
                (_model as ICameraDeviceModel).UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string Password
        {
            get { return (_model as ICameraDeviceModel).Password; }
            set
            {
                (_model as ICameraDeviceModel).Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public EnumCameraType Category
        {
            get { return (_model as ICameraDeviceModel).Category; }
            set
            {
                (_model as ICameraDeviceModel).Category = value;
                NotifyOfPropertyChange(() => Category);
            }
        }

        public List<CameraPresetModel> Presets
        {
            get { return (_model as ICameraDeviceModel).Presets; }
            set
            {
                (_model as ICameraDeviceModel).Presets = value;
                NotifyOfPropertyChange(() => Presets);
            }
        }

        public List<CameraProfileModel> Profiles
        {
            get { return (_model as ICameraDeviceModel).Profiles; }
            set
            {
                (_model as ICameraDeviceModel).Profiles = value;
                NotifyOfPropertyChange(() => Profiles);
            }
        }

        public string DeviceModel
        {
            get { return (_model as ICameraDeviceModel).DeviceModel; }
            set
            {
                (_model as ICameraDeviceModel).DeviceModel = value;
                NotifyOfPropertyChange(() => DeviceModel);
            }
        }

        public string RtspUri
        {
            get { return (_model as ICameraDeviceModel).RtspUri; }
            set
            {
                (_model as ICameraDeviceModel).RtspUri = value;
                NotifyOfPropertyChange(() => RtspUri);
            }
        }

        public int RtspPort
        {
            get { return (_model as ICameraDeviceModel).RtspPort; }
            set
            {
                (_model as ICameraDeviceModel).RtspPort = value;
                NotifyOfPropertyChange(() => RtspPort);
            }
        }

        public int Mode
        {
            get { return (_model as ICameraDeviceModel).Mode; }
            set
            {
                (_model as ICameraDeviceModel).Mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
