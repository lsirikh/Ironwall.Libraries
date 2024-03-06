using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ironwall.Libraries.Utils
{
    public class EnumDataTypeToTextConverter
    : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values == null || values[0] == null || values[1] == null)
                return null;

            if (!(values.Count() > 0))
                return null;

            if (targetType.Name != "String")
                return null;

            try
            {
                var type = values[1].ToString();

                switch (type)
                {
                    case "Map":
                        return $"[{values[0].ToString()}]{EnumDataType.Map.ToString()}";
                    case "CONTROLLER":
                        return $"[{values[0].ToString()}]{EnumDataType.Controller.ToString()}";
                    case "Sensor":
                        return $"[{values[0].ToString()}]{EnumDataType.Sensor.ToString()}";
                    //그룹 라인
                    case "GroupSymbol":
                        return $"[{values[0].ToString()}]{EnumDataType.GroupSymbol.ToString()}";
                    //그룹 라벨
                    case "Group":
                        return $"[{values[0].ToString()}]{EnumDataType.Group.ToString()}";
                    //카메라 이미지
                    case "Camera":
                        return $"[{values[0].ToString()}]{EnumDataType.Camera.ToString()}";
                    //카메라 라벨
                    case "CameraLabel":
                        return $"[{values[0].ToString()}]{EnumDataType.CameraLabel.ToString()}";
                    default:
                        return values[0].ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
