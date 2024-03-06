using Ironwall.Framework.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ActionDetectionEventModel
        : ActionEventModel
    {
        public ActionDetectionEventModel()
        {

        }

        /*public ActionDetectionEventModel(IActionDetectionEventMapper model, IBaseEventModel fromEvent) : base(model, fromEvent)
        {
            Result = model.Result;
        }*/

        public int Result { get; set; }
    }
}
