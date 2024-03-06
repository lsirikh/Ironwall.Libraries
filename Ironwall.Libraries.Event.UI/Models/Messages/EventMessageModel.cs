using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Messages;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Models.Messages
{
    public class OpenPreEventRemoveDialogMessageModel : EventMessageModel<PreEventViewModel> { }
    public class OpenPreEventRemoveAllDialogMessageModel : EventMessageModel<PreEventViewModel> { }
    public class OpenPostEventDetailsDialogMessageModel : EventMessageModel<PostEventViewModel> { }
    public class OpenPreEventFaultDetailsDialogMessageModel : EventMessageModel<PreEventViewModel> { }
    public class OpenPostEventFaultDetailsDialogMessageModel : EventMessageModel<PostEventViewModel> { }

    public class OpenCameraPopupMessageModel : EventMessageModel<PreEventViewModel> { }

    public class ActionReportRequestMessageModel
    {
        public ActionReportRequestMessageModel(IActionRequestModel requestModel)
        {
            Model = requestModel;
        }
        public IActionRequestModel Model { get; set; }
    }

    public class ActionReportResultMessageModel
        : ResultMessageModel
    {
        public ActionReportResultMessageModel(IActionResponseModel responseModel)
            : base(responseModel.Success, responseModel.Message)
        {
            Model = responseModel;
        }
        public IActionResponseModel Model { get; set; }
    }

    


    public class SearchEventMessageModel : IMessageModel
    {
        public SearchEventMessageModel(string startTime, string endTime, EnumEventType enumType)
        {
            StartTime = startTime;
            EndTime = endTime;
            EventType = enumType;
        }

        public string StartTime { get; private set; }
        public string EndTime { get; private set; }
        public EnumEventType EventType { get; private set; }
    }

    public class SearchEventResultMessageModel
        : ResultMessageModel
    {
        public SearchEventResultMessageModel(IResponseModel responseModel)
            : base(responseModel.Success, responseModel.Message)
        {
            Model = responseModel;
        }
        public IResponseModel Model { get; private set; }
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
