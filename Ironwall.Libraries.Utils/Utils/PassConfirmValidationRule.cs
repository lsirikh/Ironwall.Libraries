using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.Libraries.Utils
{
    public class PassConfirmValidationRule : ValidationRule
    {
        private PassConfirmValidationParameters _parameters = new PassConfirmValidationParameters();
        public PassConfirmValidationParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        //public PassConfirmValidationParameters Parameters { get; set; }



        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string password = value as string;

            if (!string.IsNullOrEmpty(password)) 
            {
                if(password.Length >= 8)
                {
                    if (password.Equals(Parameters.BasicString))
                    {
                        return new ValidationResult(true, "");
                    }
                    else
                    {
                        switch (cultureInfo.Name)
                        {
                            case LanguageConst.ENGLISH:
                                return new ValidationResult(false, "It is not the same as the entered password.");
                            case LanguageConst.KOREAN:
                                return new ValidationResult(false, "입력된 비밀번호와 동일하지 않습니다.");
                            default:
                                return new ValidationResult(false, "It is not the same as the entered password.");
                        }
                        
                    }
                }
                else
                {
                    switch (cultureInfo.Name)
                    {
                        case LanguageConst.ENGLISH:
                            return new ValidationResult(false, "Please enter a password of at least 8 characters.");
                        case LanguageConst.KOREAN:
                            return new ValidationResult(false, "비밀번호를 8자 이상 입력해주세요.");
                        default:
                            return new ValidationResult(false, "Please enter a password of at least 8 characters.");
                    }
                    
                }
            }
            else
            {
                return new ValidationResult(true, "");
            }
        }
    }

    public class PassConfirmValidationParameters : DependencyObject
    {

        // Using a DependencyProperty as the backing store for Criteria.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BasicStringProperty =
            DependencyProperty.Register("BasicString", typeof(string), typeof(PassConfirmValidationParameters), new PropertyMetadata(default(string)));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

        }

        public string BasicString
        {
            get { return (string)GetValue(BasicStringProperty); }
            set { SetValue(BasicStringProperty, value); }
        }

    }

    /*public class ZeroTo255MinMax : ValidationRule
    {
        private Validation1Parameters _parameters = new Validation1Parameters();
        public Validation1Parameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string numberStr = value as string;
            int val;

            if (int.TryParse(numberStr, out val))
            {
                if (val < 0 || val > 255)
                    return new ValidationResult(false, "");
                else if (Parameters.ValTypeFor0to255 == Validation1Parameters.ValTypes.ShouldBeBigger)
                {
                    if (val <= Parameters.NumberCombineTo || val - Parameters.NumberCombineTo < 2)
                        return new ValidationResult(false, "");
                }
                else if (Parameters.ValTypeFor0to255 == Validation1Parameters.ValTypes.ShouldBeSmaller)
                {
                    if (val >= Parameters.NumberCombineTo || Parameters.NumberCombineTo - val < 2)
                        return new ValidationResult(false, "");
                }
                return new ValidationResult(true, "");
            }
            else
                return new ValidationResult(false, "");
        }
    }

    public class Validation1Parameters : DependencyObject
    {
        public enum ValTypes { ShouldBeSmaller, ShouldBeBigger };
        public static readonly DependencyProperty NumberCombineToProperty = DependencyProperty.Register("NumberCombineTo", typeof(int), typeof(Validation1Parameters), new PropertyMetadata(0, new PropertyChangedCallback(OnNumberCombineToChanged)));
        public static readonly DependencyProperty ValTypeFor0to255Property = DependencyProperty.Register("ValTypeFor0to255", typeof(ValTypes), typeof(Validation1Parameters), new PropertyMetadata(ValTypes.ShouldBeBigger));

        private static void OnNumberCombineToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { d.CoerceValue(NumberCombineToProperty); }

        public int NumberCombineTo
        {
            get { return (int)GetValue(NumberCombineToProperty); }
            set { SetValue(NumberCombineToProperty, value); }
        }

        public ValTypes ValTypeFor0to255
        {
            get { return (ValTypes)GetValue(ValTypeFor0to255Property); }
            set { SetValue(ValTypeFor0to255Property, value); }
        }
    }*/
}
