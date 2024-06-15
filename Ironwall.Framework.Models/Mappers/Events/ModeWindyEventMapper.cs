using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class ModeWindyEventMapper
        : EventMapperBase, IModeWindyEventMapper
    {
        public ModeWindyEventMapper()
        {

        }

        public ModeWindyEventMapper(IModeWindyEventModel model)
            : base(model)
        {
            ModeWindy = model.ModeWindy;
        }

        public ModeWindyEventMapper(IModeWindyRequestModel model)
        {
            ModeWindy = model.ModeWindy;
        }

        public int ModeWindy { get; set; }
    }
}
