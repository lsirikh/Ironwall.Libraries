using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ActionMalfunctionEventModel
        : ActionEventModel
    {
        public ActionMalfunctionEventModel()
        {

        }

        /*public ActionMalfunctionEventModel(IActionMalfunctionEventMapper model, IBaseEventModel fromEvent)
            : base(model, fromEvent)
        {
            Reason = model.Reason;
            FirstStart = model.FirstStart;
            FirstEnd = model.FirstEnd;
            SecondStart = model.SecondStart;
            SecondEnd = model.SecondEnd;
        }*/

        public int Reason { get; set; }
        public int FirstStart { get; set; }
        public int FirstEnd { get; set; }
        public int SecondStart { get; set; }
        public int SecondEnd { get; set; }

       
    }
}
