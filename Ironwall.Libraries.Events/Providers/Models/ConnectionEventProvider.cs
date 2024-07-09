using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Events.Providers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public sealed class ConnectionEventProvider : WrapperEventProvider<ConnectionEventModel>
    {
        #region - Ctors -
        public ConnectionEventProvider(EventProvider provider) : base(provider)
        {
            ClassName = nameof(ConnectionEventProvider);
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
