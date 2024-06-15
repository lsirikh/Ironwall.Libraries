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

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class PreEventViewModel : ExEventTimerViewModel<IMetaEventModel>
    {
        #region - Ctors -
        protected PreEventViewModel(IMetaEventModel eventModel) : base(eventModel)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override Task TaskFinal() => SendAction();

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

            ActionRequestModel requestModel = null;
            var metaEvent = _model;

            if (idUser == null)
                idUser = IdUser;

            switch ((EnumEventType)metaEvent.MessageType)
            {
                case EnumEventType.Intrusion:
                    {
                        var eventModel = metaEvent as IDetectionEventModel;
                        requestModel = RequestFactory.Build<ActionRequestModel>(content, idUser, eventModel);
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
                        var eventModel = metaEvent as IMalfunctionEventModel;
                        requestModel = RequestFactory.Build<ActionRequestModel>(content, idUser, eventModel);
                    }
                    break;
                case EnumEventType.WindyMode:
                    break;
                default:
                    break;
            }

            IdUser = idUser;
            Contents = content;

            Debug.WriteLine($"This : {this.GetHashCode()}");
            Debug.WriteLine($"EventAggregator : {EventAggregator.GetHashCode()}");
            Debug.WriteLine($"IdUser : {IdUser}, Contents : {Contents}");
            Debug.WriteLine($"Id : {requestModel.Id}" +
                $", EventId : {requestModel.EventId}" +
                $", User : {requestModel.User}" +
                $", Content : {requestModel.Content}" +
                $", GroupId : {requestModel.Group}" +
                $", Command : {requestModel.Command}" +
                $", Status : {requestModel.Status}" +
                $", DateTime : {requestModel.DateTime}" +
                $"");


            //if(EnumLanguageHelper.GetAutoActionType(LanguageConst.KOREAN) == Contents)
            //    CallAutoEvent?.Invoke(requestModel);
            //else
            //    CallActionEvent?.Invoke(requestModel);

            await EventAggregator.PublishOnUIThreadAsync(new ActionReportRequestMessageModel(requestModel));
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

        public IBaseDeviceModel Device
        {
            get { return _model.Device; }
            set 
            {
                _model.Device = value;
                NotifyOfPropertyChange(() => Device);
            }
        }
        public int Status
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

        #region Comment
        //public string IdGroup
        //{
        //    get => idGroup;
        //    set
        //    {
        //        idGroup = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public int IdController
        //{
        //    get => idController;
        //    set
        //    {
        //        idController = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int IdSensor
        //{
        //    get => idSensor;
        //    set
        //    {
        //        idSensor = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int TypeMessage
        //{
        //    get => typeMessage;
        //    set
        //    {
        //        typeMessage = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int TypeDevice
        //{
        //    get => typeDevice;
        //    set
        //    {
        //        typeDevice = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string Device
        //{
        //    get => device;
        //    set
        //    {
        //        device = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int Status
        //{
        //    get => status;
        //    set
        //    {
        //        status = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public int Sequence
        //{
        //    get => sequence;
        //    set
        //    {
        //        sequence = value;
        //        OnPropertyChanged();
        //    }
        //}
        #endregion

        public string IdUser { get; set; }
        public string Contents { get; set; }
        public IMetaEventModel EventModel => _model;
        public IEventAggregator EventAggregator { get; set; }
        #endregion
        #region - Attributes -
        public override event ActionEvent CallAutoEvent;
        public override event ActionEvent CallActionEvent;
        #endregion

    }
}
