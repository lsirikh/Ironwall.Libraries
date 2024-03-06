using System.Text;
using System;

namespace Ironwall.Libraries.Tcp.Packets.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/19/2023 8:44:29 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PacketHeader
    {

        #region - Ctors -
        public PacketHeader()
        {

        }

        public PacketHeader(byte[] bytes)
        {
            Id = BitConverter.ToUInt16(bytes, 0);                               //0, 1
            CurrentSequence = BitConverter.ToUInt16(bytes, 2);                  //2, 3
            TotalSequence = BitConverter.ToUInt16(bytes, 4);                    //4, 5
            DataType = bytes[6];                                                //6

            Buffer.BlockCopy(bytes, PREHEADER_SIZE, _fileName, 0, FILE_NAME_SIZE);
            FileName = Encoding.UTF8.GetString(_fileName).Trim('\0');

            Buffer.BlockCopy(bytes, PREHEADER_SIZE + FILE_NAME_SIZE, _fileExtension, 0, FILE_EXTENSION_SIZE);
            FileExtension = Encoding.UTF8.GetString(_fileExtension).Trim('\0');

            BodyLength = BitConverter.ToUInt16(bytes, 93);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        protected byte[] CreateHeader(byte[] bArray)
        {
            BitConverter.GetBytes(Id).CopyTo(bArray, 0);
            BitConverter.GetBytes(CurrentSequence).CopyTo(bArray, 2);
            BitConverter.GetBytes(TotalSequence).CopyTo(bArray, 4);
            bArray[6] = DataType;

            var fileName = Encoding.UTF8.GetBytes(FileName);
            if (fileName.Length <= FILE_NAME_SIZE)
                Buffer.BlockCopy(fileName, 0, bArray, PREHEADER_SIZE, fileName.Length);

            var fileExtension = Encoding.ASCII.GetBytes(FileExtension);
            if (fileExtension.Length <= FILE_EXTENSION_SIZE)
                Buffer.BlockCopy(fileExtension, 0, bArray, PREHEADER_SIZE + FILE_NAME_SIZE, fileExtension.Length);

            BitConverter.GetBytes(BodyLength).CopyTo(bArray, 93);

            return bArray;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //Header Side
        public ushort Id { get; set; } // 2byte
        public ushort CurrentSequence { get; set; } // 2byte
        public ushort TotalSequence { get; set; } // 2byte
        public byte DataType { get; set; } // 1byte -  0 : NULL, 1 : Short Message, 2 : Long Message, 3 : File, 4 : Image ...
        public string FileName { get; set; } = "";// 80byte
        public string FileExtension { get; set; } = ""; // 6byte
        public ushort BodyLength { get; set; }  // 2byte
        #endregion
        #region - Attributes -
        public const int TOTAL_SIZE = 1024*8;
        //////////////////////PREHEADER////////////////////
        public const int ID_SIZE = 2; // ushort
        public const int CURRENT_SQUENCE_SIZE = 2; // ushort
        public const int TOTAL_SQUENCE_SIZE = 2; // ushort
        public const int DATA_TYPE_SIZE = 1; // byte

        public const int PREHEADER_SIZE = ID_SIZE + CURRENT_SQUENCE_SIZE + TOTAL_SQUENCE_SIZE + DATA_TYPE_SIZE;
        ///////////////////////////////////////////////////

        //////////////////////FILEHEADER////////////////////
        public const int FILE_NAME_SIZE = 80;
        public const int FILE_EXTENSION_SIZE = 6;
        public const int BODY_LENGTH_SIZE = 2; // ushort

        public const int FILEHEADER_SIZE = FILE_NAME_SIZE + FILE_EXTENSION_SIZE + BODY_LENGTH_SIZE;
        ///////////////////////////////////////////////////

        public const int HEADER_SIZE = PREHEADER_SIZE + FILEHEADER_SIZE;

        public const int BODY_SIZE = TOTAL_SIZE - HEADER_SIZE - CRC_SIZE;

        public const int CRC_SIZE = 2;

        public const long MAXIMUM_TRANSFER_FILE_SIZE = 250000000;

        protected byte[] _fileName = new byte[FILE_NAME_SIZE];
        protected byte[] _fileExtension = new byte[FILE_EXTENSION_SIZE];


        #endregion
    }
}
