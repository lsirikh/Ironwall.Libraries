using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class ConnectionEventMapper
        : MetaEventMapper
        , IConnectionEventMapper
    {

        public ConnectionEventMapper()
        {

        }

        public ConnectionEventMapper(IConnectionEventModel model) : base(model)
        {

        }
    
    }
}
