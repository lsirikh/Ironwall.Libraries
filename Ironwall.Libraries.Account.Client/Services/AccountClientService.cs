using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.DataProviders;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Tcp.Client.Services;
using Ironwall.Libraries.Tcp.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Ironwall.Framework.Events.EventHelper;

namespace Ironwall.Libraries.Account.Client.Services
{
    public abstract class AccountClientService : TcpClient, IAccountClientService
    {
        #region - Ctors -
        /// <summary>
        /// Initializes the AccountClientService with logging and TCP setup model.
        /// </summary>
        protected AccountClientService(ILogService log, TcpSetupModel tcpSetupModel): base(log, tcpSetupModel)
        {
            _class = typeof(AccountClientService);
        }
        #endregion
        #region - Implementation of Interface -
        /// <summary>
        /// Periodically checks the session's validity and sends a keep-alive request if needed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SessionTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                if ((LoginSessionModel?.TimeExpired - DateTime.Now) < TimeSpan.FromSeconds(30))
                {
                    _log.Info($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Request Session Updated...");

                    var requestModel = RequestFactory.Build<KeepAliveRequestModel>(LoginSessionModel.Token);
                    var msg = JsonConvert.SerializeObject(requestModel, _settings);

                    await SendRequest(msg).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in Tick : {ex.Message}");
                SetSessionTimerStop();
                InitSessionTimer();
                CallRefresh?.Invoke();
            }
        }

        /// <summary>
        /// Sends a login request to the server.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task LoginRequest(ILoginRequestModel requestModel)
        {
            try
            {
                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(LoginRequest)} : {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the login response from the server.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Task<bool> LoginResponse(ILoginResponseModel response, IPEndPoint endPoint)
        {
            try
            {
                if (!response.Success) return Task.FromResult(false);

                SessionTimeOut = (int)response?.Results?.SessionTimeOut;
                UserModel = ModelFactory.Build<UserModel>(response.Results.Details);
                LoginSessionModel = ModelFactory.Build<LoginSessionModel>(response.Results);
                InitSessionTimer();
                SetSessionTimerStart();
                CallRefresh?.Invoke();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in Login : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public Task CheckIdRequest(IAccountIdCheckRequestModel requestModel)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var msg = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CheckIdRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task CheckIdResponse(IResponseModel requestModel, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!requestModel.Success)
                        return false;


                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CheckIdResponse)} : {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Sends a keep-alive request to the server.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public async Task KeepAliveRequest(IKeepAliveRequestModel requestModel)
        {
            try
            {
                if (LoginSessionModel == null) return;

                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(KeepAliveRequest)} : {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the keep-alive response from the server.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public Task<bool> KeepAliveResponse(IKeepAliveResponseModel response, IPEndPoint endPoint)
        {
            try
            {
                if (LoginSessionModel == null) return Task.FromResult(false);

                if (response == null || !response.Success)
                {
                    SetSessionTimerStop();
                    InitSessionTimer();
                    CallRefresh?.Invoke();
                    return Task.FromResult(false);
                }

                LoginSessionModel.TimeExpired = response.TimeExpired;
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(KeepAliveResponse)} : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task LogoutRequest(ILogoutRequestModel requestModel)
        {
            try
            {
                if (LoginSessionModel == null) return;

                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg).ConfigureAwait(false);

                CallRefresh?.Invoke();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(LogoutRequest)} : {ex.Message}");
            }
        }

        public Task<bool> LogoutResponse(ILogoutResponseModel response, IPEndPoint endPoint)
        {
            try
            {
                if (LoginSessionModel == null) return Task.FromResult(false);

                if (!response.Success) return Task.FromResult(false);

                UserModel = null;
                LoginSessionModel = null;
                SetSessionTimerStop();
                InitSessionTimer();
                CallRefresh?.Invoke();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in Login : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task RegisterRequest(IAccountRegisterRequestModel requestModel)
        {
            try
            {
                //if (LoginSessionModel == null)
                //    return false;

                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(RegisterRequest)} : {ex.Message}");
            }
        }

        public Task<bool> RegisterResponse(IAccountRegisterResponseModel response, IPEndPoint endPoint)
        {
            try
            {
                if (!response.Success) return Task.FromResult(false);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in RegisterResponse : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task AccountAllRequest(IAccountAllRequestModel requestModel)
        {
            try
            {
                if (LoginSessionModel == null) return;

                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(AccountAllRequest)} : {ex.Message}");
            }
        }

        public Task<bool> AccountAllResponse(IAccountAllResponseModel response, IPEndPoint endPoint)
        {
            try
            {
                if (LoginSessionModel == null)
                    return Task.FromResult(false);

                if (!response.Success)
                    return Task.FromResult(false);


                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in RegisterResponse : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task AccountInfoRequest(IAccountInfoRequestModel requestModel)
        {
            try
            {
                if (LoginSessionModel == null) return;

                var msg = JsonConvert.SerializeObject(requestModel, _settings);
                await SendRequest(msg).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(AccountInfoRequest)} : {ex.Message}");
            }
        }

        public Task<bool> AccountInfoResponse(IAccountInfoResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!response.Success)
                        return false;

                    var userModel = ModelFactory.Build<UserModel>(response);

                    UserModel = userModel;
                    CallRefresh?.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in RegisterResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task EditRequest(IAccountEditRequestModel requestModel)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    var msg = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(EditRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> EditResponse(IAccountInfoResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!response.Success)
                        return false;

                    var userModel = InstanceFactory.Build<UserModel>();
                    userModel.Insert(response?.Details);

                    UserModel = userModel;
                    CallRefresh?.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in RegisterResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task DeleteRequest(IAccountDeleteRequestModel requestModel)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    var msg = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(DeleteRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeleteResponse(IAccountDeleteResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    if (!response.Success)
                        return false;

                    UserModel = null;
                    LoginSessionModel = null;

                    SocketReceived($"{response.Message}", endPoint);
                    CallRefresh?.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in RegisterResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task AccountDeleteAllRequest(IAccountDeleteAllRequestModel requestModel)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (LoginSessionModel == null)
                        return false;

                    var msg = JsonConvert.SerializeObject(requestModel, _settings);
                    await SendRequest(msg);
                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(AccountDeleteAllRequest)} : {ex.Message}");
                    return false;
                }
            });
        }

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void InitSessionTimer()
        {
            if (SessionTimer != null)
                SessionTimer.Close();

            SessionTimer = new System.Timers.Timer();
            SessionTimer.Elapsed -= SessionTick;
            SessionTimer.Elapsed += SessionTick;

            SetSessionTimerInterval(5000);
        }

        public void SetSessionTimerEnable(bool value)
        {
            SessionTimer.Enabled = value;
        }
        public bool GetSessionTimerEnable() => SessionTimer.Enabled;
        public void SetSessionTimerInterval(int time = 1000)
        {
            SessionTimer.Interval = time;
        }
        public double GetSessionTimerInterval() => SessionTimer.Interval;
        public void SetSessionTimerStart()
        {
            SetTimerEnable(true);
            SessionTimer.Start();
        }
        public void SetSessionTimerStop()
        {
            SessionTimer.Stop();
        }
        public void DisposeSessionTimer()
        {
            SessionTimer.Elapsed -= SessionTick;

            if (GetTimerEnable())
                SessionTimer.Stop();

            if (SessionTimer != null)
                SessionTimer.Close();

            SessionTimer.Dispose();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public UserModel UserModel { get; set; }
        public LoginSessionModel LoginSessionModel { get; set; }
        public int SessionTimeOut { get; set; }
        #endregion
        #region - Attributes -
        public event EventDelegate CallRefresh;
        private System.Timers.Timer SessionTimer;
        private Type _class;
        #endregion

    }
}
