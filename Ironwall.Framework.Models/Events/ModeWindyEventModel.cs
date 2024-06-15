using Ironwall.Framework.Models.Communications.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ModeWindyEventModel
        : BaseEventModel
        , IModeWindyEventModel
    {
        public ModeWindyEventModel()
        {

        }

        public ModeWindyEventModel(IModeWindyRequestModel model)
        {
            ModeWindy = model.ModeWindy;
        }



        public int ModeWindy { get; set; }
    }
}
