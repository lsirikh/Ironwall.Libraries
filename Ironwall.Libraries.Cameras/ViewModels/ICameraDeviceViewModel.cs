using Ironwall.Libraries.Cameras.Models;
using System.Threading;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public interface ICameraDeviceViewModel : 
        ICameraDeviceModel
        , ICameraBaseViewModel
    {
        ICameraDeviceModel Model { get; set; }
        
    }
}