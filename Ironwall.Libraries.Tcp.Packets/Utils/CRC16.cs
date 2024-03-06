namespace Ironwall.Libraries.Tcp.Packets.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/18/2023 10:18:02 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CRC16
    {

        #region - Ctors -
        static CRC16()
        {
            for (ushort i = 0; i < Table.Length; ++i)
            {
                ushort value = i;
                for (byte temp = 0; temp < 8; ++temp)
                {
                    value = (ushort)((value & 1) != 0 ? (value >> 1) ^ Polynomial : (value >> 1));
                }
                Table[i] = value;
            }
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public static ushort ComputeChecksum(byte[] bytes, int start, int count)
        {
            ushort crc = 0xFFFF;
            for (int i = start; i < start + count; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ Table[index]);
            }
            return crc;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private const ushort Polynomial = 0xA001;
        private static readonly ushort[] Table = new ushort[256];
        #endregion
    }
}
