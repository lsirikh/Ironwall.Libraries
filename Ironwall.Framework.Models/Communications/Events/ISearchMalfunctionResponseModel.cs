using Ironwall.Framework.Models.Events;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchMalfunctionResponseModel : IResponseModel
    {
        List<MalfunctionEventModel> Body { get; set; }
    }
}