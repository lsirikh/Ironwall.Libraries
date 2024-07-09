using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dialogs
{
    public class PreEventFaultDetailViewModel :Screen
    {
        #region - Ctors -
        public PreEventFaultDetailViewModel(IEventAggregator eventAggregator)
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
        public int Id => _eventViewModel.Id;

        public DateTime DateTime => _eventViewModel.DateTime;

        public string EventGroup => _eventViewModel.EventGroup;

        public EnumFaultType Reason => MalfunctionEventModel.Reason;

        public int FirstStart => MalfunctionEventModel.FirstStart;
        
        public int FirstEnd => MalfunctionEventModel.FirstEnd;

        public int SecondStart => MalfunctionEventModel.SecondStart;

        public int SecondEnd => MalfunctionEventModel.SecondEnd;

        public EnumTrueFalse Status => _eventViewModel.Status;

        public string TagFault => EventViewModel.TagFault;

        public IBaseDeviceModel DeviceModel => MalfunctionEventModel.Device;

        public IMalfunctionEventModel MalfunctionEventModel => _eventViewModel.EventModel as IMalfunctionEventModel;

        public PreEventViewModel EventViewModel
        {
            get => _eventViewModel;
            set
            {
                _eventViewModel = value;
                NotifyOfPropertyChange(() => EventViewModel);
            }
        }

        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        private PreEventViewModel _eventViewModel;
        #endregion
    }
}
