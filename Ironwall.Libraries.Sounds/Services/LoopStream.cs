using NAudio.Wave;

namespace Ironwall.Libraries.Sounds.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/11/2023 4:09:31 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class LoopStream : WaveStream
    {

        #region - Ctors -

        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop by resetting the position to start
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;
        public override long Length => _sourceStream.Length;

        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }
        #endregion
        #region - Attributes -
        private readonly WaveStream _sourceStream;
        #endregion
    }
}
