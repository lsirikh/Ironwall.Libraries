using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class ExEventTimerViewModel<T> : EventTimerViewModel<T>, IExEventTimerViewModel<T> where T : IBaseEventModel
    {
        #region - Ctros -
        public ExEventTimerViewModel(T model) : base(model)
        {
        }
        #endregion

        #region - Attributes -
        //protected string idGroup;
        //protected int idController;
        //protected int idSensor;
        //protected int typeMessage;
        //protected int typeDevice;
        //protected string device;
        //protected int sequence;.

        //protected string _eventGroup;
        //protected int _messageType;
        //protected int _status;
        //protected IBaseDeviceModel _device;
        protected int _map;
        //public delegate void ActionEvent(IActionRequestModel actionRequestModel);
        public delegate void ActionEvent(IActionResponseModel actionResponseModel);
        public abstract event ActionEvent CallAutoEvent;
        public abstract event ActionEvent CallActionEvent;

        public abstract event Action<PreEventViewModel> CallPreEvent;
        #endregion
    }
}