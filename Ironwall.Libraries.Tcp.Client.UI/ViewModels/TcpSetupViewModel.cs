using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Tcp.Common.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.Tcp.Client.UI.Models.Messages;
using Ironwall.Libraries.Tcp.Client.UI.Models;
using Ironwall.Libraries.Tcp.Client.UI.Infos;

namespace Ironwall.Libraries.Tcp.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 10:11:17 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class TcpSetupViewModel : BaseViewModel
    {

        #region - Ctors -
        public TcpSetupViewModel(
            IEventAggregator eventAggregator
            , TcpClientSetupModel tcpClientSetupModel
            , ClientStatusViewModel clientStatusViewModel
            ) : base(eventAggregator)
        {

            _tcpClientSetupModel = tcpClientSetupModel;

            ClientStatusViewModel = clientStatusViewModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //_eventAggregator.SubscribeOnUIThread(this);
            PropertyInitialize();
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            //_eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public async void TcpConnectionButton()
        {
            try
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                var model = MessageFactory.Build<ClientConnectionMessage>(_tcpClientSetupModel.ClientId, _tcpClientSetupModel.ServerIp, _tcpClientSetupModel.ServerPort, true);
                await _eventAggregator.PublishOnUIThreadAsync(model);

                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        public async void TcpDisconnectionButton()
        {
            try
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnCurrentThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                var model = MessageFactory.Build<ClientConnectionMessage>(_tcpClientSetupModel.ClientId, _tcpClientSetupModel.ServerIp, _tcpClientSetupModel.ServerPort, false);
                await _eventAggregator.PublishOnUIThreadAsync(model);

                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }


        }
        #endregion
        #region - Processes -
        private void PropertyInitialize()
        {
            try
            {
                string serverIp = _tcpClientSetupModel.ServerIp;

                ServerClassA = serverIp.Split('.')[0];
                ServerClassB = serverIp.Split('.')[1];
                ServerClassC = serverIp.Split('.')[2];
                ServerClassD = serverIp.Split('.')[3];

                ServerPort = _tcpClientSetupModel.ServerPort;

                string clientIp = _tcpClientSetupModel.ClientIp;

                ClientClassA = clientIp.Split('.')[0];
                ClientClassB = clientIp.Split('.')[1];
                ClientClassC = clientIp.Split('.')[2];
                ClientClassD = clientIp.Split('.')[3];

                ClientPort = _tcpClientSetupModel.ClientPort;
            }
            catch
            {
                ServerClassA = "0";
                ServerClassB = "0";
                ServerClassC = "0";
                ServerClassD = "0";

                ClientClassA = "0";
                ClientClassB = "0";
                ClientClassC = "0";
                ClientClassD = "0";
            }
        }

        public bool CanClickOk =>
            !string.IsNullOrEmpty(ServerClassA) &&
            !string.IsNullOrEmpty(ServerClassB) &&
            !string.IsNullOrEmpty(ServerClassC) &&
            !string.IsNullOrEmpty(ServerClassD) &&
            (ServerPort != 0);

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public string ServerClassA
        {
            get { return _serverClassA; }
            set
            {
                _serverClassA = value;
                NotifyOfPropertyChange(() => ServerClassA);
                NotifyOfPropertyChange(nameof(CanClickOk));

                _tcpClientSetupModel.ServerIp = $"{ServerClassA}.{ServerClassB}.{ServerClassC}.{ServerClassD}";
            }
        }

        public string ServerClassB
        {
            get { return _serverClassB; }
            set
            {
                _serverClassB = value;
                NotifyOfPropertyChange(() => ServerClassB);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ServerIp = $"{ServerClassA}.{ServerClassB}.{ServerClassC}.{ServerClassD}";
            }
        }


        public string ServerClassC
        {
            get { return _serverClassC; }
            set
            {
                _serverClassC = value;
                NotifyOfPropertyChange(() => ServerClassC);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ServerIp = $"{ServerClassA}.{ServerClassB}.{ServerClassC}.{ServerClassD}";
            }
        }


        public string ServerClassD
        {
            get { return _serverClassD; }
            set
            {
                _serverClassD = value;
                NotifyOfPropertyChange(() => ServerClassD);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ServerIp = $"{ServerClassA}.{ServerClassB}.{ServerClassC}.{ServerClassD}";
            }
        }

        public int ServerPort
        {
            get { return _tcpClientSetupModel.ServerPort; }
            set
            {
                _tcpClientSetupModel.ServerPort = value;
                NotifyOfPropertyChange(() => ServerPort);
                NotifyOfPropertyChange(nameof(CanClickOk));
            }
        }

        public int ClientId
        {
            get { return _tcpClientSetupModel.ClientId; }
            set
            {
                _tcpClientSetupModel.ClientId = value;
                NotifyOfPropertyChange(() => ClientId);
            }
        }


        private string _clientClassA;

        public string ClientClassA
        {
            get { return _clientClassA; }
            set
            {
                _clientClassA = value;
                NotifyOfPropertyChange(() => ClientClassA);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ClientIp = $"{ClientClassA}.{ClientClassB}.{ClientClassC}.{ClientClassD}";
            }
        }

        private string _clientClassB;

        public string ClientClassB
        {
            get { return _clientClassB; }
            set
            {
                _clientClassB = value;
                NotifyOfPropertyChange(() => ClientClassB);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ClientIp = $"{ClientClassA}.{ClientClassB}.{ClientClassC}.{ClientClassD}";
            }
        }

        private string _clientClassC;

        public string ClientClassC
        {
            get { return _clientClassC; }
            set
            {
                _clientClassC = value;
                NotifyOfPropertyChange(() => ClientClassC);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ClientIp = $"{ClientClassA}.{ClientClassB}.{ClientClassC}.{ClientClassD}";
            }
        }

        private string _clientClassD;

        public string ClientClassD
        {
            get { return _clientClassD; }
            set
            {
                _clientClassD = value;
                NotifyOfPropertyChange(() => ClientClassD);
                NotifyOfPropertyChange(nameof(CanClickOk));
                _tcpClientSetupModel.ClientIp = $"{ClientClassA}.{ClientClassB}.{ClientClassC}.{ClientClassD}";
            }
        }


        public int ClientPort
        {
            get { return _tcpClientSetupModel.ClientPort; }
            set
            {
                _tcpClientSetupModel.ClientPort = value;
                NotifyOfPropertyChange(() => ClientPort);
            }
        }



        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                NotifyOfPropertyChange(() => IsEnable);
            }
        }



        public bool IsAutoConnect
        {
            get { return _tcpClientSetupModel.IsAutoConnect; }
            set 
            {
                _tcpClientSetupModel.IsAutoConnect = value;
                NotifyOfPropertyChange(() => IsAutoConnect);
            }
        }

        #endregion
        #region - Attributes -
        private string _serverClassA;
        private string _serverClassB;
        private string _serverClassC;
        private string _serverClassD;
        private bool _isEnable;
        private TcpClientSetupModel _tcpClientSetupModel;
        public ClientStatusViewModel ClientStatusViewModel { get; private set; }
        #endregion
    }
}
