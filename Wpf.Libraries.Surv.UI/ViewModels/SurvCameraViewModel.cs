using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:02:06 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvCameraViewModel : BaseViewModel, ISurvCameraViewModel
    {

        #region - Ctors -
        public SurvCameraViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        public SurvCameraViewModel(ISurvCameraModel model) : this()
        {
            _model = model;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        // 추가된 메서드
        public void SubscribeToIpChanged(Action<string> handler)
        {
            IpChanged += handler;
        }

        public void UnsubscribeFromIpChanged(Action<string> handler)
        {
            IpChanged -= handler;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string DeviceName
        {
            get { return _model.DeviceName; }
            set
            {
                _model.DeviceName = value;
                NotifyOfPropertyChange(() => DeviceName);
            }
        }
        public string IpAddress
        {
            get { return _model.IpAddress; }
            set
            {
                _model.IpAddress = value;
                IpChanged?.Invoke(value);
                NotifyOfPropertyChange(() => IpAddress);
            }
        }
        public int Port
        {
            get { return _model.Port; }
            set
            {
                _model.Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }
        public string UserName
        {
            get { return _model.UserName; }
            set
            {
                _model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }
        public string Password
        {
            get { return _model.Password; }
            set
            {
                _model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        public int Mode
        {
            get { return _model.Mode; }
            set
            {
                _model.Mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }
        public string RtspUrl
        {
            get { return _model.RtspUrl; }
            set
            {
                _model.RtspUrl = value;
                NotifyOfPropertyChange(() => RtspUrl);
            }
        }


        public ISurvCameraModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        #endregion
        #region - Attributes -
        private ISurvCameraModel _model;
        public event Action<string> IpChanged;
        #endregion
    }
}
