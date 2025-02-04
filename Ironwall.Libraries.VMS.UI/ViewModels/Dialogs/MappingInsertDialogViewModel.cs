﻿using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using Ironwall.Framework.Models.Devices;
using System.Linq;
using Ironwall.Framework.Helpers;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using System.Windows.Documents;
using System.Collections.Generic;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.VMS.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/3/2023 10:07:51 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MappingInsertDialogViewModel : BaseViewModel
    {

        #region - Ctors -
        public MappingInsertDialogViewModel(IEventAggregator eventAggregator
                                            , ILogService log) 
                                            : base(eventAggregator, log)
        {

        }
        #endregion
        #region - Implementation of Interface -
        public async void ClickCancelAsync()
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new CloseDialogMessageModel());
        }

        public Task ClickOkAsync()
        {
            ///ClickOkAsync의 로직 수행
            return Task.Run(async () =>
            {
                try
                {
                    await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                    ///Validation Checkup
                    ///01.동일 그룹 체크
                    //var mappingProvider = IoC.Get<CameraMappingProvider>();

                    //var matchedGroupCount = mappingProvider.Where(entity => entity.MappingGroup == Group).Count();
                    //if (matchedGroupCount > 0) throw new Exception(message: $"There is the same group name({Group})!");

                    //int mappingPreviewId = 0;
                    //if(mappingProvider.Count > 0)
                    //   mappingPreviewId = mappingProvider.Select(entity => entity.Id).LastOrDefault();

                    //List<ICameraMappingModel> mappingList = new List<ICameraMappingModel>();

                    //for (int i = 0; i < ItemCount; i++)
                    //{
                    //    var sensor = SensorProvider.Where(entity => entity.Id == SensorDeviceViewModel.Id + i).FirstOrDefault();
                    //    if (sensor == null) throw new NullReferenceException(message: $"s{SensorDeviceViewModel.Id + i} was not exist!");
                    //    var model = new CameraMappingModel(
                    //        Group
                    //        , sensor
                    //        , SelectedFirstPreset
                    //        , SelectedSecondPreset);

                    //    mappingList.Add(model);

                    //}

                    //await _eventAggregator.PublishOnUIThreadAsync(new MappingAppliedMessage(mappingList));

                    //await _eventAggregator.PublishOnUIThreadAsync(new CloseDialogMessageModel());

                    await Task.Delay(500);
                    await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());

                }
                catch (NullReferenceException ex)
                {
                    _log.Error($"Rasied {nameof(NullReferenceException)}({nameof(ClickOkAsync)}): {ex.Message}", _class);
                    var explain = ex.Message;

                    await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
                    {
                        Explain = explain
                    }, _cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    _log.Error($"Rasied {nameof(Exception)}({nameof(ClickOkAsync)}): {ex.Message}", _class);
                    var explain = ex.Message;

                    await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
                    {
                        Explain = explain
                    }, _cancellationTokenSource.Token);
                }

            });
        }
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {

            await base.OnActivateAsync(cancellationToken);

            _ = DataInitialize(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            SensorDeviceViewModel = null;
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private object DataInitialize(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {

                Maximum = 100;
                TickFrequency = 1;
                ItemCount = 0;
                Group = null;

                await Task.Delay(500);
                //ViewModelProvider Setting


                SensorProvider = IoC.Get<SensorDeviceProvider>();

                NotifyOfPropertyChange(() => SensorProvider);
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        public int TickFrequency
        {
            get { return _tickFrequency; }
            set
            {
                _tickFrequency = value;
                NotifyOfPropertyChange(() => TickFrequency);
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                NotifyOfPropertyChange(() => ItemCount);
            }
        }


        public string Group
        {
            get { return _group; }
            set { _group = value; NotifyOfPropertyChange(() => Group); }
        }



        public SensorDeviceModel SensorDeviceViewModel
        {
            get { return _sensorDeviceViewModel; }
            set 
            {
                _sensorDeviceViewModel = value;
                NotifyOfPropertyChange(() => SensorDeviceViewModel);
            }
        }

        public SensorDeviceProvider SensorProvider { get; private set; }
        #endregion
        #region - Attributes -
        private int _maximum;
        private int _tickFrequency;
        private int _itemCount;
        private string _group;

        private SensorDeviceModel  _sensorDeviceViewModel;
        #endregion
    }
}
