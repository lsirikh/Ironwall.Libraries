using Ironwall.Framework.ViewModels;

namespace Ironwall.Framework.Models.Devices
{
    public interface IOptionBaseModel : IBaseModel
    {
        string ReferenceId { get; set; }
    }
}