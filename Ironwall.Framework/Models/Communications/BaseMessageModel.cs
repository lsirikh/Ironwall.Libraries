using Newtonsoft.Json;

namespace Ironwall.Framework.Models.Communications
{
    public class BaseMessageModel : IBaseMessageModel
    {
        public BaseMessageModel()
        {

        }

        public BaseMessageModel(int cmd)
        {
            Command = cmd;
        }

        public BaseMessageModel(IBaseMessageModel model)
        {
            Command = model.Command;
        }
        
        [JsonProperty("command", Order = 0)]
        public int Command { get; set; }



    }
}