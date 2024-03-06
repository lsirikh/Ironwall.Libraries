using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dialogs
{
    public class PostEventFaultDetailViewModel
        : Screen
    {
        #region - Ctors -
        public PostEventFaultDetailViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
        public string Id => FromEventModel.Id;

        public DateTime DateTime => FromEventModel.DateTime;

        public string EventGroup  => FromEventModel.EventGroup;

        public int Reason => FromEventModel.Reason;

        public int FirstStart => FromEventModel.FirstStart;

        public int FirstEnd => FromEventModel.FirstEnd;

        public int SecondStart => FromEventModel.SecondStart;

        public int SecondEnd => FromEventModel.SecondEnd;

        public int Status=> EventViewModel.Status;

        public string TagFault => EventViewModel.TagFault;

        public string User => EventViewModel.User;

        public string Content => EventViewModel.Content;

        public string ActionId => EventViewModel.Id;

        public DateTime ActionDateTime => EventViewModel.DateTime;

        public IBaseDeviceModel DeviceModel => FromEventModel.Device;
      
        public IMalfunctionEventModel FromEventModel => (_eventViewModel.EventModel as IActionEventModel).FromEvent as IMalfunctionEventModel;

        public PostEventViewModel EventViewModel
        {
            get => _eventViewModel;
            set
            {
                _eventViewModel = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        private PostEventViewModel _eventViewModel;
        #endregion
    }
}
