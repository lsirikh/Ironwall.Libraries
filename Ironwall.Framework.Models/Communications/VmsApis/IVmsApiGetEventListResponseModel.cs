using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiGetEventListResponseModel : IResponseModel
    {
        List<Sensorway.Events.Base.Models.EventModel> Body { get; set; }
    }
}