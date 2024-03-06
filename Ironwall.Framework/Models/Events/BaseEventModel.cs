
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.ViewModels.Events;
using System;

namespace Ironwall.Framework.Models.Events
{
    public abstract class BaseEventModel
        : IBaseEventModel
    {
        public BaseEventModel()
        {
            DateTime = DateTime.Now;
        }

        public BaseEventModel(IEventMapperBase model)
        {
            Id = model.EventId;
            DateTime = DateTime.Parse(model.DateTime);
        }

        public BaseEventModel(IBaseEventMessageModel model)
        {
            Id = model.Id;
            DateTime = DateTime.Parse(model.DateTime);
        }

        public BaseEventModel(IBaseEventViewModel model)
        {
            Id = model.Id;
            DateTime = model.DateTime;
        }

        public string Id { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}
