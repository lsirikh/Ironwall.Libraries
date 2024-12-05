using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Vms;
using Ironwall.Libraries.Apis.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Enums;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sensorway.Accounts.Base.Models;
using Sensorway.Events.Base.Models;
using Sensorway.VMS.Messages.Models.Base;
using Sensorway.VMS.Messages.Models.Communications.Accounts;
using Sensorway.VMS.Messages.Models.Communications.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Libraries.VMS.Common.Services
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/4/2024 11:42:03 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    internal class VmsApiService : TimerService, IVmsApiService
    {
        #region - Ctors -
        public VmsApiService(ILogService log
                            , IApiService apiService
                            , IVmsDbService dbService
                            , LoginSessionModel sessionModel
                            , VmsEventProvider eventProvider
                            )
        {
            _log = log;
            _dbService = dbService;
            _apiService = apiService;
            _sessionModel = sessionModel;
            _eventProvider = eventProvider;
            _settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
        }
        #endregion
        #region - Implementation of Interface -
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            try
            {
                //01.Login
                InitTimer(1000 * 60 * 10);//10분마다 Tick 발생

                //await ApiLoginProcess();

                // ApiLoginProcess를 백그라운드 Task로 실행
                _ = Task.Run(async () => await ApiLoginProcess(), token);

            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ExecuteAsync)} of {nameof(VmsApiService)} : {ex.Message}");
            }
            //return Task.CompletedTask;

        }



        public async Task StopAsync(CancellationToken token = default)
        {
            await ApiLogoutProcess();
        }

        #endregion
        #region - Overrides -
        protected override async void Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                _log.Info("Monitoring Server was tried to refresh Vms Api session token.");
                await ApiKeepAliveProcess();
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task ApiLoginProcess()
        {
            try
            {
                int processTry = 0;

                while (Status != EnumVmsStatus.LOGIN)
                {
                    try
                    {
                        string json = null;
                        if (processTry > 1)
                            json = await ApiLogin(isForced: true);
                        else
                            json = await ApiLogin(isForced: false);

                        var response = JsonConvert.DeserializeObject<LoginUserResponse>(json, _settings);
                        if (response == null || response.Success != true) throw new Exception($"Vms Api {nameof(LoginUserResponse)} was failed for the reason({response.Message})");
                        var user = response.Body;
                        UpdateSessionModel(user);

                        _log.Info(json);

                        Status = EnumVmsStatus.LOGIN;

                        await ApiGetEventListProcess();
                        SetTimerStart();
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to execute {nameof(ApiLoginProcess)}(Try:{processTry++}) : {ex.Message} ");
                        await Task.Delay(TIMEDELAY_MS);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }



        public async Task ApiLogoutProcess()
        {
            try
            {
                int processTry = 0;

                while (Status != EnumVmsStatus.LOGOUT)
                {
                    try
                    {
                        string json = await ApiLogout(_sessionModel);
                        var response = JsonConvert.DeserializeObject<LogoutUserResponse>(json, _settings);
                        if (response == null || response.Success != true) throw new Exception($"Vms Api {nameof(LogoutUserResponse)} was failed for the reason({response.Message})");
                        UpdateSessionModel(null);
                        _log.Info(json);

                        Status = EnumVmsStatus.LOGOUT;
                        SetTimerStop();
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to execute {nameof(ApiLogoutProcess)}(Try:{processTry++}) : {ex.Message} ");
                        await Task.Delay(TIMEDELAY_MS);

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ApiKeepAliveProcess()
        {
            try
            {
                int processTry = 0;
                bool isSuccess = false;
                while (!isSuccess)
                {
                    try
                    {
                        var json = await ApiKeepAlive(_sessionModel);

                        var response = JsonConvert.DeserializeObject<KeepAliveUserResponse>(json, _settings);
                        if (response == null || response.Success != true) throw new Exception($"Vms Api KeepAlive was failed for the reason({response.Message})");
                        var user = response.Body;
                        UpdateSessionModel(user);
                        _log.Info(json);

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to execute {nameof(ApiLogoutProcess)}(Try:{processTry++}) : {ex.Message} ");
                        await Task.Delay(TIMEDELAY_MS);

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ApiGetEventListProcess()
        {
            try
            {
                int processTry = 0;
                bool isSuccess = false;
                while (!isSuccess)
                {
                    try
                    {
                        var json = await ApiGetEventList(_sessionModel);

                        var response = JsonConvert.DeserializeObject<GetEventListResponse>(json, _settings);
                        if (response == null || response.Success != true) throw new Exception($"Vms Api {nameof(GetEventListResponse)} was failed for the reason({response.Message})");

                        foreach (var item in response?.Body)
                        {

                            _eventProvider.Add(item);
                        }

                        _log.Info(json);

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to execute {nameof(ApiLogoutProcess)}(Try:{processTry++}) : {ex.Message} ");
                        await Task.Delay(TIMEDELAY_MS);

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ApiSetActionEventProcess(IActionEventModel emodel)
        {
            try
            {
                int processTry = 0;
                bool isSuccess = false;
                while (!isSuccess)
                {
                    try
                    {
                        var json = await ApiSetActionEvent(_sessionModel, emodel);

                        var response = JsonConvert.DeserializeObject<ActEventResponse>(json, _settings);
                        if (response == null || response.Success != true) throw new Exception($"Vms Api {nameof(GetEventListResponse)} was failed for the reason({response.Message})");

                        _log.Info(json);

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to execute {nameof(ApiLogoutProcess)}(Try:{processTry++}) : {ex.Message} ");
                        await Task.Delay(TIMEDELAY_MS);

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UpdateSessionModel(LoginSessionModel user = default)
        {
            try
            {
                _sessionModel.Id = user != null ? user.Id : 0;
                _sessionModel.ClientId = user != null ? user.ClientId : 0;
                _sessionModel.Token = user != null ? user.Token : null;
                _sessionModel.Mode = user != null ? user.Mode : 0;
                _sessionModel.Username = user != null ? user.Username : null;
                _sessionModel.Password = user != null ? user.Password : null;
                _sessionModel.Created = user != null ? user.Created : DateTime.MinValue;
                _sessionModel.Expired = user != null ? user.Expired : DateTime.MinValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> ApiLogin(IVmsApiModel model = default, bool isForced = false, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                var url = "api/login";
                //var loginUserModel = new LoginUserModel(0, model.Username, model.Password, 0, DateTime.Now);
                var loginUserModel = new LoginUserModel(0, "sensorway", "sensorway6", 0, DateTime.Now);

                //var requestModel = new LoginUserRequest(loginUserModel);
                var requestModel = new LoginUserRequest(loginUserModel, isForced: isForced);
                var body = JsonConvert.SerializeObject(requestModel);

                var response = await _apiService.PostRequest(body, url);

                return await ResponseProcess(response, tcs);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> ApiLogout(ILoginSessionModel model, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                var url = "api/logout";
                var requestModel = new LogoutUserRequest(model as LoginSessionModel);
                var body = JsonConvert.SerializeObject(requestModel);

                var response = await _apiService.PostRequest(body, url);

                return await ResponseProcess(response, tcs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ApiKeepAlive(ILoginSessionModel model, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                var url = "api/keep-alive";

                var requestModel = new KeepAliveUserRequest(model.Token);
                var body = JsonConvert.SerializeObject(requestModel, _settings);

                var response = await _apiService.PostRequest(body, url);

                return await ResponseProcess(response, tcs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ApiGetEventList(ILoginSessionModel model, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                var url = "api/get-event-list";

                var requestModel = new GetEventListRequest(model.Token);
                var body = JsonConvert.SerializeObject(requestModel, _settings);

                var response = await _apiService.PostRequest(body, url);

                return await ResponseProcess(response, tcs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> ApiSetActionEvent(ILoginSessionModel model, IActionEventModel emodel, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                var url = "api/insert-actionEvent";
                var actionEvent = new ActionEventModel(emodel);
                var requestModel = new ActEventRequest(actionEvent, model.Token);
                var body = JsonConvert.SerializeObject(requestModel, _settings);

                var response = await _apiService.PostRequest(body, url);

                return await ResponseProcess(response, tcs);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> ResponseProcess(HttpResponseMessage response, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (response == null) throw new Exception($"Api response was null.");
                string body = null;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    response.EnsureSuccessStatusCode();
                    body = await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _log?.Info($"Api account was not authorized![{response?.ReasonPhrase}]");
                    body = await response?.Content?.ReadAsStringAsync();

                    tcs?.SetResult(false);
                    return body;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    _log?.Info($"Api Connection was failure![{response?.ReasonPhrase}]");
                    body = await response?.Content?.ReadAsStringAsync();

                    tcs?.SetResult(false);
                    return body;
                }
                else
                {
                    tcs?.SetResult(false);
                    return body;
                }

                tcs?.SetResult(true);
                return body;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public EnumVmsStatus Status { get; private set; }
        #endregion
        #region - Attributes -
        private ILogService _log;
        private IVmsDbService _dbService;
        private IApiService _apiService;
        private LoginSessionModel _sessionModel;
        private VmsEventProvider _eventProvider;
        private JsonSerializerSettings _settings;
        private const int TIMEDELAY_MS = 3000;
        #endregion

    }
}