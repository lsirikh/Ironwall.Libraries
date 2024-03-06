namespace Ironwall.Libraries.Tcp.Packets.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/18/2023 10:17:34 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public enum EnumPacketType : byte
    {

        NONE = 0x00,
        SHORT_MESSAGE = 0x01,
        LONG_MESSAGE = 0x02,
        FILE = 0x03,
        MAP = 0x04,
        PROFILE = 0x05,
        VIDEO = 0x06,

    }
}
