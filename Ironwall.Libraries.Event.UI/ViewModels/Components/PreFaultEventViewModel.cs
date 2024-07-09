using Caliburn.Micro;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Enums;
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
    public class PreFaultEventViewModel : PreEventViewModel, IEventViewModelVisitee, IPreFaultEventViewModel
    {
        #region - Ctors -
        
        public PreFaultEventViewModel(IMalfunctionEventModel model) : base(model)
        {
            MessageType = EnumEventType.Fault;
        }
        #endregion
        #region - Implementation of Interface -
        public void Accept(EventVisitor visitor, ActionEventModel actionModel)
        {
            visitor.Visit(this, actionModel);
        }
        #endregion
        #region - Overrides -
        public override async Task TaskFinal() => await SendAction(Contents, IdUser);
        public override Task SendAction(string content = null, string idUser = null)
        {
            if (content == null)
                content = EnumLanguageHelper.GetAutoActionType(LanguageConst.KOREAN);

            content = string.IsNullOrEmpty(Tag) ? content : Tag;
            return base.SendAction(content, idUser);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async void OnClickButtonActionDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                await EventAggregator?.PublishOnCurrentThreadAsync(new OpenPreEventFaultDetailsDialogMessageModel { Value = this });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override Task CloseDialog()
        {
            return Task.Run(() =>
            {
                try
                {
                    //var dialogShellViewModel = IoC.Get<DialogShellViewModel>();
                    //var dialogViewModels = dialogShellViewModel.Items;
                    //foreach (var item in dialogViewModels)
                    //{
                    //    var dialog = item as IBaseViewModel;
                    //    var targetViewModel = nameof(PreEventRemoveDialogViewModel);

                    //    if (item.IsActive && dialog.ClassName.Equals(targetViewModel))
                    //    {
                    //        var viewModel = item as PreEventRemoveDialogViewModel;
                    //        if ((Reason == ((PreFaultEventViewModel)viewModel.ExEventViewModel).Reason)
                    //        && (IdController == viewModel.ExEventViewModel.IdController)
                    //        && (IdSensor == viewModel.ExEventViewModel.IdSensor))
                    //        {
                    //            await EventAggregator?.PublishOnCurrentThreadAsync(new CloseDialogMessageModel());
                    //            break;
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CloseDialog)} of {nameof(PreFaultEventViewModel)} : {ex.Message}");
                }
            });

        }

        public override void Dispose()
        {
            _model = new MetaEventModel();
            GC.Collect();
        }

        public void UpdateModel(IMalfunctionEventModel model)
        {
            _model = model;
            Refresh();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //public string NameArea { get; set; }
        //public string NameDevice { get; set; }
        //public string NameAreaSpeaker { get; set; }
        //public string Action { get; set; }
        public string Type { get; set; }
        public EnumFaultType Reason
        {
            get => (EventModel as IMalfunctionEventModel).Reason;
            set
            {
                (EventModel as IMalfunctionEventModel).Reason = value;
                NotifyOfPropertyChange(() => Type);
            }
        }
        public int CutFirstStart
        {
            get => (EventModel as IMalfunctionEventModel).FirstStart;
            set
            {
                (EventModel as IMalfunctionEventModel).FirstStart = value;
                NotifyOfPropertyChange(() => CutFirstStart);
            }
        }
        public int CutFirstEnd
        {
            get => (EventModel as IMalfunctionEventModel).FirstEnd;
            set
            {
                (EventModel as IMalfunctionEventModel).FirstEnd = value;
                NotifyOfPropertyChange(() => CutFirstEnd);
            }
        }
        public int CutSecondStart
        {
            get => (EventModel as IMalfunctionEventModel).SecondStart;
            set
            {
                (EventModel as IMalfunctionEventModel).SecondStart = value;
                NotifyOfPropertyChange(() => CutSecondStart);
            }
        }
        public int CutSecondEnd
        {
            get => (EventModel as IMalfunctionEventModel).SecondEnd;
            set
            {
                (EventModel as IMalfunctionEventModel).SecondEnd = value;
                NotifyOfPropertyChange(() => CutSecondEnd);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
