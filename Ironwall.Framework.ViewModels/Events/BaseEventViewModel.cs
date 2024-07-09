using Caliburn.Micro;
using Ironwall.Framework.Models.Events;
using System;

namespace Ironwall.Framework.ViewModels.Events
{
    public abstract class BaseEventViewModel
        : Screen, IBaseEventViewModel
    {
        #region - Ctors -
        public BaseEventViewModel()
        {

        }
        public BaseEventViewModel(IBaseEventModel model)
        {
            Id = model.Id;
            DateTime = model.DateTime;
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

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                NotifyOfPropertyChange(() => DateTime);
            }
        }

        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        #endregion
    }
}