using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Map.Common.Helpers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/25/2023 1:15:05 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class SymbolHelper
    {

        /// <summary>
        /// GetCategory는 Symbol의 3개 Category로 분류하여 EnumSymbolCategory로 리턴을 해준다.
        /// </summary>
        /// <param name="num">(int)EnumShapeType</param>
        /// <returns>EnumSymbolCategory</returns>
        public static EnumSymbolCategory GetCategory(int num)
        {
            switch ((EnumShapeType)num)
            {
                case EnumShapeType.NONE:
                    return EnumSymbolCategory.None;
                case EnumShapeType.TEXT:
                    return EnumSymbolCategory.Symbol;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                case EnumShapeType.FENCE:
                    return EnumSymbolCategory.Shape;
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    return EnumSymbolCategory.Object;
                default:
                    break;
            }

            return EnumSymbolCategory.None;
        }

        public static bool IsSymbolCategory(int num)
        {
            switch ((EnumShapeType)num)
            {
                case EnumShapeType.NONE:
                    return false;
                case EnumShapeType.TEXT:
                    return true;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                    return false;
                case EnumShapeType.FENCE:
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    return false;
                default:
                    break;
            }

            return false;
        }

        public static bool IsShapeCategory(int num)
        {
            switch ((EnumShapeType)num)
            {
                case EnumShapeType.NONE:
                    return false;
                case EnumShapeType.TEXT:
                    return false;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                    return true;
                case EnumShapeType.FENCE:
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    return false;
                default:
                    break;
            }

            return false;
        }

        public static bool IsObjectCategory(int num)
        {
            switch ((EnumShapeType)num)
            {
                case EnumShapeType.NONE:
                    return false;
                case EnumShapeType.TEXT:
                    return false;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                    return false;
                case EnumShapeType.FENCE:
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    return true;
                default:
                    break;
            }

            return false;
        }

    }
}
