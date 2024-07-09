using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Vms;
using Sensorway.Events.Base.Models;
using System;
using System.Collections.Generic;

namespace Ironwall.Libraries.VMS.UI.Messages
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/3/2024 8:59:41 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/

    //////////////////////////////////////////////////////
    public class RequestApiSettingInsertMessage
    {
        public RequestApiSettingInsertMessage(List<IVmsApiModel> list)
        {
            ApiSettings = list;
        }
        public List<IVmsApiModel> ApiSettings { get; }
    }

    public class ResponseApiSettingInsertMessage : ResultMessageModel
    {
        public ResponseApiSettingInsertMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestApiSettingReloadMessage
    {
    }

    public class ResponseApiSettingReloadMessage : ResultMessageModel
    {
        public ResponseApiSettingReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    //////////////////////////////////////////////////////
    public class RequestApiEventListMessage
    {

    }
    public class ResponseApiEventListMessage : ResultMessageModel
    {
        public ResponseApiEventListMessage(bool isSuccess, string message, List<IEventModel> list) : base(isSuccess, message)
        {
            Events = list;
        }
        public List<IEventModel> Events { get; }
    }
    public class RequestApiMappingInsertMessage
    {
        public RequestApiMappingInsertMessage(List<IVmsMappingModel> list)
        {
            Mappings = list;
        }
        public List<IVmsMappingModel> Mappings { get; }
    }

    public class ResponseApiMappingInsertMessage : ResultMessageModel
    {
        public ResponseApiMappingInsertMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestApiMappingReloadMessage
    {
    }

    public class ResponseApiMappingReloadMessage : ResultMessageModel
    {
        public ResponseApiMappingReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    //////////////////////////////////////////////////////

    public class RequestApiSensorInsertMessage
    {
        public RequestApiSensorInsertMessage(List<IVmsSensorModel> list)
        {
            ApiSensor = list;
        }
        public List<IVmsSensorModel> ApiSensor { get; }
    }

    public class ResponseApiSensorInsertMessage : ResultMessageModel
    {
        public ResponseApiSensorInsertMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestApiSensorReloadMessage
    {
    }

    public class ResponseApiSensorReloadMessage : ResultMessageModel
    {
        public ResponseApiSensorReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }
    //////////////////////////////////////////////////////


    public class ResultMessageModel
    {
        public ResultMessageModel(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public bool IsSuccess { get; }
        public string Message { get; }
    }
}