﻿using Caliburn.Micro;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Components;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Events.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    public class PreEventListPanelViewModel : BaseEventListPanelViewModel
                                             , IHandle<ActionReportResultMessageModel>
    {
        #region - Ctors -
        public PreEventListPanelViewModel(
            IEventAggregator eventAggregator
            , ILogService log
            , ActionEventProvider actionEventProvdier
            , PreEventProvider preEventProvider
            , PendingEventProvider pendingEventProvider
            , PostEventProvider postEventProvider
            , EventSetupModel eventSetupModel)
            : base(eventAggregator, log, eventSetupModel)
        {
            ActionEventProvider = actionEventProvdier;

            PreEventProvider = preEventProvider;
            _pendingEventProvider = pendingEventProvider;
            PostEventProvider = postEventProvider;

            NameTabHeader = "탐지 이벤트";
            NameKind = "EventAlert";

            CollectionEventViewModel = PreEventProvider.CollectionEntity;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //PreEventProvider.CollectionEntity.CollectionChanged += CollectionEntity_CollectionChanged;

            return base.OnActivateAsync(cancellationToken);
        }

        
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async void OnClickButtonActionAll(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            var dataContext = source.DataContext as PreEventViewModel;

            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenPreEventRemoveAllDialogMessageModel());
        }

        public void OnClickEventCard(object sender, RoutedEventArgs e)
        {
            if (!((sender as ListBox).SelectedItem is PreEventViewModel preEventViewModel))
                return;

            if (_eventSetupModel.EventCardMapChange)
            {
                //await _eventAggregator.PublishOnCurrentThreadAsync(new OpenCanvasMessageModel() { MapNumber = preEventViewModel.Map });
            }
        }


        public async void OnButtonAction(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            var dataContext = source.DataContext as PreEventViewModel;

            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenPreEventRemoveDialogMessageModel { Value = dataContext });

        }

        public async void OnButtonCameraPopup(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            var dataContext = source.DataContext as PreEventViewModel;

            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenCameraPopupMessageModel { Value = dataContext });
        }

        public Task HandleAsync(ActionReportResultMessageModel message, CancellationToken cancellationToken)
        {
            //When Action Report was successfully registered, FromEvent from PreEventProvider has to be removed.
            IActionEventModel model = message.Model.Body;
            try
            {
                foreach (var item in PreEventProvider.ToList())
                {
                    if (item.Id == model.FromEvent.Id)
                    {
                        var visitor = new EventViewModelVisitor(_eventAggregator, _log, PreEventProvider, PostEventProvider, _eventSetupModel);

                        var actionModel = ActionEventProvider.Where(t => t.Id == model.Id).FirstOrDefault() as ActionEventModel;

                        //PreEventProvider Remove, PostEventProvider Add
                        ((IEventViewModelVisitee)item).Accept(visitor, actionModel);

                        item.ExecuteActionEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(HandleAsync)} of {nameof(ActionReportResultMessageModel)} in {nameof(PreEventListPanelViewModel)} : {ex.Message}");
            }

            return Task.CompletedTask;

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public PreEventViewModel SelectedPreEventViewModel
        {
            get => _selectedPreEventViewModel;
            set
            {
                _selectedPreEventViewModel = value;
                NotifyOfPropertyChange(() => SelectedPreEventViewModel);
            }
        }

        public EventProvider EventProvdier { get; }
        public ActionEventProvider ActionEventProvider { get; }
        public PreEventProvider PreEventProvider { get; }

        private PendingEventProvider _pendingEventProvider;

        public PostEventProvider PostEventProvider { get; }

        public ObservableCollection<PreEventViewModel> CollectionEventViewModel { get; set; }
        #endregion
        #region - Attributes -
        private PreEventViewModel _selectedPreEventViewModel;
        #endregion
    }
}
