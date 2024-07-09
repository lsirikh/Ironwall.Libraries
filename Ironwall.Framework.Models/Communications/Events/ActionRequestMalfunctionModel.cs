using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ActionRequestMalfunctionModel
        : ActionBaseRequestModel<MalfunctionEventModel>, IActionRequestMalfunctionModel
    {
        public ActionRequestMalfunctionModel()
        {
            Command = EnumCmdType.EVENT_ACTION_MALFUNCTION_REQUEST;
        }

        public ActionRequestMalfunctionModel(string content, string user, IMalfunctionEventModel model)
            : base(EnumCmdType.EVENT_ACTION_MALFUNCTION_REQUEST, content, user, model as MalfunctionEventModel)
        {
        }

        public ActionRequestMalfunctionModel(IActionEventModel model) : base(EnumCmdType.EVENT_ACTION_MALFUNCTION_REQUEST, model)
        {
            
        }
    }
}
