using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/5/2023 1:47:09 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ShapeSizeWithLableConverter : IMultiValueConverter
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        //public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        //{
        //    double shapeSize = 0d;

        //    if (parameter.ToString() == "Width")
        //    {
        //        double totalSize = (double)values[0];

        //        shapeSize = totalSize  - MARGIN;
        //        Debug.WriteLine($"Width : {shapeSize}({totalSize} - {MARGIN})");
        //        return shapeSize;
        //    }
        //    else if(parameter.ToString() == "Height")
        //    {
        //        double totalSize = (double)values[0];
        //        double elementSize = (double)values[1];

        //        shapeSize = totalSize - elementSize - MARGIN;
        //        Debug.WriteLine($"Height : {shapeSize}({totalSize} - {elementSize} - {MARGIN})");
        //        return shapeSize;
        //    }
        //    else
        //    {
        //        return shapeSize;
        //    }
        //}
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double actualHeight = 0d;
            double shapeSize = 0d;
            if (values.Length == 1 )
            {
                if (values[0] == DependencyProperty.UnsetValue)
                    return Binding.DoNothing;

                if (parameter.ToString() == "Width")
                {
                    double totalSize = (double)values[0];

                    shapeSize = totalSize - MARGIN;
                    //Debug.WriteLine($"Width : {shapeSize}({totalSize} - {MARGIN})");
                    return shapeSize;
                }
                else
                {
                    return Binding.DoNothing;
                }
            }
            else if(values.Length == 2)
            {
                if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                    return Binding.DoNothing;

                double originalSize = (double)values[0];
                DependencyObject grid = values[1] as DependencyObject;
                Viewbox viewBox = null;

                if (grid != null)
                {
                    viewBox = VisualHelper.FindChild<Viewbox>(grid, "LableName");
                }

                if (viewBox == null)
                    return originalSize;

                actualHeight = viewBox.ActualHeight;

                if (parameter.ToString() == "Height")
                {
                    double totalSize = (double)values[0];

                    shapeSize = totalSize - actualHeight - MARGIN;
                    //Debug.WriteLine($"Height : {shapeSize}({totalSize} - {actualHeight} - {MARGIN})");
                    return shapeSize;
                }
                else
                {
                    return Binding.DoNothing;
                }
            }
            else
            {
                return Binding.DoNothing;
            }
            

           

            // 여기서 높이 계산 로직을 적용하세요.
            
            //if (parameter.ToString() == "Width")
            //{
            //    double totalSize = (double)values[0];

            //    shapeSize = totalSize - MARGIN;
            //    Debug.WriteLine($"Width : {shapeSize}({totalSize} - {MARGIN})");
            //    return shapeSize;
            //}
            //else if (parameter.ToString() == "Height")
            //{
            //    double totalSize = (double)values[0];

            //    shapeSize = totalSize - actualHeight - MARGIN;
            //    Debug.WriteLine($"Height : {shapeSize}({totalSize} - {actualHeight} - {MARGIN})");
            //    return shapeSize;
            //}
            //else
            //{
            //    return shapeSize;
            //}
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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
        private const double MARGIN = 10d;
        #endregion

    }
}
