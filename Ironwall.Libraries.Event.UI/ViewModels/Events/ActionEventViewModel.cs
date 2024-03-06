using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class ActionEventViewModel
        : BaseEventViewModel<IActionEventModel>, IActionEventViewModel
    {
        #region - Ctors -
        public ActionEventViewModel()
        {
        }

        public ActionEventViewModel(IActionEventModel model) : base(model)
        {
            try
            {
                FromEvent = EventBuilder(model);
            }
            catch 
            {
            }

            Content = model.Content;
            User = model.User;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new ActionEventModel();
            GC.Collect();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private MetaEventViewModel EventBuilder(IActionEventModel model)
        {
            MetaEventViewModel eventViewModel = null;
            switch ((EnumEventType)model.FromEvent.MessageType)
            {
                case EnumEventType.Intrusion:
                    {
                        eventViewModel = ViewModelFactory.Build<DetectionEventViewModel>(model.FromEvent);
                    }
                    break;
                case EnumEventType.Fault:
                    {
                        eventViewModel = ViewModelFactory.Build<MalfunctionEventViewModel>(model.FromEvent);
                    }
                    break;
                case EnumEventType.ContactOn:
                    break;
                case EnumEventType.ContactOff:
                    break;
                case EnumEventType.Connection:
                    break;
                case EnumEventType.Action:
                    break;
                case EnumEventType.WindyMode:
                    break;
                default:
                    break;
            }
            return eventViewModel;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        private MetaEventViewModel _fromEvent;
        public MetaEventViewModel FromEvent
        {
            get { return _fromEvent; }
            set
            {
                _fromEvent = value;
                NotifyOfPropertyChange(() => FromEvent);
            }
        }

        public string Content
        {
            get { return _model.Content; }
            set
            {
                _model.Content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }

        public string User
        {
            get { return _model.User; }
            set
            {
                _model.User = value;
                NotifyOfPropertyChange(() => User);
            }
        }



        #endregion
        #region - Attributes -
        #endregion
    }
}
