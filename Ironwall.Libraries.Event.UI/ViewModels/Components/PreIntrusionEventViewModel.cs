using Caliburn.Micro;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public class PreIntrusionEventViewModel : PreEventViewModel, IEventViewModelVisitee, IPreIntrusionEventViewModel
    {
        #region - Ctors -
        public PreIntrusionEventViewModel(IDetectionEventModel model) : base(model)
        {
            //IdGroup = model.IdGroup;
            //IdController = model.IdController;
            //IdSensor = model.IdSensor;
            //TypeMessage = model.TypeMessage;
            //TypeDevice = model.TypeDevice;
            //Status = model.Status;
            //Map = model.Map;
            //Sequence = model.Sequence;
            EventGroup = model.EventGroup;
            Status = model.Status;
            MessageType = model.MessageType;
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

        public override Task SendAction(string content = null, string idUser = default)
        {
            if (content == null)
                content = EnumLanguageHelper.GetAutoActionType(LanguageConst.KOREAN);

            #region - Task for Siren -
            Task.Run(() =>
            {

            });
            #endregion
            return base.SendAction(content, idUser);
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
                    //    var type = (int)EnumEventType.Intrusion;
                    //    if (item.IsActive && dialog.ClassName.Equals(targetViewModel))
                    //    {
                    //        var viewModel = item as PreEventRemoveDialogViewModel;
                    //        if ((type == ((PreEventViewModel)viewModel.ExEventViewModel).TypeMessage)
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
                    Debug.WriteLine($"Raised Exception in {nameof(CloseDialog)} of {nameof(PreIntrusionEventViewModel)} : {ex.Message}");
                }
            });
        }

        public override void Dispose()
        {
            _model = new MetaEventModel();
            GC.Collect();
        }

        public void UpdateModel(IDetectionEventModel model)
        {
            _model = model;
            Refresh();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Result
        {
            get { return (_model as IDetectionEventModel).Result; }
            set
            {
                (_model as IDetectionEventModel).Result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }

        public string Type { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
