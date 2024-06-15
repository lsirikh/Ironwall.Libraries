using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchMalfunctionResponseModel : IResponseModel
    {
        List<MalfunctionRequestModel> Events { get; set; }
    }
}