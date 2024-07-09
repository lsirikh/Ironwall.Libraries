using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class MalfunctionEventViewModel : MetaEventViewModel, IMalfunctionEventViewModel
    {
        #region - Ctors -
        public MalfunctionEventViewModel()
        {
            _model = new MalfunctionEventModel();
        }
        public MalfunctionEventViewModel(IMalfunctionEventModel model) : base(model)
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
        public EnumFaultType Reason
        {
            get { return (_model as IMalfunctionEventModel).Reason; }
            set
            {
                (_model as IMalfunctionEventModel).Reason = value;
                NotifyOfPropertyChange(() => Reason);
            }
        }

        public int FirstStart
        {
            get { return (_model as IMalfunctionEventModel).FirstStart; }
            set
            {
                (_model as IMalfunctionEventModel).FirstStart = value;
                NotifyOfPropertyChange(() => FirstStart);
            }
        }

        public int FirstEnd
        {
            get { return (_model as IMalfunctionEventModel).FirstEnd; }
            set
            {
                (_model as IMalfunctionEventModel).FirstEnd = value;
                NotifyOfPropertyChange(() => FirstEnd);
            }
        }

        public int SecondStart
        {
            get { return (_model as IMalfunctionEventModel).SecondStart; }
            set
            {
                (_model as IMalfunctionEventModel).SecondStart = value;
                NotifyOfPropertyChange(() => SecondStart);
            }
        }

        public int SecondEnd
        {
            get { return (_model as IMalfunctionEventModel).SecondEnd; }
            set
            {
                (_model as IMalfunctionEventModel).SecondEnd = value;
                NotifyOfPropertyChange(() => SecondEnd);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
