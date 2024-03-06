using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/31/2023 10:20:35 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class GroupValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string param = (string)parameter;
            string deviceName = values[2].ToString();
            if (values[0] is bool isStreaming && values[1] is bool isGroup)
            {
                var ret = (isStreaming == true) && (isGroup == true);
                Debug.WriteLine($"[{param}][{deviceName}]Value[0] = {isStreaming}, Value[1] = {isGroup}, Result = {ret}");
                return ret;
            }
            return false; // 또는 기본값
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
