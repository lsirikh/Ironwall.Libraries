using Ironwall.Framework.Models.Events;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchDetectionResponseModel : IResponseModel
    {
        List<DetectionRequestModel> Events { get; set; }
    }
}