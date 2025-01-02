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
    public class PostIntrusionEventViewModel
        : PostEventViewModel
        , IEventViewModelVisitee
    {
        #region - Ctors -
        public PostIntrusionEventViewModel(IActionEventModel model)
            : base(model)
        {
            //IdGroup = model.EventGroup;
            //IdController = (model.Device as ISensorDeviceModel).CONTROLLER.DeviceNumber;
            //IdSensor = (model.Device as ISensorDeviceModel).DeviceNumber;
            //TypeMessage = model.MessageType;
            //TypeDevice = model.Device.DeviceType;
            //Status = model.Status;
        }

        //PostIntrusionEventViewModel(ActionEventModel actionModel)
        //    : base(actionModel.FromEvent)
        //{

        //}

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
                await EventAggregator?.PublishOnCurrentThreadAsync(new OpenPostEventDetailsDialogMessageModel { Value = this });

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, _class);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
