using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public class EventProvider : BaseProvider<IMetaEventModel>
    {
        #region - Ctors -
        public EventProvider()
        {
            ClassName = nameof(DeviceProvider);
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
