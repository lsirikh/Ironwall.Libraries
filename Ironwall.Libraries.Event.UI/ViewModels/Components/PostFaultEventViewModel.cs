using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public class PostFaultEventViewModel
        : PostEventViewModel
        , IEventViewModelVisitee
    {
        #region - Ctors -
        public PostFaultEventViewModel(IActionEventModel model)
            : base(model)
        {
            //LoginViewModel을 통해 사용자 번호를 입력해준다.

            //IdGroup = model.EventGroup;
            //IdController = (model.Device as ISensorDeviceModel).CONTROLLER.DeviceNumber;
            //IdSensor = (model.Device as ISensorDeviceModel).DeviceNumber;
            //TypeMessage = model.MessageType;
            //TypeDevice = model.Device.DeviceType;
            //Status = model.Status;
            //Reason = model.Reason;
            //CutFirstStart = model.FirstStart;
            //CutFirstEnd = model.FirstEnd;
            //CutSecondStart = model.SecondStart;
            //CutSecondEnd = model.SecondEnd;
            //Map = ?
        }

        #endregion
        #region - Implementation of Interface -
        public void Accept(EventVisitor visitor, ActionEventModel actionModel)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async void OnClickButtonActionDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                await EventAggregator?.PublishOnCurrentThreadAsync(new OpenPostEventFaultDetailsDialogMessageModel { Value = this });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //int Reason { get; set; }
        //int CutFirstStart { get; set; }
        //int CutFirstEnd { get; set; }
        //int CutSecondStart { get; set; }
        //int CutSecondEnd { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
