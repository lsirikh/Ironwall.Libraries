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
                        return $"[{values[0]}]{EnumDataType.Map}";
                    case "CONTROLLER":
                        return $"[{values[0]}]{EnumDataType.Controller}";
                    case "Sensor":
                        return $"[{values[0]}]{EnumDataType.Sensor}";
                    //그룹 라인
                    case "GroupSymbol":
                        return $"[{values[0]}]{EnumDataType.GroupSymbol}";
                    //그룹 라벨
                    case "Group":
                        return $"[{values[0]}]{EnumDataType.Group}";
                    //카메라 이미지
                    case "Camera":
                        return $"[{values[0]}]{EnumDataType.Camera}";
                    //카메라 라벨
                    case "CameraLabel":
                        return $"[{values[0]}]{EnumDataType.CameraLabel}";
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
