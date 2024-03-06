using System.Text;
using System;
using Ironwall.Libraries.Tcp.Packets.Utils;

namespace Ironwall.Libraries.Tcp.Packets.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/18/2023 10:15:38 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PacketClass : PacketHeader
    {

        #region - Ctors -
        public PacketClass()
        {

        }

        public PacketClass(byte[] bytes) : base(bytes)
        {
            Buffer.BlockCopy(bytes, HEADER_SIZE, Body, 0, BodyLength);

            CRC = BitConverter.ToUInt16(bytes, TOTAL_SIZE - CRC_SIZE);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void CreateCRC()
        {
            byte[] result = new byte[TOTAL_SIZE];

            byte[] headeredArray = CreateHeader(result);

            byte[] bodied = CreateBody(headeredArray);

            CRC = CRC16.ComputeChecksum(bodied, 0, TOTAL_SIZE - CRC_SIZE);
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[TOTAL_SIZE];

            byte[] headeredArray = CreateHeader(result);

            byte[] bodied = CreateBody(headeredArray);

            ushort crcValue = CRC16.ComputeChecksum(bodied, 0, TOTAL_SIZE - CRC_SIZE);
            BitConverter.GetBytes(crcValue).CopyTo(bodied, TOTAL_SIZE - CRC_SIZE);

            return result;
        }

        
        private byte[] CreateBody(byte[] bArray)
        {
            Buffer.BlockCopy(Body, 0, bArray, HEADER_SIZE, BodyLength);
            return bArray;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        
        //Body Side
        public byte[] Body { get; set; } = new byte[BODY_SIZE];
        //Crc Side
        public ushort CRC { get; set; } // 2byte
        #endregion
        #region - Attributes -
        
        #endregion
    }
}
