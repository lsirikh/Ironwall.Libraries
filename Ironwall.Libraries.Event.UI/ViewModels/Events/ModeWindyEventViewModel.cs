using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class ModeWindyEventViewModel
        : BaseEventViewModel<IModeWindyEventModel>, IModeWindyEventViewModel
    {
        #region - Ctors -
        public ModeWindyEventViewModel()
        {
        }

        public ModeWindyEventViewModel(IModeWindyEventModel model)
            : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new ModeWindyEventModel();
            GC.Collect();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int ModeWindy
        {
            get { return _model.ModeWindy; }
            set
            {
                _model.ModeWindy = value;
                NotifyOfPropertyChange(() => ModeWindy);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
