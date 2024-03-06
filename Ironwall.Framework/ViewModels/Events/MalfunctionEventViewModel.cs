using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Events
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
            Reason = model.Reason;
            FirstStart = model.FirstStart;
            FirstEnd = model.FirstEnd;
            SecondStart = model.SecondStart;
            SecondEnd = model.SecondEnd;

            Device = GetDevice(model.Device);
        }

        private IBaseDeviceViewModel GetDevice(IBaseDeviceModel device)
        {
            IBaseDeviceViewModel model = null;
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
                            model = ViewModelFactory.Build<ControllerDeviceViewModel>(device as IControllerDeviceModel);
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
                            model =  ViewModelFactory.Build<SensorDeviceViewModel>(device as ISensorDeviceModel);
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
        private int _reason;

        public int Reason
        {
            get { return _reason; }
            set
            {
                _reason = value;
                NotifyOfPropertyChange(() => Reason);
            }
        }

        private int _firstStart;

        public int FirstStart
        {
            get { return _firstStart; }
            set
            {
                _firstStart = value;
                NotifyOfPropertyChange(() => FirstStart);
            }
        }

        private int _firstEnd;

        public int FirstEnd
        {
            get { return _firstEnd; }
            set
            {
                _firstEnd = value;
                NotifyOfPropertyChange(() => FirstEnd);
            }
        }

        private int _secondStart;

        public int SecondStart
        {
            get { return _secondStart; }
            set
            {
                _secondStart = value;
                NotifyOfPropertyChange(() => SecondStart);
            }
        }

        private int _secondEnd;

        public int SecondEnd
        {
            get { return _secondEnd; }
            set
            {
                _secondEnd = value;
                NotifyOfPropertyChange(() => SecondEnd);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
