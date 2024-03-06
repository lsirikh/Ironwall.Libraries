namespace Wpf.AxisAudio.Common.Enums
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/25/2023 3:38:31 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public enum EnumAxisAudioDeviceState
    {

        NONE = 0,
        ACTIVATED = 1,
        MIC_STREAMING = 2,
        AUDIO_CLIP_PLAYING = 3,
        AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING = 4,
    }
}
