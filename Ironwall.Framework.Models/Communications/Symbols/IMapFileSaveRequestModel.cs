using Ironwall.Framework.Models.Maps;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public interface IMapFileSaveRequestModel : IUserSessionBaseRequestModel
    {
        List<MapModel> Maps { get; set; }
    }
}