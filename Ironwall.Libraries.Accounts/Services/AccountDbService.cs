using Dapper;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Accounts.Models;
using Ironwall.Libraries.Accounts.Providers;
using Ironwall.Libraries.Enums;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Accounts.Services
{
    public class AccountDbService : IAccountDbService
    {

        #region - Ctors -
        public AccountDbService(
            IDbConnection dbConnection
            , SessionProvider sessionProvider
            , LoginProvider loginProvider
            , UserProvider userProvider
            , AccountSetupModel setupModel)
        {
            _dbConnection = dbConnection;
            _sessionProvider = sessionProvider;
            _loginProvider = loginProvider;
            _userProvider = userProvider;
            _setupModel = setupModel;
        }
        #endregion
        #region - Implementation of Interface -


        /// <summary>
        /// Save session user information in Db and SessionProvider
        /// </summary>
        /// <param name="loginSessionModel">Session Model</param>
        /// <param name="mode">0:Delete, 1:Insert, 2:Update</param>
        /// <returns>Task.Complete</returns>
        public Task SaveSession(LoginSessionModel loginSessionModel, int mode = 0)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableSession;

                    switch (mode)
                    {
                        case 0://DB 레코드 DELETE
                            _sessionProvider.Remove(loginSessionModel);
                            //connection.Execute("DELETE FROM dbo.Student WHERE ID = @ID", student);
                            commitResult = conn.Execute($@"DELETE FROM {table} WHERE token = @Token ", loginSessionModel);
                            //(userid, userpass, token, timecreated, timeexpired) VALUES (@UserId, @UserPass, @Token, @TimeCreated, @TimeExpired)", item);
                            break;

                        case 1://DB 레코드 INSERT
                            _sessionProvider.Add(loginSessionModel);
                            commitResult = conn.Execute($@"INSERT INTO {table} 
                                        (userid, userpass, token, timecreated, timeexpired) VALUES (@UserId, @UserPass, @Token, @TimeCreated, @TimeExpired)", loginSessionModel);
                            break;

                        case 2://DB 레코드 UPDATE
                               //Session 연장
                            commitResult = conn.Execute($@"UPDATE {table} SET timecreated = @TimeCreated, timeexpired = @TimeExpired WHERE token = @Token", loginSessionModel);
                            var fetchedSession = await FetchSessionToken(loginSessionModel.Token);
                            var searchedItem = _sessionProvider
                            .Where(entity => entity.Token == loginSessionModel.Token).FirstOrDefault();
                            var index = _sessionProvider.CollectionEntity.IndexOf(searchedItem);
                            _sessionProvider.Remove(searchedItem);

                            _sessionProvider.CollectionEntity.Insert(index, fetchedSession);
                            break;
                        default:
                            break;
                    }
                    #region deprecated
                    /*//DB 레코드 DELETE
                    if (mode == 0)
                    {
                        _sessionProvider.Remove(loginSessionModel);
                        //connection.Execute("DELETE FROM dbo.Student WHERE ID = @ID", student);
                        commitResult = conn.Execute($@"DELETE FROM {table} WHERE token = @Token ", loginSessionModel);
                        //(userid, userpass, token, timecreated, timeexpired) VALUES (@UserId, @UserPass, @Token, @TimeCreated, @TimeExpired)", item);
                    }
                    //DB 레코드 INSERT
                    else if (mode == 1)
                    {
                        _sessionProvider.Add(loginSessionModel);
                        commitResult = conn.Execute($@"INSERT INTO {table} 
                                        (userid, userpass, token, timecreated, timeexpired) VALUES (@UserId, @UserPass, @Token, @TimeCreated, @TimeExpired)", loginSessionModel);
                    }
                    //DB 레코드 UPDATE
                    else if (mode == 2)
                    {
                        //Session 연장
                        var sessionModel = _sessionProvider.Where(t => t.Token == loginSessionModel.Token).FirstOrDefault();
                        sessionModel.TimeCreated = loginSessionModel.TimeCreated;
                        sessionModel.TimeExpired = loginSessionModel.TimeExpired;
                        ///update Table set val = @val where Id = @id", new {val, id = 1}
                        commitResult = conn.Execute($@"UPDATE {table} SET timecreated = @TimeCreated, timeexpired = @TimeExpired WHERE token = @Token", loginSessionModel);
                    }*/
                    #endregion deprecated

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}]");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Task to insert DB data Error: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Fetch Login info from Db(session_users) and insert SessionProvider collection type instance
        /// </summary>
        /// <returns>Task.Complete</returns>
        public Task FetchSession()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    //                           Session
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {_setupModel.TableSession}";

                    //Provider 초기화
                    await Task.Run(() => _sessionProvider.Clear());

                    /// 로직 설명 생략
                    await Task.Run(() =>
                    {
                        foreach (var model in (_dbConnection
                    .Query<LoginSessionModel>(sql).ToList()))
                        {
                            _sessionProvider.Add(model);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchSession: {ex.Message}");
                }
            });
        }

        public Task<LoginSessionModel> FetchSessionToken(string token)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    ///                      Session
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {_setupModel.TableSession} WHERE token = '{token}'";

                    /// 로직 설명 생략
                    var searchedItem = _dbConnection.Query<LoginSessionModel>(sql).FirstOrDefault();

                    return searchedItem;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchSessionToken: {ex.Message}");
                    return null;
                }
            });
        }


        /// <summary>
        /// Save the login information
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Task.Complete</returns>
        public Task SaveLogin(LoginUserModel model)
        {
            int commitResult = 0;

            return Task.Run(() =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableLogin;

                    //DB 레코드 INSERT
                    commitResult = conn.Execute($@"INSERT INTO {table} 
                                    (userid, userlevel, clientid, mode, timecreated) VALUES (@UserId, @UserLevel, @ClientId, @Mode, @TimeCreated)", model);


                    _loginProvider.CollectionEntity.Insert(0, model);
                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Task to insert DB data Error: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Fetch Login info from Db(login_users) and insert LoginProvider collection type instance
        /// </summary>
        /// <returns>Task.Complete</returns>
        public Task FetchLogin()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    ///                         Login
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {_setupModel.TableLogin}";


                    //Provider 초기화
                    await Task.Run(() => _loginProvider.Clear());

                    /// 로직 설명 생략
                    await Task.Run(() =>
                    {
                        foreach (var model in (_dbConnection
                        .Query<LoginUserModel>(sql).ToList()
                        #region N/A
                        /*.Select((item) => new LoginAccountModel()
                        {
                            UserId = item.UserId,
                            UserPass = item.UserPass,
                            Token = item.Token,
                            TimeCreated = item.TimeCreated,
                            TimeExpired = item.TimeExpired
                        })*/
                        #endregion
                        ))
                        {
                            _loginProvider.Add(model);
                        }
                    });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchLogin: {ex.Message}");
                }
            });
        }

        public Task<UserModel> SaveUser(UserModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var tableUser = _setupModel.TableUser;

                    ///Account 갯수 확인 후, 존재 안할 경우 Admin으로 등록
                    if (_userProvider.Count > 0)
                    {
                        commitResult = conn.Execute($@"INSERT INTO {tableUser} 
                                (iduser, name, password, employeenumber,
                                birth, phone, address, email, image, position, 
                                department, company, used) VALUES 
                                (@IdUser, @Name, @Password, @EmployeeNumber, 
                                @Birth, @Phone, @Address, @EMail, @Image, 
                                @Position, @Department, @Company, @Used)", model);
                    }
                    else
                    {
                        commitResult = conn.Execute($@"INSERT INTO {tableUser} 
                                (iduser, level, name, password, employeenumber,
                                birth, phone, address, email, image, position, 
                                department, company, used) VALUES 
                                (@IdUser, '{((int)EnumLevel.ADMIN)}',@Name, @Password, 
                                @EmployeeNumber, @Birth, @Phone, @Address, @EMail,
                                @Image, @Position, @Department, @Company, @Used)", model);
                    }

                    var fetchedUser = await FetchUserId(model.IdUser);

                    _userProvider.Add(fetchedUser);

                    return fetchedUser;
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"SaveUser: {ex.Message}");

                    return null;
                }
            });
        }

        public Task<bool> RemoveUser(UserModel model)
        {
            int commitResult = 0;

            return Task.Run(() =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var tableUser = _setupModel.TableUser;

                    ///Account 갯수 확인 후, 존재 안할 경우 Admin으로 등록
                    var ret = conn.Execute($@"DELETE FROM {tableUser} 
                                        WHERE iduser = @IdUser 
                                        AND password = @Password", model);

                    commitResult += ret;

                    _userProvider.Remove(model);

                    return true;
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"RemoveUser: {ex.Message}");

                    return false;
                }
            });
        }

        public Task<UserModel> EditeUser(UserModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var tableUser = _setupModel.TableUser;

                    ///Account 갯수 확인 후, 존재 안할 경우 Admin으로 등록
                    var ret = conn.Execute($@"UPDATE {tableUser} 
                        SET iduser = @IdUser, 
                        level = @Level, 
                        name = @Name, 
                        password = @Password, 
                        employeenumber = @EmployeeNumber, 
                        birth = @Birth, 
                        phone = @Phone, 
                        address = @Address, 
                        email = @EMail, 
                        image = @Image, 
                        position = @Position, 
                        department = @Department, 
                        company = @Company, 
                        used = @Used 
                        WHERE iduser = @IdUser", model);

                    commitResult += ret;

                    var fetchedUser = await FetchUserId(model.IdUser);

                    var searchedUser = _userProvider
                    .Where(entity => entity.IdUser == model.IdUser).FirstOrDefault();
                    var index = _userProvider.CollectionEntity.IndexOf(searchedUser);
                    _userProvider.Remove(searchedUser);

                    _userProvider.CollectionEntity.Insert(index, fetchedUser);

                    return fetchedUser;
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"RemoveUser: {ex.Message}");

                    return null;
                }
            });
        }

        private void UpdateUserModel(UserModel searchedUser, UserModel fetchedUser)
        {
            searchedUser.IdUser = fetchedUser.IdUser;
            searchedUser.Password = fetchedUser.Password;
            searchedUser.Level = fetchedUser.Level;
            searchedUser.Name = fetchedUser.Name;
            searchedUser.EmployeeNumber = fetchedUser.EmployeeNumber;
            searchedUser.Birth = fetchedUser.Birth;
            searchedUser.Phone = fetchedUser.Phone;
            searchedUser.Address = fetchedUser.Address;
            searchedUser.EMail = fetchedUser.EMail;
            searchedUser.Image = fetchedUser.Image;
            searchedUser.Position = fetchedUser.Position;
            searchedUser.Department = fetchedUser.Department;
            searchedUser.Company = fetchedUser.Company;
            searchedUser.Used = fetchedUser.Used;
        }

        public Task FetchUserAll()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    ///                         User
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {_setupModel.TableUser}";


                    //Provider 초기화
                    await Task.Run(() => _userProvider.Clear());

                    /// 로직 설명 생략
                    await Task.Run(() =>
                    {
                        foreach (var model in (_dbConnection
                        .Query<UserModel>(sql).ToList()
                        #region N/A
                        /*.Select((item) => new LoginAccountModel()
                        {
                            UserId = item.UserId,
                            UserPass = item.UserPass,
                            Token = item.Token,
                            TimeCreated = item.TimeCreated,
                            TimeExpired = item.TimeExpired
                        })*/
                        #endregion
                        ))
                        {
                            _userProvider.Add(model);
                        }
                    });

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchUserAll: {ex.Message}");
                }
            });
        }

        public Task<UserModel> FetchUserId(string id)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    ///                         User
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {_setupModel.TableUser} WHERE iduser = '{id}'";

                    /// 로직 설명 생략
                    var searchedUser = _dbConnection.Query<UserModel>(sql).FirstOrDefault();

                    return searchedUser;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchUserId: {ex.Message}");
                    return null;
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
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private SessionProvider _sessionProvider;
        private LoginProvider _loginProvider;
        private UserProvider _userProvider;
        private AccountSetupModel _setupModel;
        #endregion
    }
}
