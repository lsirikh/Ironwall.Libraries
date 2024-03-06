using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.AxisAudio.Common.Enums;

namespace Wpf.AxisAudio.Client.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/1/2023 10:19:27 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AxisAudioEnumValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int mode)) return null;

            switch ((EnumAxisAudioDeviceState)mode)
            {
                case EnumAxisAudioDeviceState.NONE:
                    return EnumAxisAudioDeviceState.NONE.ToString();
                case EnumAxisAudioDeviceState.ACTIVATED:
                    return EnumAxisAudioDeviceState.ACTIVATED.ToString();
                case EnumAxisAudioDeviceState.MIC_STREAMING:
                    return EnumAxisAudioDeviceState.MIC_STREAMING.ToString();
                case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING:
                    return EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING.ToString();
                case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING:
                    return EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
