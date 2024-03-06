using Ironwall.Libraries.Enums;
using System.Windows.Forms;

namespace Ironwall.Libraries.Tcp.Common.Models
{
    public interface ITcpDataModel
    {
        EnumTcpDataType EnumTcpDataType { get; set; }
        EnumTcpStateType EnumTcpStateType { get; set; }
        byte[] MetaData { get; set; }
        byte[] Binary { get; set; }
        
    }
}