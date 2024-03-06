using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Libraries.Tcp.Client.Models;
using Ironwall.Libraries.Tcp.Client.Services;
using Ironwall.Libraries.Tcp.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static Ironwall.Framework.Events.EventHelper;

namespace Ironwall.Libraries.Accounts.Services
{
    public class AccountClientService : TcpClient, IAccountClientService
    {
        #region - Ctors -
        public AccountClientService(TcpClientSetupModel setupModel) : base(setupModel)
        {
        }
        #endregion
        #region - Implementation of Interface -
        public Task Connection(TcpServerModel model)
        {
            return Task.Run(() =>
            {

                SetServerIPEndPoint(model);
                InitSocket();
            });
        }

        public Task Disconnection()
        {
            return Task.Run(() =>
            {
                CloseSocket();
            });
        }

        protected override async void Tick(object sender, ElapsedEventArgs e)
        {

            try
            {
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Tick...{(int)(DateTime.Parse(LoginSessionModel.TimeExpired) - DateTime.Now).TotalSeconds}");


                if (DateTime.Parse(LoginSessionModel.TimeExpired) - DateTime.Now < TimeSpan.FromSeconds(10))
                {
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]Session was expired!");

                    var requestModel = InstanceFactory.Build<KeepAliveRequestModel>();
                    requestModel.Insert(LoginSessionModel.Token);
                    var msg = JsonConvert.SerializeObject(requestModel);

                    await SendRequest(msg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in Tick : {ex.Message}");
            }

        }

        /*public Task KeepAliveRequest(string token, IPEndPoint endPoint = null)
		{
			return Task.Run(() =>
			{
				try
				{
					var requestModel = InstanceFactory.Build<KeepAliveRequestModel>();
					requestModel.Insert(token);
					var msg = JsonConvert.SerializeObject(requestModel);

					SendRequest(msg);
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Raised Exception in KeepAliveRequest : {ex.Message}");
				}
			});
		}*/

        public Task KeepAliveResponse(KeepAliveResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    LoginSessionModel.TimeExpired = response?.TimeExpired;
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in KeepAliveResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> LoginResponse(LoginResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    var loginSessionModel = InstanceFactory.Build<LoginSessionModel>();
                    var userModel = InstanceFactory.Build<UserModel>();
                    userModel.Insert(response?.Results?.Details);
                    loginSessionModel.Insert(
                        response?.Results?.UserId,
                        userModel?.Password,
                        response?.Results?.Token,
                        response?.Results?.TimeCreated,
                        response?.Results?.TimeExpired
                        );
                    SessionTimeOut = (int)response?.Results?.SessionTimeOut;
                    UserModel = userModel;
                    LoginSessionModel = loginSessionModel;
                    SetTimerStart();
                    CallRefresh?.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in Login : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> LogoutResponse(LogoutResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;

                    UserModel = null;
                    LoginSessionModel = null;
                    SetTimerStop();
                    InitTimer();
                    CallRefresh?.Invoke();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in Login : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> RegisterResponse(AccountRegisterResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!response.Success)
                        return false;


                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in RegisterResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> EditResponse(AccountInfoResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
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
                    Debug.WriteLine($"Raised Exception in RegisterResponse : {ex.Message}");
                    return false;
                }
            });
        }

        public Task<bool> DeleteResponse(AccountDeleteResponseModel response, IPEndPoint endPoint)
        {
            return Task.Run(() =>
            {
                try
                {
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
                    Debug.WriteLine($"Raised Exception in RegisterResponse : {ex.Message}");
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
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public UserModel UserModel { get; set; }
        public LoginSessionModel LoginSessionModel { get; set; }
        public int SessionTimeOut { get; private set; }

        #endregion
        #region - Attributes -
        public event EventDelegate CallRefresh;
        #endregion

    }
}
