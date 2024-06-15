using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Communications
{
    public interface IBaseMessageModel
    {
        EnumCmdType Command { get; set; }
    }
}