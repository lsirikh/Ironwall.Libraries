using System.Windows.Controls;
using System.Windows;
using System;

namespace Ironwall.Libraries.Map.UI.UI.Units
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/20/2023 12:30:34 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CustomControl : ContentControl
    {

        #region - Ctors -
        static CustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomControl), new FrameworkPropertyMetadata(typeof(CustomControl)));
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
        public bool OnEditable
        {
            get { return (bool)GetValue(OnEditableProperty); }
            set { SetValue(OnEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnEditableProperty =
            DependencyProperty.Register("OnEditable", typeof(bool), typeof(ContentControl), new PropertyMetadata(false));


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CustomControl), new PropertyMetadata(false));


        public string Fill
        {
            get { return (string)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(string), typeof(CustomControl), new PropertyMetadata("#000000"));


        public bool IsShowLable
        {
            get { return (bool)GetValue(IsShowLableProperty); }
            set { SetValue(IsShowLableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowLable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowLableProperty =
            DependencyProperty.Register("IsShowLable", typeof(bool), typeof(CustomControl), new PropertyMetadata(false));


        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CustomControl), new PropertyMetadata(0d));

        public string Lable
        {
            get { return (string)GetValue(LableProperty); }
            set { SetValue(LableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LableProperty =
            DependencyProperty.Register("Lable", typeof(string), typeof(CustomControl), new PropertyMetadata(String.Empty));


        #endregion
        #region - Attributes -
        #endregion
    }
}
