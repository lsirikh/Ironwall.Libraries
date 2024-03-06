using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public abstract class EventMapperBase : IEventMapperBase
    {

        public EventMapperBase()
        {
            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public EventMapperBase(IBaseEventModel model)
        {
            EventId = model.Id != null ? model.Id : IdCodeGenerator.GenIdCode();
            DateTime = model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public EventMapperBase(IBaseEventMessageModel model)
        {
            EventId = model.Id;
            DateTime = model.DateTime;
        }

        public string EventId { get; set; }
        
        public string DateTime { get; set; }
    }
}
