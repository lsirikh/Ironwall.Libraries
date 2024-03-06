using System;
using System.Globalization;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/5/2023 1:36:21 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ShapeSizeConverter : IValueConverter
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter.ToString() == "Width")
            {
                return (double)value * 0.9; 

            }else if(parameter.ToString() == "Height")
            {
                return (double)value * 0.7;
            }
            else
            {
                return (double)value * 0.9; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
