using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Events.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public class EventViewModelVisitor
        : EventVisitor
    {
        #region - Ctors -
        public EventViewModelVisitor(
            PreEventProvider preEventProvider
            //PendingEventProvider pendingEventProvider
            , PostEventProvider postEventProvider
            , EventSetupModel setupModel)
        {
            PreEventProvider = preEventProvider;
            //PendingEventProvider = pendingEventProvider;
            PostEventProvider = postEventProvider;
            EventSetupModel = setupModel;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public override void Visit(PreIntrusionEventViewModel viewModel, ActionEventModel actionModel)
        {
            try
            {
                var eventViewModel = new PostIntrusionEventViewModel(actionModel)
                {
                    EventAggregator = viewModel.EventAggregator,
                    #region Commemt
                    //FromEventModel = actionModel.FromEvent,
                    //NameArea = viewModel.NameArea,
                    //NameDevice = viewModel.NameDevice,
                    //NameAreaSpeaker = viewModel.NameAreaSpeaker,
                    //EventId = actionModel.FromEvent.Id,
                    //DateTimeAction = actionModel.DateTime,
                    //ActionContents = actionModel.Content,
                    //IdUser = actionModel.User,
                    #endregion
                };

                DispatcherService.Invoke((System.Action)(() =>
                {
                    viewModel.Cancel();
                    PreEventProvider?.Remove(viewModel);
                    //PendingEventProvider?.Remove(viewModel);
                    PostEventProvider?.Add(eventViewModel);
                }));

                viewModel = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                GC.Collect(0);
            }
        }

        public override void Visit(PreFaultEventViewModel viewModel, ActionEventModel actionModel)
        {
            try
            {
                Func<int, string> funcReason = (reasonType) =>
                {
                    return EnumLanguageHelper.GetEventType(LanguageConst.KOREAN, reasonType);
                };

                Func<int, string> funcType = (uType) =>
                {
                    return EnumLanguageHelper.GetDeviceType(LanguageConst.KOREAN, uType);
                };


                var model = viewModel.EventModel as MalfunctionEventModel;
                

                var eventViewModel = new PostFaultEventViewModel(actionModel)
                {
                    EventAggregator = viewModel.EventAggregator,
                    TagFault = viewModel.TagFault,
                };

                DispatcherService.Invoke((System.Action)(() =>
                {
                    viewModel.Cancel();
                    PreEventProvider?.Remove(viewModel);
                    PostEventProvider?.Add(eventViewModel);
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                GC.Collect(0);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public EventSetupModel EventSetupModel { get; }
        public PreEventProvider PreEventProvider { get; }
        public PendingEventProvider PendingEventProvider { get; }
        public PostEventProvider PostEventProvider { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
