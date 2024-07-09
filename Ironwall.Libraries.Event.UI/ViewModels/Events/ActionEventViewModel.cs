using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class ActionEventViewModel : BaseEventViewModel<IActionEventModel>, IActionEventViewModel
    {
        #region - Ctors -
        public ActionEventViewModel()
        {
            _model = new ActionEventModel();
        }

        public ActionEventViewModel(IActionEventModel model) : base(model)
        {
            FromEvent = EventBuilder(model.FromEvent);

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
        private MetaEventViewModel EventBuilder(IMetaEventModel model)
        {
            try
            {
                if (model == null) return null;

                MetaEventViewModel eventViewModel = null;
                switch (model.MessageType)
                {
                    case EnumEventType.Intrusion:
                        {
                            eventViewModel = new DetectionEventViewModel(model as IDetectionEventModel);
                        }
                        break;
                    case EnumEventType.Fault:
                        {
                            eventViewModel = new MalfunctionEventViewModel(model as IMalfunctionEventModel);
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
            catch (Exception)
            {

                throw;
            }
        }
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
