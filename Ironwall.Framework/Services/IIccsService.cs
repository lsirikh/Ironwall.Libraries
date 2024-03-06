using Ironwall.Libraries.Base.Services;
using Newtonsoft.Json.Linq;

namespace Ironwall.Framework.Services
{
    public interface IIccsService 
        : IService
    {
        void BuildLookupTabel();
        void ProcessDetection(JToken target);
        void ProcessFault(JToken target);
        void ProcessConnection(JToken target);
    }
}
