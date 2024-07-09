using System;

namespace Ironwall.Libraries.VMS.Common.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 3:28:39 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsSetupModel
    {
        public string TableVmsApiSetting => "vms_api_setting";
        public string TableVmsApiSensor => "vms_api_sensor";
        public string TableVmsApiMapping => "vms_api_mapping";

        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}