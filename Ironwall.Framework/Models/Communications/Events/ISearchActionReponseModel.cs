using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchActionReponseModel : IResponseModel
    {
        List<DetectionRequestModel> DetectionEvents { get; set; }
        List<MalfunctionRequestModel> MalfunctionEvents { get; set; }
        List<ActionRequestModel> ActionEvents { get; set; }
    }
}