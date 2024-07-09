using Caliburn.Micro;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;

using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Events.Models;
using Ironwall.Framework.Helpers;
using Ironwall.Libraries.Base.Services;
using System.Web.UI.WebControls;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class PreEventViewModel : ExEventTimerViewModel<IMetaEventModel>
    {
        
        #region - Ctors -
        protected PreEventViewModel(IMetaEventModel eventModel) : base(eventModel)
        {
            _log = IoC.Get<ILogService>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override Task TaskFinal() => SendAction(Contents, IdUser);

        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async virtual Task SendAction(string content = null, string idUser = default)
        {
            ///1. 조치보고하는 계정 정보 확인
            ///2. 조치보고 내용을 위한 이벤트 모델 확인
            ///3. 조치보고를 위한 ActionRequestModel 생성
            ///4. ActionReportRequestMessageModel 이벤트 처리

            IdUser = idUser;
            Contents = content;

            if (idUser == null)
                idUser = IdUser;

            switch (_model.MessageType)
            {
                case EnumEventType.Intrusion:
                    {
                        var eventModel = _model as IDetectionEventModel;
                        //var requestModel = new ActionRequestDetectionModel(content, idUser, eventModel);
                        //await EventAggregator.PublishOnUIThreadAsync(new ActionReportRequestMessageModel<IActionRequestDetectionModel>(requestModel));
                        var actionEvent = new ActionEventModel()
                        {
                            FromEvent = eventModel as MetaEventModel,
                            Content = content,
                            User = idUser,
                        };
                        var requestModel = new ActionRequestModel(actionEvent);
                        await EventAggregator.PublishOnUIThreadAsync(new ActionReportRequestMessageModel(requestModel));

                    }
                    break;
                case EnumEventType.ContactOn:
                    break;
                case EnumEventType.ContactOff:
                    break;
                case EnumEventType.Connection:
                    break;
                case EnumEventType.Action:
                    break;
                case EnumEventType.Fault:
                    {
                        var eventModel = _model as IMalfunctionEventModel;
                        var requestModel = new ActionRequestMalfunctionModel(content, idUser, eventModel);
                        await EventAggregator.PublishOnUIThreadAsync(new ActionReportRequestMessageModel<IActionRequestMalfunctionModel>(requestModel));
                    }
                    break;
                case EnumEventType.WindyMode:
                    break;
                default:
                    break;
            }

           

            //_log.Info($"Id : {requestModel.Id}" +
            //    $", Id : {requestModel.EventId}" +
            //    $", User : {requestModel.User}" +
            //    $", Content : {requestModel.Content}" +
            //    $", MappingGroup : {requestModel.Group}" +
            //    $", Command : {requestModel.Command}" +
            //    $", Status : {requestModel.Status}" +
            //    $", DateTime : {requestModel.DateTime}" +
            //    $"");


            //if(EnumLanguageHelper.GetAutoActionType(LanguageConst.KOREAN) == Contents)
            //    CallAutoEvent?.Invoke(requestModel);
            //else
            //    CallActionEvent?.Invoke(requestModel);
            
            
           

            
            await CloseDialog();
        }

        public void ExecuteActionEvent(IActionResponseModel model)
        {
            CallActionEvent?.Invoke(model);
        }

        protected abstract Task CloseDialog();
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string EventGroup
        {
            get => _model.EventGroup;
            set 
            {
                _model.EventGroup = value;
                NotifyOfPropertyChange(() => EventGroup);
            }
        }

        public EnumEventType? MessageType
        {
            get { return _model.MessageType; }
            set 
            {
                _model.MessageType = value;
                NotifyOfPropertyChange(() => MessageType);
            }
        }

        public BaseDeviceModel Device
        {
            get { return _model.Device; }
            set 
            {
                _model.Device = value;
                NotifyOfPropertyChange(() => Device);
            }
        }
        public EnumTrueFalse Status
        {
            get { return _model.Status; }
            set 
            {
                _model.Status = value; 
                NotifyOfPropertyChange(() => Status);
            }
        }
        public int Map
        {
            get => _map;
            set
            {
                _map = value;
                NotifyOfPropertyChange(() => Map);
            }
        }

        public string IdUser { get; set; }
        public string Contents { get; set; }
        public IMetaEventModel EventModel => _model;
        public IEventAggregator EventAggregator { get; set; }
        #endregion
        #region - Attributes -
        public override event ActionEvent CallAutoEvent;
        public override event ActionEvent CallActionEvent;

        private ILogService _log;
        #endregion

    }
}
