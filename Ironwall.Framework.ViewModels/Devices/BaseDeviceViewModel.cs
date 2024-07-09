using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Devices
{
    public class BaseDeviceViewModel : Screen, IBaseDeviceViewModel
    {
        #region - Ctors -
        public BaseDeviceViewModel()
        {

        }
        public BaseDeviceViewModel(IBaseDeviceModel model)
        {
            Id = model.Id;
            DeviceGroup = model.DeviceGroup;
            DeviceNumber = model.DeviceNumber;
            DeviceName = model.DeviceName;
            DeviceType = model.DeviceType;
            Version = model.Version;
            Status = model.Status;
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
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        private int _deviceGroup;

        public int DeviceGroup
        {
            get { return _deviceGroup; }
            set 
            {
                _deviceGroup = value;
                NotifyOfPropertyChange(() => DeviceGroup);
            }
        }


        private int _deviceNumber;
        public int DeviceNumber
        {
            get { return _deviceNumber; }
            set
            {
                _deviceNumber = value;
                NotifyOfPropertyChange(() => DeviceNumber);
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                _deviceName = value;
                NotifyOfPropertyChange(() => DeviceName);
            }
        }

        private EnumDeviceType _deviceType;
        public EnumDeviceType DeviceType
        {
            get { return _deviceType; }
            set
            {
                _deviceType = value;
                NotifyOfPropertyChange(() => DeviceType);
            }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                NotifyOfPropertyChange(() => Version);
            }
        }

        private EnumDeviceStatus _status;
        public EnumDeviceStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        #endregion
        #region - Attributes -
        public IEventAggregator _eventAggregator { get; set; }
        #endregion
    }
}
