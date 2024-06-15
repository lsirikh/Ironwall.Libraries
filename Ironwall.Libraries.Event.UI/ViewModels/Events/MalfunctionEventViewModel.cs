using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class MalfunctionEventViewModel
        : MetaEventViewModel, IMalfunctionEventViewModel
    {
        #region - Ctors -
        public MalfunctionEventViewModel()
        {

        }
        public MalfunctionEventViewModel(IMalfunctionEventModel model) : base(model)
        {
            //Reason = model.Reason;
            //FirstStart = model.FirstStart;
            //FirstEnd = model.FirstEnd;
            //SecondStart = model.SecondStart;
            //SecondEnd = model.SecondEnd;

            //Device = GetDevice(model.Device);
        }

        private IDeviceViewModel GetDevice(IBaseDeviceModel device)
        {
            IDeviceViewModel model = null;
            try
            {
                switch ((EnumDeviceType)device.DeviceType)
                {
                    case EnumDeviceType.NONE:
                        {

                        }
                        break;
                    case EnumDeviceType.Controller:
                        {
                            model = Libraries.Device.UI.ViewModels.ViewModelFactory.Build<ControllerDeviceViewModel>(device as IControllerDeviceModel);
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
                            model = Libraries.Device.UI.ViewModels.ViewModelFactory.Build<SensorDeviceViewModel>(device as ISensorDeviceModel);
                        }
                        break;
                    case EnumDeviceType.IpCamera:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }

            return model;
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
        public EnumFaultType Reason
        {
            get { return (_model as IMalfunctionEventModel).Reason; }
            set
            {
                (_model as IMalfunctionEventModel).Reason = value;
                NotifyOfPropertyChange(() => Reason);
            }
        }

        public int FirstStart
        {
            get { return (_model as IMalfunctionEventModel).FirstStart; }
            set
            {
                (_model as IMalfunctionEventModel).FirstStart = value;
                NotifyOfPropertyChange(() => FirstStart);
            }
        }

        public int FirstEnd
        {
            get { return (_model as IMalfunctionEventModel).FirstEnd; }
            set
            {
                (_model as IMalfunctionEventModel).FirstEnd = value;
                NotifyOfPropertyChange(() => FirstEnd);
            }
        }

        public int SecondStart
        {
            get { return (_model as IMalfunctionEventModel).SecondStart; }
            set
            {
                (_model as IMalfunctionEventModel).SecondStart = value;
                NotifyOfPropertyChange(() => SecondStart);
            }
        }

        public int SecondEnd
        {
            get { return (_model as IMalfunctionEventModel).SecondEnd; }
            set
            {
                (_model as IMalfunctionEventModel).SecondEnd = value;
                NotifyOfPropertyChange(() => SecondEnd);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
