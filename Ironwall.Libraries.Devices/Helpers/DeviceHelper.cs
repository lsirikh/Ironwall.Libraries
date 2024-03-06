using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Devices.Helpers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/26/2023 1:04:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class DeviceHelper
    {

        public static EnumDeviceCategory GetCategory(int num)
        {
            switch ((EnumDeviceType)num)
            {
                case EnumDeviceType.NONE:
                    return EnumDeviceCategory.None;
                case EnumDeviceType.Controller:
                    return EnumDeviceCategory.Controller;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return EnumDeviceCategory.Sensor;
                case EnumDeviceType.IpCamera:
                    return EnumDeviceCategory.Camera;
                case EnumDeviceType.Fence_Line:
                    return EnumDeviceCategory.Etc;
                default:
                    return EnumDeviceCategory.None;
            }
        }

        public static bool IsControllerCategory(int num)
        {
            switch ((EnumDeviceType)num)
            {
                case EnumDeviceType.NONE:
                    return false;
                case EnumDeviceType.Controller:
                    return true;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return false;
                case EnumDeviceType.IpCamera:
                    return false;
                case EnumDeviceType.Fence_Line:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsSensorCategory(int num)
        {
            switch ((EnumDeviceType)num)
            {
                case EnumDeviceType.NONE:
                    return false;
                case EnumDeviceType.Controller:
                    return false;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return true;
                case EnumDeviceType.IpCamera:
                    return false;
                case EnumDeviceType.Fence_Line:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsCameraCategory(int num)
        {
            switch ((EnumDeviceType)num)
            {
                case EnumDeviceType.NONE:
                    return false;
                case EnumDeviceType.Controller:
                    return false;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return false;
                case EnumDeviceType.IpCamera:
                    return true;
                case EnumDeviceType.Fence_Line:
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsEtcCategory(int num)
        {
            switch ((EnumDeviceType)num)
            {
                case EnumDeviceType.NONE:
                    return false;
                case EnumDeviceType.Controller:
                    return false;
                case EnumDeviceType.Multi:
                case EnumDeviceType.Fence:
                case EnumDeviceType.Underground:
                case EnumDeviceType.Contact:
                case EnumDeviceType.PIR:
                case EnumDeviceType.IoController:
                case EnumDeviceType.Laser:
                case EnumDeviceType.Cable:
                    return false;
                case EnumDeviceType.IpCamera:
                    return false;
                case EnumDeviceType.Fence_Line:
                    return true;
                default:
                    return false;
            }
        }
    }
}
