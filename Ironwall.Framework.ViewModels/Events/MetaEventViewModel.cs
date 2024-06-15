using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.Events
{
    public class MetaEventViewModel
        : BaseEventViewModel, IMetaEventViewModel
    {
        #region - Ctors -
        public MetaEventViewModel()
        {
        }

        public MetaEventViewModel(IMetaEventModel model) : base(model)
        {
            EventGroup = model.EventGroup;
            MessageType = model.MessageType;
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
        private string _eventGroup;
        public string EventGroup
        {
            get { return _eventGroup; }
            set
            {
                _eventGroup = value;
                NotifyOfPropertyChange(() => EventGroup);
            }
        }

        private EnumEventType? _messageType;
        public EnumEventType? MessageType
        {
            get { return _messageType; }
            set
            {
                _messageType = value;
                NotifyOfPropertyChange(() => MessageType);
            }
        }

        private IBaseDeviceViewModel _device;
        public IBaseDeviceViewModel Device
        {
            get { return _device; }
            set
            {
                _device = value;
                NotifyOfPropertyChange(() => Device);
            }
        }
        
        private int _status;
        public int Status
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
        #endregion
    }
}