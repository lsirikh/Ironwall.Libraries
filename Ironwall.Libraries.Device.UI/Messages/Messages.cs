using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Device.UI.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Ironwall.Libraries.Device.UI.Messages
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/3/2023 5:34:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OpenMappingInsertDialog
    {
        public OpenMappingInsertDialog(ObservableCollection<CameraMappingViewModel> provider)
        {
            Provider = provider;
        }

        public ObservableCollection<CameraMappingViewModel> Provider { get; }
    }

    public class MappingAppliedMessage
    {
        public MappingAppliedMessage()
        {
        }
    }

    public class RequestMappingInsertMessage
    {
    }

    
    public class ResponseMappingInsertMessage : ResultMessageModel
    {
        public ResponseMappingInsertMessage(bool isSuccess, string message): base(isSuccess, message) 
        {
        }
    }

    public class RequestMappingReloadMessage
    {
    }


    public class ResponseMappingReloadMessage : ResultMessageModel
    {
        public ResponseMappingReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestCameraInsertMessage
    {
    }


    public class ResponseCameraInsertMessage : ResultMessageModel
    {
        public ResponseCameraInsertMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestCameraReloadMessage
    {
    }

    public class ResponseCameraReloadMessage : ResultMessageModel
    {
        public ResponseCameraReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestPresetInsertMessage
    {
    }


    public class ResponsePresetInsertMessage : ResultMessageModel
    {
        public ResponsePresetInsertMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

    public class RequestPresetReloadMessage
    {
    }

    public class ResponsePresetReloadMessage : ResultMessageModel
    {
        public ResponsePresetReloadMessage(bool isSuccess, string message) : base(isSuccess, message)
        {
        }
    }

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
