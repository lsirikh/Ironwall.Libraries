using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Events
{
    public class ActionEventViewModel
        : BaseEventViewModel, IActionEventViewModel
    {
        #region - Ctors -
        public ActionEventViewModel()
        {
        }

        public ActionEventViewModel(IActionEventModel model) : base(model)
        {
            try
            {

                switch ((EnumEventType)model.FromEvent.MessageType)
                {
                    case EnumEventType.Intrusion:
                        {
                            FromEvent = ViewModelFactory.Build<DetectionEventViewModel>(model.FromEvent);
                        }
                        break;
                    case EnumEventType.Fault:
                        {
                            FromEvent = ViewModelFactory.Build<MalfunctionEventViewModel>(model.FromEvent);
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
            }
            catch (Exception)
            {
                throw;
            }
            
            Content = model.Content;
            User = model.User;
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
        private IMetaEventViewModel _fromEvent;
        public IMetaEventViewModel FromEvent
        {
            get { return _fromEvent; }
            set
            {
                _fromEvent = value;
                NotifyOfPropertyChange(() => FromEvent);
            }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }

        private string _user;
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                NotifyOfPropertyChange(() => User);
            }
        }

        #endregion
        #region - Attributes -
        #endregion
    }
}
