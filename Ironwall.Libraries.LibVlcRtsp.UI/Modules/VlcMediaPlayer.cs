using LibVLCSharp.Shared;

namespace Ironwall.Libraries.LibVlcRtsp.UI.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/5/2023 8:35:09 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class VlcMediaPlayer : MediaPlayer
    {

        #region - Ctors -
        public VlcMediaPlayer(LibVLC libVLC) : base(libVLC)
        {
            LibVLC = libVLC;
        }

        public VlcMediaPlayer(Media media) : base(media)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public LibVLC LibVLC { get; }
        #endregion
        #region - Attributes -
        #endregion

    }
}
