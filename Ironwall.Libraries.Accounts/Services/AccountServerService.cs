using Ironwall.Libraries.Accounts.Models;
using Ironwall.Libraries.Accounts.Providers;
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

namespace Ironwall.Libraries.Accounts.Services
{
    public class AccountServerService
        : TcpServer
        , IAccountServerService
    {
        #region - Ctors -
        public AccountServerService(
            AccountSetupModel accountSetupModel
            , TcpServerSetupModel tcpServerSetupModel
            , SessionProvider sessionProvider
            , TcpClientProvider tcpClientProvider
            , LoginProvider loginProvider
            , UserProvider userProvider
            , LoginUserProvider loginUserProvider
            , AccountDbService accountDbService)
            : base(tcpServerSetupModel)
        {

            AccountSetupModel = accountSetupModel;
            SessionProvider = sessionProvider;
            LoginProvider = loginProvider;
            UserProvider = userProvider;
            TcpClientProvider = tcpClientProvider;

            AccountDbService = accountDbService;
            LoginUserProvider = loginUserProvider;
            LoginSessionUsers = new ObservableCollection<UserModel>();
            Sessions = new Dictionary<string, LoginSessionModel>();
            TimeTags = new Dictionary<string, object>();
        }
        #endregion
        #region - Implementation of Interface -
        public Task<bool> Login(LoginRequestModel model, IPEndPoint endPoint = null)
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
                    var userAccount = UserProvider.Where(entity => entity.IdUser == model.UserId && entity.Password == model.UserPass).FirstOrDefault();

                    if (userAccount == null)
                        throw new Exception(message:"There is not matched account for login");
                    
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
                            var logoutRequestModel = InstanceFactory.Build<LogoutRequestModel>();
                            //Insert data
                            logoutRequestModel.Insert((int)EnumCmdType.LOGOUT_REQUEST_FORCE_LOGIN, accountModel.UserId, accountModel.Token);
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
                    LoginUserProvider.Add(userAccount);

                    //[9]Login Success Response
                    await SendLoginResponse(true, "Successfully Login!", endPoint, loginSessionModel, loginAccountModel);

                    //[10]Send UI log message
                    AcceptedClient_Received($"{userAccount.IdUser} was login!", endPoint);
                    
                    //[11]Login Event trriger
                    ClientLogin(userAccount.IdUser, endPoint);
                    
                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"({model.UserId})Raised SocketSendException in Login : {ex.Message}");
                    AcceptedClient_Received($"({model.UserId})Raised SocketSendException in Login : {ex.Message}", endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised an exception in Login : {ex.Message}");

                    await SendLoginResponse(false, $"Raised an exception in Login : {ex.Message}", endPoint);

                    return false;
                }
            });
        }

        public Task<bool> KeepAlive(KeepAliveRequestModel model, IPEndPoint endPoint = null)
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
                    AcceptedClient_Received($"{sessionModel.UserId} was successfully extend the expiration time", endPoint);
                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in Logout : {ex.Message}");
                    AcceptedClient_Received($"Raised SocketSendException in Logout : {ex.Message}", endPoint);
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine($"Raised NullReferenceException in KeepAlive : {ex.Message}");
                    //Response message to the Client
                    await SendKeepAliveResponse(false, "token was not matched!", null, endPoint);
                    AcceptedClient_Received("token was not matched!", endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in KeepAlive : {ex.Message}");
                    //Response message to the Client
                    await SendKeepAliveResponse(false, $"Raised Exception in KeepAlive : {ex.Message}", null, endPoint);
                    AcceptedClient_Received("token was not matched!", endPoint);
                    return false;
                }
            });
        }

        public Task<bool> Logout(LogoutRequestModel model = null, IPEndPoint endPoint = null)
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
                        LoginSessionModel sessionModel = GetLoginSessionModel(model.Token);

                        //[2]Save LoginSessionModel in DB
                        await AccountDbService.SaveSession(sessionModel);

                        //[3]Remove data from Session Dictionary
                        RemoveLoginSessionModel(sessionModel);

                        //[4]Remove data from LoginUserProvider
                        RemoveLoginUser(model.UserId);

                        //[5]Get LoginAccountModel instance
                        LoginUserModel loginAccount = GetLoginUserModel(model.UserId);

                        //[6]Save LoginAccountModel in DB
                        await AccountDbService.SaveLogin(loginAccount);
                        Debug.WriteLine($"Logout : {loginAccount}");

                        //[7]Get IPEndPoint instance
                        if(endPoint == null)
                            endPoint = GetIPEndPoint(loginAccount.ClientId);

                        //[8]Send response message to client
                        await SendLogoutResponse(true, "Successfully logout!", endPoint);

                        //[9]UI Message Show
                        AcceptedClient_Received($"{model.UserId} was logout!", endPoint);

                        //[10]Logout Event
                        ClientLogout?.Invoke(model.UserId, endPoint);
                    }
                    else
                    {
                        //Logout all sessions 
                        foreach (var sessionModel in SessionProvider.ToList())
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
                            AcceptedClient_Received($"{sessionModel.UserId} was logout!", clientEndPoint);
                            //Logout Event
                            ClientLogout?.Invoke(sessionModel.UserId, clientEndPoint);
                        }
                    }
                    return true;
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"Raised SocketSendException in Logout : {ex.Message}");
                    return false;
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine($"Raised NullReferenceException in Logout : {ex.Message}");
                    await SendLogoutResponse(false, "Token was not matched!", endPoint);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in Logout : {ex.Message}");
                    await SendLogoutResponse(false, $"Raised Exception in Logout : {ex.Message}", endPoint);
                    return false;
                }
            });
        }

        public Task RegisterAccount(AccountRegisterRequestModel model, IPEndPoint endPoint = null)
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
                    UserModel userModel = GenerateUserModel(model);
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
                    AcceptedClient_Received($"{userModel.IdUser} was successfully registered!", endPoint);
                }
                catch (UserRegisterException ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                    AcceptedClient_Received(ex.Message, endPoint);
                    await SendRegisterResponse(false, ex.Message, null, endPoint);
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised SocketSendException in {nameof(RegisterAccount)} : {ex.Message}");
                    AcceptedClient_Received($"({model.IdUser}) Raised SocketSendException in {nameof(RegisterAccount)} : {ex.Message}", endPoint);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised Exception in {nameof(RegisterAccount)} : {ex.Message}");
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
            return Task.Run(async() => 
            {
                try
                {
                    if (model == null)
                        throw new Exception(message: "Delete data is empty.");
                    //01.Check Id and Password
                    UserModel userModel = IsValidUser(model.UserId, model.UserPass);
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
                    if(endPoint == null)
                    {
                        var loginAccount = GetLoginUserModel(model.UserId);
                        endPoint = GetIPEndPoint(loginAccount.ClientId);
                    }
                    //06. Send Register Result Message to Client as Response
                    await SendDeleteRegisterResponse(true, $"{userModel.IdUser} was successfully deleted!", endPoint);
                    //07. UI Communication
                    AcceptedClient_Received($"{userModel.IdUser} was successfully deleted!", endPoint);

                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}");
                    AcceptedClient_Received($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in DeleteAccount : {ex.Message}");
                    await SendDeleteRegisterResponse(false, ex.Message, endPoint);
                    AcceptedClient_Received($"({model?.UserId}) Raised SocketSendException in DeleteAccount : {ex.Message}", endPoint);
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
                    UserModel userModel = GenerateUserModel(model);

                    //03. Account Update in DB
                    //04. Account Instance Find &Edit in UserProvider
                    //05. Fetch User from DB
                    userModel = await AccountDbService.EditeUser(userModel);

                    //06. Send Edit Result Message to Client as Response
                    await SendEditRegisterResponse(true, $"{userModel.IdUser} is successfully edited", userModel, endPoint);

                    //07. UI Communication
                    AcceptedClient_Received($"{userModel.IdUser} is successfully edited", endPoint);
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised SocketSendException in EditAccount : {ex.Message}");
                    AcceptedClient_Received($"({model.IdUser}) Raised SocketSendException in EditAccount : {ex.Message}", endPoint);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised Exception in EditAccount : {ex.Message}");
                    await SendEditRegisterResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Received($"({model.IdUser}) Raised Exception in EditAccount : {ex.Message}", endPoint);
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
                    AcceptedClient_Received($"{userModel.IdUser} is successfully fetched", endPoint);
                }
                catch (SocketSendException ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}");
                    AcceptedClient_Received($"({model.IdUser}) Raised SocketSendException in {nameof(FetchAccount)} : {ex.Message}", endPoint);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}");
                    await SendEditRegisterResponse(false, ex.Message, null, endPoint);
                    AcceptedClient_Received($"({model.IdUser}) Raised Exception in {nameof(FetchAccount)} : {ex.Message}", endPoint);
                }
            });
        }

        

        #region TaskResponse Process
        private Task SendLoginResponse(bool success, string msg, IPEndPoint endPoint = null, LoginSessionModel loginSessionModel = null, LoginUserModel loginAccountModel = null)
        {
            return Task.Run(async () => 
            {
                try
                {
                    LoginResultModel loginResponseResultModel = null;
                    if (loginSessionModel != null && loginAccountModel != null)
                    {
                        var userModel = UserProvider.Where(entity => entity.IdUser == loginSessionModel.UserId).FirstOrDefault();

                        /*var detailModel = InstanceFactory.Build<AccountDetailModel>();
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
                            );*/
                        var detailModel = ResponseFactory.Build<AccountDetailModel>(userModel);

                        //Create LoginResponseResultModel instance
                        //loginResponseResultModel = InstanceFactory.Build<LoginResponseResultModel>();

                        //Insert data
                        //loginResponseResultModel.Insert(loginSessionModel.UserId, loginSessionModel.Token, loginAccountModel.ClientId, loginAccountModel.UserLevel, AccountSetupModel.SessionTimeout, detailModel, loginSessionModel.TimeCreated, loginSessionModel.TimeExpired);

                        loginResponseResultModel = ResponseFactory.Build<LoginResultModel>(loginSessionModel.UserId, loginSessionModel.Token, loginAccountModel.ClientId, loginAccountModel.UserLevel, AccountSetupModel.SessionTimeout, detailModel, loginSessionModel.TimeCreated, loginSessionModel.TimeExpired);
                    }

                    //Create LoginResponseModel instance
                    //var loginResponseModel = InstanceFactory.Build<LoginResponseModel>();
                    //Insert data
                    //loginResponseModel.Insert(success, msg, loginResponseResultModel);
                    var loginResponseModel = ResponseFactory.Build<LoginResponseModel>(success, msg, loginResponseResultModel);
                    //Send Response message(data)
                    await SendRequest(JsonConvert.SerializeObject(loginResponseModel), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }
        
        private Task SendKeepAliveResponse(bool success, string msg, string timeExpired, IPEndPoint endPoint)
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
                    await SendRequest(JsonConvert.SerializeObject(keepAliveResponseModel), endPoint);
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
                    await SendRequest(JsonConvert.SerializeObject(logoutResponseModel), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
                
            });
        }
        
        private Task SendRegisterResponse(bool success, string msg, UserModel userModel = null, IPEndPoint endPoint = null)
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
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountRegisterResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }
        
        private Task SendEditRegisterResponse(bool success, string msg, UserModel userModel = null, IPEndPoint endPoint = null)
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
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountEditResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }

        private Task SendDeleteRegisterResponse(bool success, string msg, IPEndPoint endPoint = null)
        {
            return Task.Run(async () => 
            {
                try
                {
                    //var responseModel = InstanceFactory.Build<AccountDeleteResponseModel>();
                    //responseModel.Insert(success, msg);
                    var responseModel = ResponseFactory.Build<AccountDeleteResponseModel>(success, msg);
                    //Send Response message(data)
                    await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                }
                catch (Exception ex)
                {
                    throw new SocketSendException($"Raised Exception while sending message : {ex.Message}", endPoint);
                }
            });
        }
        
        private Task SendInfoRegisterResponse(bool success, string msg, UserModel userModel = null, IPEndPoint endPoint = null)
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
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
                    else
                    {
                        var responseModel = InstanceFactory.Build<AccountInfoResponseModel>();
                        responseModel.Insert(success, msg, null);
                        //Send Response message(data)
                        await SendRequest(JsonConvert.SerializeObject(responseModel), endPoint);
                    }
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
        #endregion

        #region UserModel Process
        private UserModel GenerateUserModel(AccountRegisterRequestModel model)
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

        private UserModel GenerateUserModel(AccountEditRequestModel model)
        {
            var userModel = InstanceFactory.Build<UserModel>();
            //Insert data
            userModel
            .Insert(
                model.Details.IdUser,
                model.Details.Password,
                model.Details.Name,
                model.Details.EmployeeNumber,
                model.Details.Birth,
                model.Details.Phone,
                model.Details.Address,
                model.Details.EMail,
                model.Details.Image,
                model.Details.Position,
                model.Details.Department,
                model.Details.Company,
                model.Details.Level,
                model.Details.Used
                );
            return userModel;
        }
        private void RemoveLoginUser(string userId)
        {
            UserModel userAccount = UserProvider.Where(entity => entity.IdUser == userId).FirstOrDefault();
            LoginUserProvider.Remove(userAccount);

        }
        #endregion

        #region LoginUserModel Process
        private LoginUserModel GenerateLoginUserModel(UserModel userAccount, LoginSessionModel loginSessionModel, IPEndPoint endPoint)
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
            var loginUserModel = LoginProvider.Where(t => t.UserId == idUser).FirstOrDefault();
            //Create LoginUserModel instance
            var loginAccount = InstanceFactory.Build<LoginUserModel>();
            //Insert data
            loginAccount.Insert(
                loginUserModel.UserId,
                loginUserModel.UserLevel,
                loginUserModel.ClientId,
                mode,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );

            return loginAccount;
        }
        #endregion

        #region LoginSessionModel Process
        public Task<LoginSessionModel> GenerateLoginSessionModel(UserModel userAccount)
        {
            return Task.Run(async () =>
            {
                var loginSessionModel = InstanceFactory.Build<LoginSessionModel>();
                //Insert data
                loginSessionModel
                .Insert(
                    userAccount.IdUser
                    , userAccount.Password
                    , await CreateToken()
                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    , (DateTime.Now + TimeSpan.FromSeconds(AccountSetupModel.SessionTimeout)).ToString("yyyy-MM-dd HH:mm:ss")
                    );

                return loginSessionModel;
            });
        }
        public LoginSessionModel GetLoginSessionModel(string userId, string userPass)
        {
            var sessionModel = SessionProvider.Where(t => t.UserId == userId
            && t.UserPass == userPass).FirstOrDefault();
            /*if (sessionModel == null)
                throw new NullReferenceException();*/

            return sessionModel;
        }
        public LoginSessionModel GetLoginSessionModel(string token)
        {
            var sessionModel = SessionProvider.Where(t => t.Token == token).FirstOrDefault();
            /*if (sessionModel == null)
                throw new NullReferenceException();*/

            return sessionModel;
        }
        public LoginSessionModel UpdateLoginSessionModel(KeepAliveRequestModel model)
        {
            var sessionModel = SessionProvider.Where(t => t.Token == model.Token).FirstOrDefault();
            if (sessionModel == null)
                throw new NullReferenceException();

            sessionModel.TimeCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sessionModel.TimeExpired = (DateTime.Now + TimeSpan.FromSeconds(AccountSetupModel.SessionTimeout)).ToString("yyyy-MM-dd HH:mm:ss");

            return sessionModel;
        }
        private void AddLoginSessionModel(LoginSessionModel sessionModel)
        {
            Sessions[sessionModel.UserId] = sessionModel;
            TimeTags[sessionModel.UserId] = DateTime.Now;
        }
        private void RemoveLoginSessionModel(LoginSessionModel sessionModel)
        {
            var session = Sessions.ToList().Where(session => session.Value.Token == sessionModel.Token).FirstOrDefault();
            Sessions.Remove(session.Key);
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
        protected override async void Tick(object sender, ElapsedEventArgs e)
        {
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

                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][TcpServer][Tick] Session({item.Key}) is in Dictionary({Sessions.Count()})");

                //Check the expiration date of a session
                if (DateTime.Now - DateTime.Parse(sessionModel.TimeExpired) > TimeSpan.Zero)
                {
                    //Create LogoutRequestModel instance
                    var logoutRequestModel = InstanceFactory.Build<LogoutRequestModel>();
                    //Insert data
                    logoutRequestModel.Insert((int)EnumCmdType.LOGOUT_REQUEST_TIMEOUT, item.Value.UserId, item.Value.Token);
                    //Execute Logout process
                    await Logout(logoutRequestModel);

                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][TcpServer][Tick] Session({item.Key}) was expired and cleared!!({Sessions.Count()})");
                }
            }
        }

        protected override void SetupSessions()
        {
            foreach (var item in SessionProvider)
            {
                Sessions[item.UserId] = item;
                TimeTags[item.UserId] = DateTime.Now;
                var userModel = UserProvider.Where(entity => entity.IdUser == item.UserId).FirstOrDefault();
                LoginUserProvider.Add(userModel);
                ClientLogin(userModel.IdUser);
            }
        }

        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task<bool> IsLoggedInUser(LoginRequestModel model)
        {
            return Task.Run(() =>
            {
                try
                {
                    var loggedInUser = SessionProvider.Where(t => t.UserId == model.UserId).FirstOrDefault();
                    return loggedInUser != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        private Task<bool> IsRegisteredUser(string userId)
        {
            return Task.Run(() => 
            {
                try
                {
                    var registeredUser = UserProvider.Where(model => model.IdUser == userId).FirstOrDefault();
                    return registeredUser != null ? true : false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        private UserModel IsValidUser(string userId, string userPass)
        {
            var userModel = UserProvider.Where(model => model.IdUser == userId
            && model.Password == userPass).FirstOrDefault();
            return userModel;

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public Dictionary<string, LoginSessionModel> Sessions { get; }
        public Dictionary<string, object> TimeTags { get; }
        public TcpClientProvider TcpClientProvider { get; set; }
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
        #endregion
    }
}
