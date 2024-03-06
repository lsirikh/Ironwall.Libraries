
using Ironwall.Framework.Models.Events;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class ExEventViewModel : EventViewModel<IActionEventModel>, IExEventViewModel
    {
        #region - Ctors -
        public ExEventViewModel(IActionEventModel eventModel)
            : base(eventModel)
        {
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
        public string Content
        {
            get => _model.Content;
            set
            {
                _model.Content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }

        public string User
        {
            get => _model.User;
            set
            {
                _model.User = value;
                NotifyOfPropertyChange(() => User);
            }
        }

        public IMetaEventModel FromEventModel
        {
            get => _model.FromEvent;
            set
            {
                _model.FromEvent = value;
                NotifyOfPropertyChange(() => FromEventModel);
            }
        }
        public int Status
        {
            get => status;
            set
            {
                status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }
        #endregion
        #region - Attributes -
        protected int status;
        #endregion
    }
}