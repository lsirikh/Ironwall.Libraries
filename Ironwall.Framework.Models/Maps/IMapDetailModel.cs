using Newtonsoft.Json;

using System.Collections.Generic;

namespace Ironwall.Framework.Models.Maps
{
    public interface IMapDetailModel : IUpdateDetailBaseModel
    {
        List<MapModel> Maps { get; set; }
    }
}