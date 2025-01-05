using Ironwall.Libraries.Tcp.Common.Defines;
using Ironwall.Libraries.Tcp.Common.Models;
using Ironwall.Libraries.Tcp.Common.Proivders;
using Ironwall.Libraries.Tcp.Server.Services;
using Ironwall.Framework.DataProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.ObjectModel;
using Ironwall.Libraries.Utils.Exceptions;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Libraries.Account.Common.Providers;
using Ironwall.Libraries.Account.Common.Models;
using Ironwall.Libraries.Account.Common.Services;
using Ironwall.Framework.Models;
using static Dapper.SqlMapper;
using Newtonsoft.Json.Linq;
using Ironwall.Libraries.Tcp.Common.Proivders.Models;
using Ironwall.Libraries.Account.Common.Helpers;
using Ironwall.Libraries.Common.Providers;
using Ironwall.Libraries.Tcp.Server.Models;
using Ironwall.Libraries.Base.DataProviders;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Newtonsoft.Json.Converters;

namespace Ironwall.Libraries.Account.Server.Services
{
    public abstract class AccountServerService : TcpServer, IAccountServerService
    {
        #region - Ctors -
        public AccountServerService(
                                    ILogService log
                                    , AccountSetupModel accountSetupModel
                                    , TcpSetupModel tcpSetupModel
                                    , TcpServerSetupModel tcpServerSetupModel
                                    , SessionProvider sessionProvider
                                    , TcpClientProvider tcpClientProvider
                                    , LoginProvider loginProvider
                                    , UserProvider userProvider
                                    , LoginUserProvider loginUserProvider
                                    , AccountDbService accountDbService
                                    , TcpUserProvider tcpUserProvider
                                    , LogProvider logProvider 
                                    )
                                    : base(log, tcpSetupModel, tcpServerSetupModel, logProvider)
        {

            AccountSetupModel = accountSetupModel;
            SessionProvider = sessionProvider;
            LoginProvider = loginProvider;
            UserProvider = userProvider;
            TcpClientProvider = tcpClientProvider;

            TcpUserProvider = tcpUserProvider;


            AccountDbService = accountDbService;
            LoginUserProvider = loginUserProvider;
            LoginSessionUsers = new ObservableCollection<UserModel>();
            Sessions = new Dictionary<string, ILoginSessionModel>();
            TimeTags = new Dictionary<string, object>();

            _locker = new ();
            _settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.ff"
            };
        }
        #endregion
        #region - Implementation of Interface -
        public Task InitializeAccountService()
        {
            return Task.Run(() =>
            {
                InitSessionTimer();
                SetSessionTimerStart();
            });
        }

        public Task DeinitializeAccountService()
        {
            return Task.Run(() =>
            {
                SetSessionTimerStop();
                InitSessionTimer();
            });
        }

        public Task<bool> Login(ILoginRequestModel model, IPEndPoint endPoint = null)
        {
            ///Login Process
            ///01. Check Whether User is valid or not
            ///02. Check Pre-login condition
            ///03. LoginSessionModel Generation
            ///04. Save SessionModel
            ///05. LoginUserModel Generation
            ///06. Save Login Information
            ///07. Insert Session Dictionary
            ///08. Add UserModel for login account
            ///09. Send Response to Client
            ///10. Send UI log message
            ///11. Login Event trriger
            return Task.Run(async () =>
            {
                try
                {
                    //[1]Check Validation
                    var userAccount = UserProvider.Where(entity => (entity as IUserModel).IdUser == model.UserId && (entity as IUserModel).Password == model.UserPass).FirstOrDefault() as IUserModel;

                    if (userAccount == null)
                        throw new Exception(message: "There is not matched account for login");

                    if (!userAccount.Used)
                        throw new Exception(message: "This account was not allowed to use.");

                    //[2]Check whether the loggin user was already login or not
                    if (await IsLoggedInUser(model))
                    {
                        //Is forceful login mode
                        if (!model.IsForceLogin)
                        {
                            //The user is already loginned!
                            throw new Exception(message: $"{model.UserId} is already loginned!");
                        }
                        else
                        {
                            //Logout model
                            //var accountModel = _sessionProvider.Where(t => t.UserId == model.UserId).FirstOrDefault();
                            var accountModel = GetLoginSessionModel(model.UserId, model.UserPass);

                            //Create LogoutRequestModel instance
                            //var logoutRequestModel = InstanceFactory.Build<LogoutRequestModel>();
                            //Insert data
                            //logoutRequestModel.Insert((int)EnumCmdType.LOGOUT_REQUEST_FORCE_LOGIN, accountModel.UserId, accountModel.Token);

                            //Create LogoutRequestModel instance
                            var logoutRequestModel = RequestFactory.Build<LogoutRequestModel>(accountModel.UserId, accountModel.Token);
                            //Execute Logout process
                            await Logout(logoutRequestModel);
                        }
                    }

                    //[3]Create LoginSession instance
                    LoginSessionModel loginSessionModel = await GenerateLoginSessionModel(userAccount);

                    //[4]Save LoginSessionModel in DB
                    await AccountDbService.SaveSession(loginSessionModel, 1);

                    //[5]Create LoginUserModel instance
                    LoginUserModel loginAccountModel = GenerateLoginUserModel(userAccount, loginSessionModel, endPoint);
                    //[6]Save LoginAccountModel in DB
                    await AccountDbService.SaveLogin(loginAccountModel);

                    //[7]Insert new session dictionary
                    AddLoginSessionModel(loginSessionModel);

                    //[8]Add UserModel instance for login account
                    await LoginUserProvider.InsertedItem(userAccount);

                    //[9]Login Success Response
                    await SendLoginResponse(true, "Successfully Login!", endPoint, loginSessionModel, loginAccountModel);

                    //[10]Send UI log message
                    AcceptedClient_Event($"{userAccount.IdUser} was login!", endPoint);

                    //[11]Login Event trriger
                    ClientLogin(userAccount.IdUser, endPoint);


                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.UserId})Raised SocketSendException in Login : {ex.Message}");
                    AcceptedClient_Event($"({model.UserId})Raised SocketSendException in Login : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised an exception in Login : {ex.Message}");
                    await SendLoginResponse(false, $"{ex.Message}", endPoint);
                    return false;
                }
            });
        }

        public Task<bool> KeepAlive(IKeepAliveRequestModel model, IPEndPoint endPoint = null)
        {
            ///KeepAlive Process
            ///1. Update LoginSessionModel instance
            ///2. Save Session
            return Task.Run(async () =>
            {
                try
                {
                    //Update LoginSessionModel instance
                    var sessionModel = UpdateLoginSessionModel(model);

                    //Save LoginSessionModel in DB
                    await AccountDbService.SaveSession(sessionModel, 2);

                    //Remove from Session Dictionary
                    RemoveLoginSessionModel(sessionModel);

                    //Add Session Dictionary
                    AddLoginSessionModel(sessionModel);

                    //Response message to the Client
                    await SendKeepAliveResponse(true, $"{sessionModel.UserId} was successfully extend the expiration time", sessionModel.TimeExpired, endPoint);
                    //
                    AcceptedClient_Event($"{sessionModel.UserId} was successfully extend the expiration time", endPoint);
                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in Logout : {ex.Message}");
                    AcceptedClient_Event($"Raised SocketSendException in Logout : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    _log.Error($"Raised NullReferenceException in KeepAlive : {ex.Message}");
                    //Response message to the Client
                    await SendKeepAliveResponse(false, "token was not matched!", DateTime.Now, endPoint);
                    AcceptedClient_Event("token was not matched!", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in KeepAlive : {ex.Message}");
                    //Response message to the Client
                    await SendKeepAliveResponse(false, $"Raised Exception in KeepAlive : {ex.Message}", DateTime.Now, endPoint);
                    AcceptedClient_Event("token was not matched!", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                    return false;
                }
            });
        }

        public Task<bool> Logout(ILogoutRequestModel model = null, IPEndPoint endPoint = null)
        {
            ///Logout Process
            ///01. Get LoginSessionModel instance
            ///02. Save LoginSessionModel in DB
            ///03. Remove data from Session Dictionary
            ///04. Remove data from LoginUserProvider
            ///05. Get LoginAccountModel instance
            ///06. Save Login Information
            ///07. Get IPEndPoint instance
            ///08. Send response message to client
            ///09. Send UI log message
            ///10. Logout Event trigger
            return Task.Run(async () =>
            {
                try
                {
                    //Logout the designated client
                    if (model != null)
                    {
                        //[1]Get LoginSessionModel instance which was matched with LogoutRequestModel.Token
                        _log.Info($"Logout Process was started!");
                        _log.Info($"Model ==> Id : {model.UserId}, token : {model.Token}");

                        var sessionModel = GetLoginSessionModel(model.Token);
                        if(sessionModel == null)
                            throw new NullReferenceException($"Searched token({model.Token}) was not found in Session!");

                        //[2]Save LoginSessionModel in DB
                        await AccountDbService.SaveSession(sessionModel);

                        //[3]Remove data from Session Dictionary
                        RemoveLoginSessionModel(sessionModel);

                        //[4]Remove data from LoginUserProvider
                        RemoveLoginUser(model.UserId);

                        //[5]Get LoginUserModel(is different from UserModel) to save DB for login record 
                        LoginUserModel loginAccount = GetLoginUserModel(model.UserId);

                        //[6]Save LoginAccountModel in DB
                        await AccountDbService.SaveLogin(loginAccount);
                        _log.Info($"Logout : {loginAccount}");

                        //[7]Get IPEndPoint instance
                        if (endPoint == null)
                            endPoint = GetIPEndPoint(model.UserId);

                        //[8]Send response message to client
                        await SendLogoutResponse(true, "Successfully logout!", endPoint);

                        //[9]UI Message Show
                        AcceptedClient_Event($"{model.UserId} was logout!", endPoint);

                        //[10]Logout Event
                        ClientLogout?.Invoke(model.UserId, endPoint);
                    }
                    else
                    {
                        //Logout all sessions 
                        foreach (ILoginSessionModel sessionModel in SessionProvider.ToList())
                        {
                            //[2]Save LoginSessionModel in DB for Logout
                            await AccountDbService.SaveSession(sessionModel);

                            //[3]Remove data from Session Dictionary
                            RemoveLoginSessionModel(sessionModel);

                            //[4]Remove data from LoginUserProvider
                            RemoveLoginUser(sessionModel.UserId);

                            //[5]Get LoginAccountModel instance
                            LoginUserModel loginAccount = GetLoginUserModel(sessionModel.UserId);

                            //[6]Save LoginAccountModel in DB
                            await AccountDbService.SaveLogin(loginAccount);

                            //[7]Get IPEndPoint instance
                            IPEndPoint clientEndPoint = GetIPEndPoint(loginAccount.ClientId);

                            //Send response message to client
                            await SendLogoutResponse(true, "Successfully logout!", clientEndPoint);

                            //Send Ui Message
                            AcceptedClient_Event($"{sessionModel.UserId} was logout!", clientEndPoint);
                            //Logout Event
                            ClientLogout?.Invoke(sessionModel.UserId, clientEndPoint);
                        }
                    }
                    return true;
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"Raised SocketSendException in Logout : {ex.Message}");
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    _log.Error($"Raised NullReferenceException in Logout : {ex.Message}");
                    //await SendLogoutResponse(false, "Token was not matched!", endPoint);
                    await SendLogoutResponse(false, ex.Message, endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in Logout : {ex.Message}");
                    await SendLogoutResponse(false, $"Raised Exception in Logout : {ex.Message}", endPoint);
                    return false;
                }
            });
        }

        public Task CheckIdAccount(IAccountIdCheckRequestModel model, IPEndPoint endPoint = null)
        {
            ///Logout Process
            ///01. Get UserId
            ///02. Fetch UserInstance in DB
            ///03. Send response message to client
            ///05. Send UI log message
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Delete data is empty.");

                    var FetchedUser = await AccountDbService.FetchUserId(model.IdUser);

                    //03. Send Register Result Message to Client as Response
                    if (FetchedUser != null)
                    {
                        await SendCheckIdResponse(true, $"{model.IdUser} is already registered!", endPoint);
                    }
                    else
                    {
                        await SendCheckIdResponse(false, $"{model.IdUser} was not registered!", endPoint);
                    }

                    //08. UI Communication
                    AcceptedClient_Event($"Check Id({model.IdUser})  from DB, before register Account!", endPoint);
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.IdUser}) Raised SocketSendException in {nameof(CheckIdAccount)} : {ex.Message}");
                    AcceptedClient_Event($"({model.IdUser}) Raised SocketSendException in {nameof(CheckIdAccount)} : {ex.Message}", endPoint);
                }
                catch (Exception ex)
                {
                    _log.Error($"({model.IdUser}) Raised Exception in {nameof(CheckIdAccount)} : {ex.Message}");
                    await SendRegisterResponse(false, $"({model.IdUser}) Raised Exception in {nameof(CheckIdAccount)} : {ex.Message}", null, endPoint);
                }
            });
        }

        public Task RegisterAccount(IAccountRegisterRequestModel model, IPEndPoint endPoint = null)
        {
            ///Register Account Process
            ///01. Check Pre-registered Id
            ///02. Password Length Check
            ///03. Generate UserModel from requestModel
            ///04. Account Register in DB
            ///05. Account Instance Register in Provider
            ///06. Fetch User from DB
            ///07. Send Register Result Message to Client as Response
            ///08. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new UserRegisterException(message: "Register data is empty.");

                    //01. Check Pre-registered Id
                    if (await IsRegisteredUser(model.IdUser))
                        throw new UserRegisterException(message: $"{model.IdUser} is already registered.");
                    //02. Password Length Check
                    if (model.Password.Length <= 8)
                        throw new Exception(message: "Password is too short.");
                    //03. Generate UserModel from requestModel
                    IUserModel userModel = ModelFactory.Build<UserModel>(model);
                    //04. Account Register in DB
                    //05. Account Instance Register in Provider
                    //06. Fetch User from DB
                    userModel = await AccountDbService.SaveUser(userModel);
                    if (endPoint == null)
                    {
                        var loginAccount = GetLoginUserModel(model.IdUser);
                        endPoint = GetIPEndPoint(loginAccount.ClientId);
                    }
                    //07. Send Register Result Message to Client as Response
                    await SendRegisterResponse(true, $"{userModel.IdUser} was successfully registered!", userModel, endPoint);
                    //08. UI Communication
                    AcceptedClient_Event($"{userModel.IdUser} was successfully registered!", endPoint);
                }
                catch (UserRegisterException ex)
                {
                    _log.Error($"{ex.Message}");
                    AcceptedClient_Event(ex.Message, endPoint);
                    await SendRegisterResponse(false, ex.Message, null, endPoint);
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.IdUser}) Raised SocketSendException in {nameof(RegisterAccount)} : {ex.Message}");
                    AcceptedClient_Event($"({model.IdUser}) Raised SocketSendException in {nameof(RegisterAccount)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"({model.IdUser}) Raised Exception in {nameof(RegisterAccount)} : {ex.Message}");
                    await SendRegisterResponse(false, $"({model.IdUser}) Raised Exception in {nameof(RegisterAccount)} : {ex.Message}", null, endPoint);
                }
            });
        }

        public Task DeleteAccount(AccountDeleteRequestModel model, IPEndPoint endPoint = null)
        {
            ///Delete Account Process
            ///01. Check Id and Password
            ///02. Check LoginSession from SessionProvider
            ///03. If Included in SessioProvider, then Logout
            ///04. Delete Account Record from DB
            ///05. Remove UserModel Instance from UserProvider
            ///06. Send Register Result Message to Client as Response
            ///07. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Delete data is empty.");
                    //01.Check Id and Password
                    var userModel = IsValidUser(model.UserId, model.UserPass);
                    if (userModel == null)
                        throw new Exception(message: "Account Id or Password is not correct!");
                    //02.Check LoginSession from SessionProvider
                    var loginSessionModel = GetLoginSessionModel(model.Token);
                    //03. If Included in SessioProvider, then Logout
                    if (loginSessionModel != null)
                    {
                        //Create LogoutRequestModel instance
                        var logoutRequestModel = InstanceFactory.Build<LogoutRequestModel>();
                        //Insert data
                        logoutRequestModel.Insert(loginSessionModel.UserId, loginSessionModel.Token);
                        //Logout Process
                        await Logout(logoutRequestModel);
                    }
                    //04. Delete Account Record from DB
                    await AccountDbService.RemoveUser(userModel);
                    //05. Remove UserModel Instance from UserProvider
                    //UserProvider.Remove(userModel);
                    if (endPoint == null)
                    {
                        var loginAccount = GetLoginUserModel(model.UserId);
                        endPoint = GetIPEndPoint(loginAccount.ClientId);
                    }
                    //06. Send Register Result Message to Client as Response
                    await SendDeleteAccountResponse(true, $"{userModel.IdUser} was successfully deleted!", endPoint);
                    //07. UI Communication
                    AcceptedClient_Event($"{userModel.IdUser} was successfully deleted!", endPoint);

                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}");
                    AcceptedClient_Event($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in DeleteAccount : {ex.Message}");
                    await SendDeleteAccountResponse(false, ex.Message, endPoint);
                    AcceptedClient_Event($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task DeleteAllAccount(AccountDeleteAllRequestModel model, IPEndPoint endPoint = null)
        {
            ///Delete Account Process
            ///01. Check Id and Password
            ///02. Check LoginSession from SessionProvider
            ///03. If Included in SessioProvider, then Logout
            ///04. Delete Account Record from DB
            ///05. Remove UserModel Instance from UserProvider
            ///06. Send Register Result Message to Client as Response
            ///07. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Delete data is empty.");
                    //01.Check Id and Password for admin
                    var userModel = IsValidUser(model.UserId, model.UserPass);
                    if (userModel == null)
                        throw new Exception(message: "This Account Authority for DeleteAll is not correct, please check Id or Password!");

                    //02.Check LoginSession from SessionProvider
                    var adminSession = GetLoginSessionModel(model.Token);

                    //03. If Included in SessioProvider, then Logout
                    if (adminSession == null)
                        throw new Exception(message: "This Account Authority for DeleteAll has incorrect token, please check again!");


                    List<string> deletedAccount = new List<string>();

                    foreach (var item in model.UserList)
                    {
                        //get IPEndPoint from ClientPC, if Login Account
                        var eachEndPoint = GetIPEndPoint(item.IdUser);
                        if(eachEndPoint != null)
                        {
                            ///get Session information from AccountDetailModel
                            var itemSession = GetAccountSessionModel(item.IdUser, item.Password);

                            ///if item is login, logout first
                            var logoutRequestModel = RequestFactory.Build<LogoutRequestModel>(item.IdUser, itemSession.Token);
                            await Logout(logoutRequestModel);
                        }

                        var itemUserModel = IsValidUser(item.IdUser, item.Password);

                        if (itemUserModel == null)
                            continue;

                        //04. Delete Account Record from DB
                        if (await AccountDbService.RemoveUser(itemUserModel))
                            deletedAccount.Add(itemUserModel.IdUser);


                        //06. Send Register Result Message to Client as Response
                        await SendDeleteAccountResponse(true, $"{item.IdUser} was deleted by admin!", eachEndPoint);
                    }

                    await SendDeleteAllAccountResponse(true, $"Reqeust Accounts({deletedAccount.Count}) was successfully deleted!", deletedAccount, endPoint);

                    //07. UI Communication
                    AcceptedClient_Event($"Reqeust Accounts({deletedAccount.Count}) was successfully deleted!", endPoint);

                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}");
                    AcceptedClient_Event($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in DeleteAccount : {ex.Message}");
                    await SendDeleteAccountResponse(false, ex.Message, endPoint);
                    AcceptedClient_Event($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }



        public Task EditAccount(AccountEditRequestModel model, IPEndPoint endPoint = null)
        {
            ///Register Account Process
            ///01. Check Matched Id and Password
            ///02. Generate UserModel from requestModel
            ///03. Account Update in DB
            ///04. Account Instance Find & Edit in UserProvider
            ///05. Fetch User from DB
            ///06. Send Edit Result Message to Client as Response
            ///07. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Edit data is empty.");

                    //01.Check Id and Password
                    if (IsValidUser(model.IdUser, model.Password) == null)
                        throw new Exception(message: "Account Id or Password is not correct!");

                    //02. Generate UserModel from requestModel
                    IUserModel userModel = ModelFactory.Build<UserModel>(model);

                    //03. Account Update in DB
                    //04. Account Instance Find &Edit in UserProvider
                    //05. Fetch User from DB
                    await AccountDbService.EditeUser(userModel);

                    //06. Send Edit Result Message to Client as Response
                    await SendEditRegisterResponse(true, $"{userModel.IdUser} is successfully edited", userModel, endPoint);

                    //07. UI Communication
                    AcceptedClient_Event($"{userModel.IdUser} is successfully edited", endPoint);
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.IdUser}) Raised SocketSendException in EditAccount : {ex.Message}");
                    AcceptedClient_Event($"({model.IdUser}) Raised SocketSendException in EditAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"({model.IdUser}) Raised Exception in EditAccount : {ex.Message}");
                    await SendEditRegisterResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"({model.IdUser}) Raised Exception in EditAccount : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task FetchAccount(AccountInfoRequestModel model, IPEndPoint endPoint = null)
        {
            ///Register Account Process
            ///01. Check Matched Id and Password
            ///02. Fetch User from DB
            ///03. Send Edit Result Message to Client as Response
            ///04. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Edit data is empty.");

                    //01.Check Id and Password
                    if (IsValidUser(model.IdUser, model.Password) == null)
                        throw new Exception(message: "Account Id or Password is not correct!");

                    //02. Fetch User from DB
                    var userModel = await AccountDbService.FetchUserId(model.IdUser);

                    //03. Send Fetch Result Message to Client as Response
                    await SendInfoRegisterResponse(true, $"{userModel.IdUser} is successfully fetched", userModel, endPoint);

                    //07. UI Communication
                    AcceptedClient_Event($"{userModel.IdUser} is successfully fetched", endPoint);
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}");
                    AcceptedClient_Event($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}");
                    await SendEditRegisterResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }

        public Task FetchAllAccount(AccountAllRequestModel model, IPEndPoint endPoint = null)
        {
            ///Register Account Process
            ///01. Check Matched Id and Password
            ///02. Fetch User from DB
            ///03. Send Edit Result Message to Client as Response
            ///04. UI Communication
            return Task.Run(async () =>
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Edit data is empty.");

                    //01.Check Id and Password
                    if (IsValidUser(model.IdUser, model.Password) == null)
                        throw new Exception(message: "Account Id or Password is not correct!");

                    //02. Fetch User from DB
                    await AccountDbService.FetchUserAll();

                    //03. Send Fetch Result Message to Client as Response
                    await SendAllAccountResponse(true, $"All accounts were successfully fetched", endPoint);

                    //07. UI Communication
                    AcceptedClient_Event($"All accounts were successfully fetched", endPoint);
                }
                catch (SocketSendException ex)
                {
                    _log.Error($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}");
                    AcceptedClient_Event($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
                catch (Exception ex)
                {
                    _log.Error($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}");
                    await SendEditRegisterResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Event($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}", endPoint, EnumTcpCommunication.COMMUNICATION_ERROR);
                }
            });
        }



        #region TaskResponse Process
        private Task SendLoginResponse(bool success, string msg, IPEndPoint endPoint = null, LoginSessionModel loginSessionModel = null, LoginUserModel loginAccountModel = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    LoginResultModel loginResponseResultModel = null;
                    if (loginSessionModel != null && loginAccountModel != null)
                    {
                        var userModel = UserProvider.Where(entity => (entity as IUserModel).IdUser == loginSessionModel.UserId).FirstOrDefault() as IUserModel;

                        var detailModel = ResponseFactory.Build<AccountDetailModel>(userModel);

                        loginResponseResultModel = new LoginResultModel(loginSessionModel.UserId
                            , loginSessionModel.Token
                            , loginAccountModel.ClientId
                            , loginAccountModel.UserLevel
                            , AccountSetupModel.SessionTimeout
                            , detailModel
                            , loginSessionModel.TimeCreated
                            , loginSessionModel.TimeExpired);
                    }

                    //Create LoginResponseModel instance
                    //Insert data
                    var loginResponseModel = ResponseFactory.Build<LoginResponseModel>(success, msg, loginResponseResultModel);
                    //Send Response message(data)
                    SendRequest(JsonConvert.SerializeObject(loginResponseModel, _settings), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendKeepAliveResponse(bool success, string msg, DateTime timeExpired, IPEndPoint endPoint)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //Create LoginSession instance
                    var keepAliveResponseModel = InstanceFactory.Build<KeepAliveResponseModel>();
                    //Insert data
                    keepAliveResponseModel.Insert(success, msg, timeExpired);
                    //Send Response message(data)
                    await SendRequest(JsonConvert.SerializeObject(keepAliveResponseModel, _settings), endPoint);
                }
                catch (JsonReaderException)
                {

                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendLogoutResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    //Create LogoutResponseModel instance
                    var logoutResponseModel = ResponseFactory.Build<LogoutResponseModel>(success, msg);
                    //Insert data
                    //logoutResponseModel.Insert(success, msg);
                    //Send Response message(data)
                    await SendRequest(JsonConvert.SerializeObject(logoutResponseModel, _settings), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }

            });
        }

        private Task SendCheckIdResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var responseModel = ResponseFactory.Build<AccountIdCheckResponseModel>(success, msg);
                    await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendRegisterResponse(bool success, string msg, IUserModel userModel = null, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (userModel != null)
                    {
                        var detailModel = InstanceFactory.Build<AccountDetailModel>();
                        detailModel.Insert(
                            userModel.Id,
                            userModel.IdUser,
                            userModel.Password,
                            userModel.Name,
                            userModel.EmployeeNumber,
                            userModel.Birth,
                            userModel.Phone,
                            userModel.Address,
                            userModel.EMail,
                            userModel.Image,
                            userModel.Position,
                            userModel.Department,
                            userModel.Company,
                            userModel.Level,
                            userModel.Used
                            );
                        var responseModel = InstanceFactory.Build<AccountRegisterResponseModel>();
                        responseModel.Insert(success, msg, detailModel);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountRegisterResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendEditRegisterResponse(bool success, string msg, IUserModel userModel = null, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (userModel != null)
                    {
                        var detailModel = InstanceFactory.Build<AccountDetailModel>();
                        detailModel.Insert(
                            userModel.Id,
                            userModel.IdUser,
                            userModel.Password,
                            userModel.Name,
                            userModel.EmployeeNumber,
                            userModel.Birth,
                            userModel.Phone,
                            userModel.Address,
                            userModel.EMail,
                            userModel.Image,
                            userModel.Position,
                            userModel.Department,
                            userModel.Company,
                            userModel.Level,
                            userModel.Used
                            );
                        var responseModel = InstanceFactory.Build<AccountEditResponseModel>();
                        responseModel.Insert(success, msg, detailModel);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountEditResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendDeleteAccountResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var responseModel = ResponseFactory.Build<AccountDeleteResponseModel>(success, msg);
                    await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendDeleteAllAccountResponse(bool success, string msg, List<string> deletedAccounts, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var responseModel = ResponseFactory.Build<AccountDeleteAllResponseModel>(success, msg, deletedAccounts);
                    await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendInfoRegisterResponse(bool success, string msg, IUserModel userModel = null, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (userModel != null)
                    {
                        var detailModel = InstanceFactory.Build<AccountDetailModel>();
                        detailModel.Insert(userModel);
                        var responseModel = InstanceFactory.Build<AccountInfoResponseModel>();
                        responseModel.Insert(success, msg, detailModel);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountInfoResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendAllAccountResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var userList = new List<AccountDetailModel>();
                    foreach (IUserModel item in UserProvider.ToList())
                    {
                        var detailModel = ResponseFactory.Build<AccountDetailModel>(item);
                        userList.Add(detailModel);
                    }

                    var responseModel = ResponseFactory.Build<AccountAllResponseModel>(success, msg, userList);

                    await SendRequest(JsonConvert.SerializeObject(responseModel, _settings), endPoint);

                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }
        #endregion

        #region IPEndPoint Process
        private IPEndPoint GetIPEndPoint(int id)
        {
            try
            {
                var tcpClient = TcpClientProvider.Where(t => t.Id == id).FirstOrDefault();

                //Get TcpAcceptedClient instance
                var clientEndPoint = ClientList.Where(t => (t.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() == tcpClient.IpAddress
                && (t.Socket.RemoteEndPoint as IPEndPoint).Port == tcpClient.Port).Select(t => t.Socket.RemoteEndPoint as IPEndPoint).FirstOrDefault();

                return clientEndPoint;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private IPEndPoint GetIPEndPoint(string idUser)
        {
            try
            {
                var tcpUserModel = TcpUserProvider.Where(t => t.UserModel.IdUser == idUser).FirstOrDefault();
                if (tcpUserModel == null)
                    throw new NullReferenceException($"{nameof(TcpUserProvider)} doesn't have a usermodel for \"{idUser}\" ");
                //Get TcpAcceptedClient instance
                var clientEndPoint = ClientList.Where(t => (t.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() == tcpUserModel.TcpModel.IpAddress
                && (t.Socket.RemoteEndPoint as IPEndPoint).Port == tcpUserModel.TcpModel.Port).Select(t => t.Socket.RemoteEndPoint as IPEndPoint).FirstOrDefault();

                return clientEndPoint;
            }
            catch (NullReferenceException ex)
            {
                _log.Error(ex.Message);
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion

        #region UserModel Process
        private UserModel GenerateUserModel(IAccountRegisterRequestModel model)
        {
            var userModel = InstanceFactory.Build<UserModel>();
            //Insert data
            userModel
            .Insert(
                model.IdUser,
                model.Password,
                model.Name,
                model.EmployeeNumber,
                model.Birth,
                model.Phone,
                model.Address,
                model.EMail,
                model.Image,
                model.Position,
                model.Department,
                model.Company,
                model.Level,
                model.Used
                );
            return userModel;
        }

        private async void RemoveLoginUser(string userId)
        {
            var userAccount = UserProvider.Where(entity => (entity as IUserModel).IdUser == userId).FirstOrDefault();
            await LoginUserProvider.DeletedItem(userAccount);
        }
        #endregion

        #region LoginUserModel Process
        private LoginUserModel GenerateLoginUserModel(IUserModel userAccount, ILoginSessionModel loginSessionModel, IPEndPoint endPoint)
        {
            var loginAccountModel = InstanceFactory.Build<LoginUserModel>();
            //Get TcpClient instance
            var tcpClient = TcpClientProvider.Where(t => t.IpAddress == endPoint.Address.ToString()
            && t.Port == endPoint.Port).FirstOrDefault();
            //Insert data
            loginAccountModel
            .Insert(
                userAccount.IdUser,
                userAccount.Level,
                tcpClient.Id,
                1,
                loginSessionModel.TimeCreated);
            return loginAccountModel;
        }
        private LoginUserModel GetLoginUserModel(string idUser, int mode = 0)
        {
            //Get LoginAccountModel instance
            var loginUserModel = LoginProvider.Where(entity => (entity as ILoginUserModel).UserId == idUser).FirstOrDefault() as ILoginUserModel;
            //Create LoginUserModel instance
            var loginAccount = new LoginUserModel
                (loginUserModel.UserId,
                loginUserModel.UserLevel,
                loginUserModel.ClientId,
                mode,
                DateTime.Now);

            return loginAccount;
        }
        #endregion

        #region LoginSessionModel Process
        public Task<LoginSessionModel> GenerateLoginSessionModel(IUserModel userAccount)
        {
            return Task.Run(async () =>
            {
                var loginSessionModel =new LoginSessionModel(
                    ProviderHelper.GetMaxId(SessionProvider) + 1
                    , userAccount.IdUser
                    , userAccount.Password
                    , await CreateToken()
                    , DateTime.Now
                    , (DateTime.Now + TimeSpan.FromSeconds(AccountSetupModel.SessionTimeout)));

                return loginSessionModel;
            });
        }

        public ILoginSessionModel GetLoginSessionModel(string userId, string userPass)
        {
            var sessionModel = SessionProvider
                .Where(entity => (entity as ILoginSessionModel).UserId == userId
            && (entity as ILoginSessionModel).UserPass == userPass)
                .FirstOrDefault() as ILoginSessionModel;

            return sessionModel;
        }
        public ILoginSessionModel GetLoginSessionModel(string token)
        {
            var sessionModel = SessionProvider.Where(entity => (entity as ILoginSessionModel).Token == token).FirstOrDefault() as ILoginSessionModel;
            /*if (sessionModel == null)
                throw new NullReferenceException();*/

            return sessionModel;
        }
        public ILoginSessionModel UpdateLoginSessionModel(IKeepAliveRequestModel model)
        {
            var sessionModel = SessionProvider.Where(entity => (entity as ILoginSessionModel).Token == model.Token).FirstOrDefault() as ILoginSessionModel;

            if (sessionModel == null) throw new NullReferenceException();

            sessionModel.TimeCreated = DateTime.Now;
            sessionModel.TimeExpired = DateTime.Now + TimeSpan.FromSeconds(AccountSetupModel.SessionTimeout);

            return sessionModel;
        }
        protected void AddLoginSessionModel(ILoginSessionModel sessionModel)
        {
            Sessions[sessionModel.UserId] = sessionModel;
            TimeTags[sessionModel.UserId] = DateTime.Now;
        }
        protected void RemoveLoginSessionModel(ILoginSessionModel sessionModel)
        {
            if (!(Sessions.Count > 0)) return;
            if (sessionModel != null)
            {
                var session = Sessions.ToList().Where(session => session.Value.Token == sessionModel.Token).FirstOrDefault();
                Sessions.Remove(session.Key);
            }
        }
        #endregion

        public Task<string> CreateToken()
        {
            return Task.Run(() =>
            {
                byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                byte[] key = Guid.NewGuid().ToByteArray();
                string token = Convert.ToBase64String(time.Concat(key).ToArray());
                return token;
            });
        }
        #endregion
        #region - Overrides -
        private void SessionTick(object sender, ElapsedEventArgs e)
        {
            //_log.Error($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}][{nameof(SessionTick)}] TICK...");

            //Periodic process for checking Session
            foreach (var item in Sessions.ToList())
            {
                //Get item for LoginSessionModel
                var sessionModel = item.Value;
                //Output instance for Date
                object date;
                TimeTags.TryGetValue(item.Key, out date);

                //Validation Check
                if (!(date is DateTime time))
                    continue;

                //_log.Info($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][TcpServer][Tick] Session({item.Key})[Expired:{item.Value.TimeExpired}] is in Dictionary({Sessions.Count()})");

                //Check the expiration date of a session
                if (DateTime.Now - sessionModel.TimeExpired > TimeSpan.Zero)
                {
                    lock (_locker)
                    {

                        //Create LogoutRequestModel instance
                        var logoutRequestModel = InstanceFactory.Build<LogoutRequestModel>();
                        //Insert data
                        logoutRequestModel.Insert(EnumCmdType.LOGOUT_REQUEST_TIMEOUT, sessionModel.UserId, sessionModel.Token);
                        //Execute Logout process
                        Logout(logoutRequestModel).Wait();

                        _log.Info($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][TcpServer][Tick] Session({item.Key}) was expired and cleared!!({Sessions.Count()})");
                    }
                }
            }
        }

        protected override async void SetupSessions()
        {
            foreach (ILoginSessionModel item in SessionProvider)
            {
                Sessions[item.UserId] = item;
                TimeTags[item.UserId] = DateTime.Now;
                var userModel = UserProvider.Where(entity => (entity as IUserModel).IdUser == item.UserId).FirstOrDefault() as IUserModel;
                await LoginUserProvider.InsertedItem(userModel);
                ClientLogin(userModel.IdUser);
            }
        }

        private ILoginSessionModel GetAccountSessionModel(string idUser, string password)
        {
            var sessionModel = SessionProvider
                .Where(entity => (entity as ILoginSessionModel).UserId == idUser
                && (entity as ILoginSessionModel).UserPass == password)
                .FirstOrDefault() as ILoginSessionModel;
            return sessionModel;
        }
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

            SetSessionTimerInterval();
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

        protected Task<bool> IsLoggedInUser(ILoginRequestModel model)
        {
            return Task.Run(() =>
            {
                try
                {
                    var loggedInUser = SessionProvider.Where(endtity => (endtity as ILoginSessionModel).UserId == model.UserId).FirstOrDefault() as ILoginSessionModel;
                    return loggedInUser != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        protected Task<bool> IsLoggedInUser(IUserSessionBaseRequestModel model)
        {
            return Task.Run(() =>
            {
                try
                {
                    var loggedInUser = SessionProvider.Where(endtity => (endtity as ILoginSessionModel).UserId == model.UserId).FirstOrDefault() as ILoginSessionModel;
                    return loggedInUser != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        protected Task<bool> IsRegisteredUser(string userId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var registeredUser = UserProvider.Where(entity => (entity as IUserModel).IdUser == userId).FirstOrDefault() as IUserModel;
                    return registeredUser != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        protected IUserModel IsValidUser(string userId, string userPass)
        {
            var userModel = UserProvider.Where(entity => (entity as IUserModel).IdUser == userId
            && (entity as IUserModel).Password == userPass).FirstOrDefault() as IUserModel;
            return userModel;

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public Dictionary<string, ILoginSessionModel> Sessions { get; }
        public Dictionary<string, object> TimeTags { get; }
        public TcpClientProvider TcpClientProvider { get; set; }
        public TcpUserProvider TcpUserProvider { get; set; }
        public LoginProvider LoginProvider { get; set; }
        public UserProvider UserProvider { get; }
        public ObservableCollection<UserModel> LoginSessionUsers { get; set; }
        public LoginUserProvider LoginUserProvider { get; }
		public AccountSetupModel AccountSetupModel { get; }
        private AccountDbService AccountDbService { get; }
        public SessionProvider SessionProvider { get; set; }
        #endregion
        #region - Attributes -
        
        public event ITcpCommon.TcpLogin_dele ClientLogin;
        public event ITcpCommon.TcpLogout_dele ClientLogout;
        private System.Timers.Timer SessionTimer;
        private object _locker;
        protected JsonSerializerSettings _settings;
        #endregion
    }
}
