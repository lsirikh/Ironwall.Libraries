using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Events
{
    public class ActionEventModel
        : BaseEventModel, IActionEventModel
    {

        public ActionEventModel()
        {

        }

        public ActionEventModel(IReportEventMapper model, IMetaEventModel fromEvent) : base(model)
        {
            //Id = model.EventId != null ? model.EventId : IdCodeGenerator.GenIdCode();
            
            FromEvent = fromEvent;

            Content = model.Content;
            User = model.User;
        }

        public ActionEventModel(IActionRequestModel model, IMetaEventModel fromEvent) : base(model)
        {
            //Id = model.Id != null ? model.Id : IdCodeGenerator.GenIdCode();

            FromEvent = fromEvent;

            Content = model.Content;
            User = model.User;
        }

        //public ActionEventModel(IActionEventViewModel model) : base(model)
        //{
        //    //Id = model.Id != null ? model.Id : IdCodeGenerator.GenIdCode();

        //    Content = model.Content;
        //    User = model.User;
        //    FromEvent = ModelTypeHelper.GetEvent(model.FromEvent);
        //}

        

        public IMetaEventModel FromEvent { get; set; }
        public string Content { get; set; }
        public string User { get; set; }

    }
}
