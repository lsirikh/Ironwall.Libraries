using Caliburn.Micro;
using Ironwall.Libraries.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public class CameraDeviceViewModel
        //: Screen
        //, ICameraDeviceViewModel
        : CameraBaseViewModel
        , ICameraDeviceViewModel
    {
        
        /*
        #region - Ctors -
        public CameraDeviceViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        public CameraDeviceViewModel(ICameraDeviceModel model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            Model = model;
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
        public int Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string HostName
        {
            get => Model.HostName;
            set
            {
                Model.HostName = value;
                NotifyOfPropertyChange(() => HostName);
            }
        }

        public string IpAddress
        {
            get => Model.IpAddress;
            set
            {
                Model.IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get => Model.Port;
            set
            {
                Model.Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }



        public string UserName
        {
            get => Model.UserName;
            set
            {
                Model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }


        public string Password
        {
            get => Model.Password;
            set
            {
                Model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        /////////////////////////////////////////////////////

        public string FirmwareVersion
        {
            get => Model.FirmwareVersion;
            set
            {
                Model.FirmwareVersion = value;
                NotifyOfPropertyChange(() => FirmwareVersion);
            }
        }

        public string HardwareId
        {
            get => Model.HardwareId;
            set
            {
                Model.HardwareId = value;
                NotifyOfPropertyChange(() => HardwareId);
            }
        }

        public string Manufacturer
        {
            get => Model.Manufacturer;
            set
            {
                Model.Manufacturer = value;
                NotifyOfPropertyChange(() => Manufacturer);
            }
        }

        public string DeviceModel
        {
            get => Model.DeviceModel;
            set
            {
                Model.DeviceModel = value;
                NotifyOfPropertyChange(() => DeviceModel);
            }
        }

        public string SerialNumber
        {
            get => Model.SerialNumber;
            set
            {
                Model.SerialNumber = value;
                NotifyOfPropertyChange(() => SerialNumber);
            }
        }

        public int Profile
        {
            get => Model.Profile;
            set
            {
                Model.Profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }

        public string Uri
        {
            get => Model.Uri;
            set
            {
                Model.Uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }
        public string Type
        {
            get => Model.Type;
            set
            {
                Model.Type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }


        /////////////////////////////////////////////////////


        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string RtspUri
        {
            get => Model.RtspUri;
            set
            {
                Model.RtspUri = value;
                NotifyOfPropertyChange(() => RtspUri);
            }
        }

        public int RtspPort
        {
            get => Model.RtspPort;
            set
            {
                Model.RtspPort = value;
                NotifyOfPropertyChange(() => RtspPort);
            }
        }

        public int TypeDevice
        {
            get => Model.TypeDevice;
            set
            {
                Model.TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public string Mac
        {
            get => Model.Mac;
            set
            {
                Model.Mac = value;
                NotifyOfPropertyChange(() => Mac);
            }
        }

        public int Mode
        {
            get => Model.Mode;
            set
            {
                Model.Mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public ICameraDeviceModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        
        #endregion
        #region - Attributes -
        private ICameraDeviceModel _model;
        private IEventAggregator _eventAggregator;

        private bool _isSelected;

        #endregion
*/
        #region - Ctors -
        public CameraDeviceViewModel()
        {
        }
        public CameraDeviceViewModel(ICameraDeviceModel model)
        {
            Model = model;
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
        public string HostName
        {
            get => Model.HostName;
            set
            {
                Model.HostName = value;
                NotifyOfPropertyChange(() => HostName);
            }
        }

        public string IpAddress
        {
            get => Model.IpAddress;
            set
            {
                Model.IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get => Model.Port;
            set
            {
                Model.Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }

        public string UserName
        {
            get => Model.UserName;
            set
            {
                Model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string Password
        {
            get => Model.Password;
            set
            {
                Model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        /////////////////////////////////////////////////////

        public string FirmwareVersion
        {
            get => Model.FirmwareVersion;
            set
            {
                Model.FirmwareVersion = value;
                NotifyOfPropertyChange(() => FirmwareVersion);
            }
        }

        public string HardwareId
        {
            get => Model.HardwareId;
            set
            {
                Model.HardwareId = value;
                NotifyOfPropertyChange(() => HardwareId);
            }
        }

        public string Manufacturer
        {
            get => Model.Manufacturer;
            set
            {
                Model.Manufacturer = value;
                NotifyOfPropertyChange(() => Manufacturer);
            }
        }

        public string DeviceModel
        {
            get => Model.DeviceModel;
            set
            {
                Model.DeviceModel = value;
                NotifyOfPropertyChange(() => DeviceModel);
            }
        }

        public string SerialNumber
        {
            get => Model.SerialNumber;
            set
            {
                Model.SerialNumber = value;
                NotifyOfPropertyChange(() => SerialNumber);
            }
        }

        public int Profile
        {
            get => Model.Profile;
            set
            {
                Model.Profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }

        public string Uri
        {
            get => Model.Uri;
            set
            {
                Model.Uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }
        public string Type
        {
            get => Model.Type;
            set
            {
                Model.Type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }
        /////////////////////////////////////////////////////

        public string Name
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string RtspUri
        {
            get => Model.RtspUri;
            set
            {
                Model.RtspUri = value;
                NotifyOfPropertyChange(() => RtspUri);
            }
        }

        public int RtspPort
        {
            get => Model.RtspPort;
            set
            {
                Model.RtspPort = value;
                NotifyOfPropertyChange(() => RtspPort);
            }
        }

        public int TypeDevice
        {
            get => Model.TypeDevice;
            set
            {
                Model.TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public string Mac
        {
            get => Model.Mac;
            set
            {
                Model.Mac = value;
                NotifyOfPropertyChange(() => Mac);
            }
        }

        public int Mode
        {
            get => Model.Mode;
            set
            {
                Model.Mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }


        public ICameraDeviceModel Model
        {
            get { return _model as ICameraDeviceModel; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }

}
