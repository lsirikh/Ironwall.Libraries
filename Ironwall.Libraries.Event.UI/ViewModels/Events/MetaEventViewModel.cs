using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System.Runtime.InteropServices;
using System;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.ViewModels.Devices;
using SensorDeviceViewModel = Ironwall.Framework.ViewModels.Devices.SensorDeviceViewModel;
using ControllerDeviceViewModel = Ironwall.Framework.ViewModels.Devices.ControllerDeviceViewModel;
using CameraDeviceViewModel = Ironwall.Framework.ViewModels.Devices.CameraDeviceViewModel;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class MetaEventViewModel : BaseEventViewModel<IMetaEventModel>, IMetaEventViewModel
    {
        #region - Ctors -
        public MetaEventViewModel()
        {
            _model = new MetaEventModel();
        }

        public MetaEventViewModel(IMetaEventModel model) : base(model)
        {
            Device = DeviceBuilder(model);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new MetaEventModel();
            GC.Collect();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private IBaseDeviceViewModel DeviceBuilder(IMetaEventModel model)
        {
            IBaseDeviceViewModel deviceViewModel = null;

            switch ((EnumDeviceType)model?.Device?.DeviceType)
            {
                case EnumDeviceType.NONE:
                    break;
                case EnumDeviceType.Controller:
                    {
                        deviceViewModel = new ControllerDeviceViewModel(model.Device as IControllerDeviceModel);
                    }
                    break;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    {
                        deviceViewModel = new SensorDeviceViewModel(model.Device as ISensorDeviceModel);
                    }
                    break;
                case EnumDeviceType.IpCamera:
                    {
                        deviceViewModel = new CameraDeviceViewModel(model.Device as ICameraDeviceModel);
                    }
                    break;
                case EnumDeviceType.Fence_Line:
                    break;
                default:
                    break;
            }

            return deviceViewModel;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string EventGroup
        {
            get { return _model.EventGroup; }
            set
            {
                _model.EventGroup = value;
                NotifyOfPropertyChange(() => EventGroup);
            }
        }

        public EnumEventType? MessageType
        {
            get { return _model.MessageType; }
            set
            {
                _model.MessageType = value;
                NotifyOfPropertyChange(() => MessageType);
            }
        }

        public IBaseDeviceViewModel Device
        {
            get { return _device; }
            set
            {
                _device = value;
                NotifyOfPropertyChange(() => Device);
            }
        }

        public EnumTrueFalse Status
        {
            get { return _model.Status; }
            set
            {
                _model.Status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }


        #endregion
        #region - Attributes -
        private IBaseDeviceViewModel _device;
        #endregion
    }
}