using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Globalization;
using System.Windows.Data;
using System;

namespace Ironwall.Libraries.Map.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/28/2023 9:21:12 AM                                                    
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
            if (value is SymbolViewModel symbolViewModel)
            {
                return symbolViewModel;
            }
            else if (value is ShapeSymbolViewModel shapeViewModel)
            {
                return shapeViewModel;
            }
            else if (value is ObjectShapeViewModel objectViewModel)
            {
                return objectViewModel;
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
