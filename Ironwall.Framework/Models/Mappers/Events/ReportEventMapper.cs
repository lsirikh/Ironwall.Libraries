using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class ReportEventMapper
        : EventMapperBase, IReportEventMapper
    {
        public ReportEventMapper()
        {

        }
        public ReportEventMapper(IActionEventModel model) : base(model)
        {
            FromEventId = model.FromEvent.Id;
            Content = model.Content;
            User = model.User;
        }

        public string FromEventId { get; set; }
        public string Content { get; set; }
        public string User { get; set; }
    }
}
