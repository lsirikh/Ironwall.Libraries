using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Helpers
{
    public static class EnumLanguageHelper
    {
        public static string GetEventType(string langCode, int enumId)
        {
            if (LanguageConst.ENGLISH == langCode)
                switch ((EnumFaultType)enumId)
                {
                    case EnumFaultType.FaultController:
                        return TypeConst_en.FAULT_CONTROLLER;
                    case EnumFaultType.FaultFence:
                        return TypeConst_en.FAULT_FENCE;
                    case EnumFaultType.FaultCompound:
                        return TypeConst_en.FAULT_MULTI;
                    case EnumFaultType.FaultCable:
                        return TypeConst_en.FAULT_CABLE;
                    case EnumFaultType.FaultUnderground:
                        return TypeConst_en.FAULT_UNDERGROUND;
                    case EnumFaultType.FaultPIR:
                        return TypeConst_en.FAULT_PIR;
                    case EnumFaultType.FaultIOController:
                        return TypeConst_en.FAULT_IOCONTROLLER;
                    case EnumFaultType.FaultContact:
                        return TypeConst_en.FAULT_CONTACT;
                    case EnumFaultType.FaultIpCamera:
                        return TypeConst_en.FAULT_CAMERA;
                    case EnumFaultType.FaultIpSpeaker:
                        return TypeConst_en.FAULT_SPEAKER;
                    case EnumFaultType.FaultRadar:
                        return TypeConst_en.FAULT_RADAR;
                    case EnumFaultType.FaultOpticalCable:
                        return TypeConst_en.FAULT_OPTICAL_CABLE;
                    default:
                        return "Unknown";
                }
            
            else if (LanguageConst.KOREAN == langCode)
                switch ((EnumFaultType)enumId)
                {
                    case EnumFaultType.FaultController:
                        return TypeConst_kr.FAULT_CONTROLLER;
                    case EnumFaultType.FaultFence:
                        return TypeConst_kr.FAULT_FENCE;
                    case EnumFaultType.FaultCompound:
                        return TypeConst_kr.FAULT_MULTI;
                    case EnumFaultType.FaultCable:
                        return TypeConst_kr.FAULT_CABLE;
                    case EnumFaultType.FaultUnderground:
                        return TypeConst_kr.FAULT_UNDERGROUND;
                    case EnumFaultType.FaultPIR:
                        return TypeConst_kr.FAULT_PIR;
                    case EnumFaultType.FaultIOController:
                        return TypeConst_kr.FAULT_IOCONTROLLER;
                    case EnumFaultType.FaultContact:
                        return TypeConst_kr.FAULT_CONTACT;
                    case EnumFaultType.FaultIpCamera:
                        return TypeConst_kr.FAULT_CAMERA;
                    case EnumFaultType.FaultIpSpeaker:
                        return TypeConst_kr.FAULT_SPEAKER;
                    case EnumFaultType.FaultRadar:
                        return TypeConst_kr.FAULT_RADAR;
                    case EnumFaultType.FaultOpticalCable:
                        return TypeConst_kr.FAULT_OPTICAL_CABLE;
                    default:
                        return "알 수 없음";
                }
            else
                switch ((EnumFaultType)enumId)
                {
                    case EnumFaultType.FaultController:
                        return TypeConst_en.FAULT_CONTROLLER;
                    case EnumFaultType.FaultFence:
                        return TypeConst_en.FAULT_FENCE;
                    case EnumFaultType.FaultCompound:
                        return TypeConst_en.FAULT_MULTI;
                    case EnumFaultType.FaultCable:
                        return TypeConst_en.FAULT_CABLE;
                    case EnumFaultType.FaultUnderground:
                        return TypeConst_en.FAULT_UNDERGROUND;
                    case EnumFaultType.FaultPIR:
                        return TypeConst_en.FAULT_PIR;
                    case EnumFaultType.FaultIOController:
                        return TypeConst_en.FAULT_IOCONTROLLER;
                    case EnumFaultType.FaultContact:
                        return TypeConst_en.FAULT_CONTACT;
                    case EnumFaultType.FaultIpCamera:
                        return TypeConst_en.FAULT_CAMERA;
                    case EnumFaultType.FaultIpSpeaker:
                        return TypeConst_en.FAULT_SPEAKER;
                    case EnumFaultType.FaultRadar:
                        return TypeConst_en.FAULT_RADAR;
                    case EnumFaultType.FaultOpticalCable:
                        return TypeConst_en.FAULT_OPTICAL_CABLE;
                    default:
                        return "Unknown";
                }
        }

        public static string GetDeviceType(string langCode, int enumId)
        {
            if (LanguageConst.ENGLISH == langCode)
                switch ((EnumDeviceType)enumId)
                {
                    case EnumDeviceType.Controller:
                        return UnitConst_en.CONTROLLER;
                    case EnumDeviceType.Multi:
                        return UnitConst_en.MULTI;
                    case EnumDeviceType.Fence:
                        return UnitConst_en.FENCE;
                    case EnumDeviceType.Underground:
                        return UnitConst_en.UNDERGROUND;
                    case EnumDeviceType.Contact:
                        return UnitConst_en.CONTACT;
                    case EnumDeviceType.PIR:
                        return UnitConst_en.PIR;
                    case EnumDeviceType.IoController:
                        return UnitConst_en.IOCONTROLLER;
                    case EnumDeviceType.Laser:
                        return UnitConst_en.LASER;
                    case EnumDeviceType.Cable:
                        return UnitConst_en.CABLE;
                    
                    case EnumDeviceType.IpCamera:
                        return UnitConst_en.CAMERA;
                    case EnumDeviceType.IpSpeaker:
                        return UnitConst_en.SPEAKER;
                    case EnumDeviceType.Radar:
                        return UnitConst_en.RADAR;
                    case EnumDeviceType.OpticalCable:
                        return UnitConst_en.OPTICAL_CABLE;
                    default:
                        return "알 수 없음";
                }
            else if (LanguageConst.KOREAN == langCode)
                switch ((EnumDeviceType)enumId)
                {
                    case EnumDeviceType.Controller:
                        return UnitConst_kr.CONTROLLER;
                    case EnumDeviceType.Multi:
                        return UnitConst_kr.MULTI;
                    case EnumDeviceType.Fence:
                        return UnitConst_kr.FENCE;
                    case EnumDeviceType.Underground:
                        return UnitConst_kr.UNDERGROUND;
                    case EnumDeviceType.Contact:
                        return UnitConst_kr.CONTACT;
                    case EnumDeviceType.PIR:
                        return UnitConst_kr.PIR;
                    case EnumDeviceType.IoController:
                        return UnitConst_kr.IOCONTROLLER;
                    case EnumDeviceType.Laser:
                        return UnitConst_kr.LASER;
                    case EnumDeviceType.Cable:
                        return UnitConst_kr.CABLE;

                    case EnumDeviceType.IpCamera:
                        return UnitConst_kr.CAMERA;
                    case EnumDeviceType.IpSpeaker:
                        return UnitConst_kr.SPEAKER;
                    case EnumDeviceType.Radar:
                        return UnitConst_kr.RADAR;
                    case EnumDeviceType.OpticalCable:
                        return UnitConst_kr.OPTICAL_CABLE;

                    default:
                        return "Unknown";
                }
            else
                switch ((EnumDeviceType)enumId)
                {
                    case EnumDeviceType.Controller:
                        return UnitConst.CONTROLLER;
                    case EnumDeviceType.Multi:
                        return UnitConst.MULTI;
                    case EnumDeviceType.Fence:
                        return UnitConst.FENCE;
                    case EnumDeviceType.Underground:
                        return UnitConst.UNDERGROUND;
                    case EnumDeviceType.Contact:
                        return UnitConst.CONTACT;
                    case EnumDeviceType.PIR:
                        return UnitConst.PIR;
                    case EnumDeviceType.IoController:
                        return UnitConst.IOCONTROLLER;
                    case EnumDeviceType.Laser:
                        return UnitConst.LASER;
                    case EnumDeviceType.Cable:
                        return UnitConst.CABLE;

                    case EnumDeviceType.IpCamera:
                        return UnitConst_en.CAMERA;
                    case EnumDeviceType.IpSpeaker:
                        return UnitConst_en.SPEAKER;
                    case EnumDeviceType.Radar:
                        return UnitConst_en.RADAR;
                    case EnumDeviceType.OpticalCable:
                        return UnitConst_en.OPTICAL_CABLE;
                    default:
                        return "Unknown";
                }
        }

        public static string GetAutoActionType(string langCode)
        {
            if (LanguageConst.ENGLISH == langCode)
                return "Automatic Action Reporting";
            else if (LanguageConst.KOREAN == langCode)
                return "자동 조치 보고";
            else
                return "Automatic Action Reporting";

        }

        public static string GetAutoRecoveryType(string langCode)
        {
            if (LanguageConst.ENGLISH == langCode)
                return "Auto-recovery report";
            else if (LanguageConst.KOREAN == langCode)
                return "자동 복구 보고 ";
            else
                return "Auto-recovery report";

        }
    }
}
