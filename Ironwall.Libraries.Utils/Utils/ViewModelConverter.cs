using System;
using System.Globalization;
using System.Windows.Data;
using Wpf.AdornerProject.Sample.ViewModels.Elements;

namespace Wpf.AdornerProject.Sample.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/12/2023 10:33:47 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ViewModelConverter : IValueConverter
    {

        #region - Ctors -

        #endregion
        #region - Implementation of Interface -
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is SymbolViewModel symbolViewModel)
            {
                return symbolViewModel;
            }
            else if(value is ShapeViewModel shapeViewModel)
            {
                return shapeViewModel;
            }
            else
            {
                return null;
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
