using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class MalfunctionDetailModel
        : IMalfunctionDetailModel
    {

        public MalfunctionDetailModel()
        {

        }

        public MalfunctionDetailModel(int reason, int fStart, int fEnd, int SStart, int SEnd)
        {
            Reason = reason;
            FirstStart = fStart;
            FirstEnd = fEnd;
            SecondStart = SStart;
            SecondEnd = SEnd;
        }

        [JsonProperty("reason", Order = 1)]
        public int Reason { get; set; }
        [JsonProperty("first_start", Order = 2)]
        public int FirstStart { get; set; }
        [JsonProperty("first_end", Order = 3)]
        public int FirstEnd { get; set; }
        [JsonProperty("second_start", Order = 4)]
        public int SecondStart { get; set; }
        [JsonProperty("second_end", Order = 5)]
        public int SecondEnd { get; set; }

        public void Insert(int reason, int fStart, int fEnd, int sStart, int sEnd)
        {
            Reason = reason;
            FirstStart = fStart;
            FirstEnd = fEnd;
            SecondStart = sStart;
            SecondEnd = sEnd;
        }
    }
}
