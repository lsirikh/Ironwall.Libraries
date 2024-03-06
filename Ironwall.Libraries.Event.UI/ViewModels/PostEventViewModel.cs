using Caliburn.Micro;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public class PostEventViewModel : ExEventViewModel, IPostEventViewModel
    {
        #region - Ctors -
        public PostEventViewModel(IActionEventModel eventModel)
            : base(eventModel)
        {
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
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IEventAggregator EventAggregator { get; set; }

        #endregion
        #region - Attributes -
        #endregion

    }
}
