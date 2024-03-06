using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Events
{
    public class ModeWindyEventViewModel
        : BaseEventViewModel, IModeWindyEventViewModel
    {
        #region - Ctors -
        public ModeWindyEventViewModel()
        {

        }

        public ModeWindyEventViewModel(IModeWindyEventModel model)
            : base(model)
        {
            ModeWindy = model.ModeWindy;
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
        private int _modeWindy;

        public int ModeWindy
        {
            get { return _modeWindy; }
            set
            {
                _modeWindy = value;
                NotifyOfPropertyChange(() => ModeWindy);
            }
        }

        #endregion
        #region - Attributes -
        #endregion
    }
}
