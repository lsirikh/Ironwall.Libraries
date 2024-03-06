using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Ironwall.Libraries.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/7/2023 11:17:37 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class StringToColorConverter : IValueConverter
    {

        #region - Ctors -

        #endregion
        #region - Implementation of Interface -
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString)
            {
                try
                {
                    return (Color)ColorConverter.ConvertFromString(colorString);
                }
                catch(Exception e)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(StringToColorConverter)} for {e.Message}");
                    
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                Debug.WriteLine($"Color Converter is not string type.");
                return (Color)ColorConverter.ConvertFromString("#FFFF0000");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return color.ToString();
            }
            return DependencyProperty.UnsetValue;
        }
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
        #endregion
        #region - Attributes -
        #endregion

    }
}
