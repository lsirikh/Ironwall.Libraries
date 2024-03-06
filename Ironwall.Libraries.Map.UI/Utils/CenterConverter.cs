using System;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Windows.Data;

namespace Ironwall.Libraries.Map.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/12/2023 9:11:50 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CenterConverter : IValueConverter
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double coordinate)) return 0;

            if (parameter == null)
            {
                return coordinate - 5; // Rectangle의 너비나 높이의 절반 (여기서는 10/2 = 5)
            }
            else
            {
                var param = 0d;
                double.TryParse(parameter.ToString(), out param);

                return coordinate - (param/2);
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
