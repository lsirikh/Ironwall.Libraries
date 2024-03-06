using Caliburn.Micro;
using System.Diagnostics;
using System.Windows.Controls;
using System;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.UI.Providers.ViewModels;
using System.Linq;
using Wpf.Libraries.Surv.UI.Models;
using System.Threading.Tasks;
using System.Threading;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:02:33 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvEventViewModel : SurvBaseViewModel<ISurvEventModel>
                                    , IHandle<RefreshSurvIpAddressMessageModel>
    {

        #region - Ctors -
        public SurvEventViewModel(ISurvEventModel model
                                , ISurvApiModel apiModel
                                , ISurvCameraModel cameraModel) : base(model)
        {
            Api = apiModel;
            Camera = cameraModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            DisconnectEvent();

            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void LinkEvent(ISurvCameraViewModel viewModel)
        {
            _survCameraViewModel = viewModel;
            _survCameraViewModel?.SubscribeToIpChanged(_survCameraViewModel_IpChanged);
        }

        private void _survCameraViewModel_IpChanged(string obj)
        {
            IpAddress = obj;
        }
       

        public void DisconnectEvent()
        {
            try
            {
                _survCameraViewModel?.UnsubscribeFromIpChanged(_survCameraViewModel_IpChanged);
            }
            catch
            {
                Debug.WriteLine($"Null Reference Exception");
            }
            finally 
            { 
                _survCameraViewModel = null;
            }
        }


        public async void OnClickCameraSetting(object sender, EventArgs e)
        {
            try
            {
                Debug.WriteLine($"{this.Id}");
                if (!(sender is MenuItem item)) return;
                var viewModel = item.DataContext as SurvEventViewModel;
                var viewModelProvider = IoC.Get<SurvCameraViewModelProvider>();
                var viewModelFromProvider = viewModelProvider.Where(entity => entity.Id == viewModel.Camera.Id).FirstOrDefault();
                viewModel.LinkEvent(viewModelFromProvider);
                await _eventAggregator.PublishOnUIThreadAsync(new OpenSurvCameraDialogMessageModel(viewModelFromProvider));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion
        #region - IHanldes -
        public Task HandleAsync(RefreshSurvIpAddressMessageModel message, CancellationToken cancellationToken)
        {
            //if (Camera == message.Model)
            //{
            //    IpAddress = message.Model.IpAddress;
            //}

            return Task.CompletedTask;
        }
        #endregion
        #region - Properties -
        public int Channel
        {
            get { return _model.Channel; }
            set 
            { 
                _model.Channel = value;
                NotifyOfPropertyChange(() => Channel);
            }
        }

        public string EventName
        {
            get { return _model.EventName; }
            set
            {
                _model.EventName = value;
                NotifyOfPropertyChange(() => EventName);
            }
        }

        public string IpAddress
        {
            get { return _model.IpAddress; }
            set
            {
                _model.IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public bool IsOn
        {
            get { return _model.IsOn; }
            set
            {
                _model.IsOn = value;
                NotifyOfPropertyChange(() => IsOn);
            }
        }

        public int EventId
        {
            get { return _model.EventId; }
            set
            {
                _model.EventId = value;
                NotifyOfPropertyChange(() => EventId);
            }
        }

        public ISurvApiModel Api
        {
            get { return _api; }
            set
            {
                _api = value;
                NotifyOfPropertyChange(() => Api);
                if (value != null)
                    _model.ApiId = value.Id;
            }
        }

        public ISurvCameraModel Camera
        {
            get { return _camera; }
            set
            {
                _camera = value;
                NotifyOfPropertyChange(() => Camera);
                if(value != null)
                    _model.CameraId  = value.Id;
            }
        }
        #endregion
        #region - Attributes -
        private ISurvApiModel _api;
        private ISurvCameraModel _camera;
        private ISurvCameraViewModel _survCameraViewModel;
        #endregion
    }
}
