using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Events.Providers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public sealed class DetectionEventProvider : WrapperEventProvider<DetectionEventModel>
    {
        #region - Ctors -
        public DetectionEventProvider(EventProvider provider) : base(provider)
        {
            ClassName = nameof(DetectionEventProvider);
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
    

