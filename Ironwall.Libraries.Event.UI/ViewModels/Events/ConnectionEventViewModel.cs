using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class ConnectionEventViewModel
        : MetaEventViewModel
        , IConnectionEventViewModel
    {
        #region - Ctors -
        public ConnectionEventViewModel()
        {

        }

        public ConnectionEventViewModel(IConnectionEventModel model)
            : base(model)
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
