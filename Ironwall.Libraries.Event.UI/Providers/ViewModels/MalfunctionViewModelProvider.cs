using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public sealed class MalfunctionViewModelProvider : WrapperEventViewModelProvider<IMalfunctionEventModel, MalfunctionEventViewModel>
    {

        #region - Ctors -
        public MalfunctionViewModelProvider(EventProvider provider) : base(provider)
        {
            ClassName = nameof(MalfunctionViewModelProvider);
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
