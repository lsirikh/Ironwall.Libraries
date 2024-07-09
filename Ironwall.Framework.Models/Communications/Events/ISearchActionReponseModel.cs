﻿using Ironwall.Framework.Models.Events;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchActionReponseModel : IResponseModel
    {
        List<ActionEventModel> Body { get; set; }
        //List<DetectionEventModel> Detections { get; set; }
        //List<MalfunctionEventModel> Malfunctions { get; set; }
    }
}