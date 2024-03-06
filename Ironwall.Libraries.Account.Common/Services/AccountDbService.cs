using Dapper;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Account.Common.Models;
using Ironwall.Libraries.Account.Common.Providers;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Services
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
        public Task SaveSession(ILoginSessionModel loginSessionModel, int mode = 0)
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

                            try
                            {
                                //connection.Execute("DELETE FROM dbo.Student WHERE ID = @ID", student);
                                Debug.WriteLine($"Try to delete session(Id : {loginSessionModel.UserId}, Token : {loginSessionModel.Token}) from {nameof(table)}");
                                commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} WHERE token = @Token ", loginSessionModel);

                                await _sessionProvider.DeletedItem(loginSessionModel);
                            }
                            catch (SQLiteException ex)
                            {
                                Debug.WriteLine($"{nameof(SQLiteException)} has raised in {nameof(SaveSession)} for DELETE : {ex.Message}");
                            }
                            break;

                        case 1://DB 레코드 INSERT

                            try
                            {
                                commitResult = conn.Execute($@"INSERT INTO {table} 
                                            (id, userid, userpass, token, timecreated, timeexpired) VALUES (@Id, @UserId, @UserPass, @Token, @TimeCreated, @TimeExpired)", loginSessionModel);

                                await _sessionProvider.InsertedItem(loginSessionModel);

                            }
                            catch (SQLiteException ex)
                            {
                                Debug.WriteLine($"{nameof(SQLiteException)} has raised in {nameof(SaveSession)} for INSERT : {ex.Message}");
                            }
                            break;

                        case 2://DB 레코드 UPDATE
                            try
                            {
                                //Session 연장
                                commitResult = conn.Execute($@"UPDATE {table} SET timecreated = @TimeCreated, timeexpired = @TimeExpired WHERE token = @Token", loginSessionModel);
                                var fetchedSession = await FetchSessionToken(loginSessionModel.Token);
                                
                                var searchedItem = _sessionProvider
                                .Where(entity => (entity as ILoginSessionModel).Token == loginSessionModel.Token).FirstOrDefault() as ILoginSessionModel;

                                //var index = _sessionProvider.CollectionEntity.IndexOf(searchedItem);
                                await _sessionProvider.DeletedItem(searchedItem);
                                await _sessionProvider.InsertedItem(fetchedSession);
                            }
                            catch (SQLiteException ex)
                            {
                                Debug.WriteLine($"{nameof(SQLiteException)} has raised in  {nameof(SaveSession)} for UPDATE : {ex.Message}");
                            }
                              
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
        public Task FetchSession(CancellationToken token = default)
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

                    _sessionProvider.Clear();

                    foreach (var model in (_dbConnection
                        .Query<LoginSessionModel>(sql).ToList()))
                    {
                        if (token.IsCancellationRequested)
                            break;

                        _sessionProvider.Add(model);
                    }

                    await _sessionProvider.Finished();
                }
                catch (SQLiteException ex)
                {
                    var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    Debug.WriteLine($"{nameof(SQLiteException)} has raised in {methodName} : {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchSession: {ex.Message}");
                }
            });
        }

        public Task<ILoginSessionModel> FetchSessionToken(string token)
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
                    ILoginSessionModel searchedItem = (await _dbConnection.QueryAsync<LoginSessionModel>(sql)).FirstOrDefault();

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
        public Task SaveLogin(ILoginUserModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableLogin;

                    //DB 레코드 INSERT
                    commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                    (userid, userlevel, clientid, mode, timecreated) VALUES (@UserId, @UserLevel, @ClientId, @Mode, @TimeCreated)", model);

                    //_loginProvider.CollectionEntity.Insert(0, model);
                    await _loginProvider.InsertedItem(model);
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
        public Task FetchLogin(CancellationToken token = default)
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
                        .Query<LoginUserModel>(sql).ToList()))
                        {
                            if (token.IsCancellationRequested)
                                break;

                            _loginProvider.Add(model);
                        }
                    }, token);
                    await _loginProvider.Finished();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchLogin: {ex.Message}");
                }
            });
        }

        public Task<IUserModel> SaveUser(IUserModel model)
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
                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {tableUser} 
                                (iduser, level, name, password, employeenumber,
                                birth, phone, address, email, image, position, 
                                department, company, used) VALUES 
                                (@IdUser, @Level, @Name, @Password, @EmployeeNumber, 
                                @Birth, @Phone, @Address, @EMail, @Image, 
                                @Position, @Department, @Company, @Used)", model);
                    }
                    else
                    {
                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {tableUser} 
                                (iduser, level, name, password, employeenumber,
                                birth, phone, address, email, image, position, 
                                department, company, used) VALUES 
                                (@IdUser, '{((int)EnumLevel.ADMIN)}',@Name, @Password, 
                                @EmployeeNumber, @Birth, @Phone, @Address, @EMail,
                                @Image, @Position, @Department, @Company, @Used)", model);
                    }

                    var fetchedUser = await FetchUserId(model.IdUser);

                    //_userProvider.Add(fetchedUser);
                    await _userProvider.InsertedItem(fetchedUser);

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

        public Task<bool> RemoveUser(IUserModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var tableUser = _setupModel.TableUser;

                    ///Account 갯수 확인 후, 존재 안할 경우 Admin으로 등록
                    var ret = await conn.ExecuteAsync($@"DELETE FROM {tableUser} 
                                        WHERE iduser = @IdUser 
                                        AND password = @Password", model);

                    commitResult += ret;

                    //_userProvider.Remove(model);
                    await _userProvider.DeletedItem(model);

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

        public Task<bool> EditeUser(IUserModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var tableUser = _setupModel.TableUser;

                    ///Account 갯수 확인 후, 존재 안할 경우 Admin으로 등록
                    commitResult += await conn.ExecuteAsync($@"UPDATE {tableUser} 
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

                    
                    var fetchedUser = await FetchUserId(model.IdUser);
                    var searchedUser = _userProvider
                            .Where(entity => entity.Id == fetchedUser.Id).FirstOrDefault();

                    var index = _userProvider.CollectionEntity.IndexOf(searchedUser);

                    _userProvider.Remove(searchedUser);
                    _userProvider.CollectionEntity.Insert(index, fetchedUser);

                    await _userProvider.UpdatedItem(model);
                    //await _userProvider.Finished();
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

        public Task FetchUserAll(CancellationToken token = default)
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
                        .Query<UserModel>(sql).ToList()))
                        {
                            if (token.IsCancellationRequested)
                                break;
                            _userProvider.Add(model);
                        }
                    }, token);

                    await _userProvider.Finished();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchUserAll: {ex.Message}");
                }
            });
        }

        public Task<IUserModel> FetchUserId(string id)
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
                    IUserModel searchedUser = (await _dbConnection.QueryAsync<UserModel>(sql)).FirstOrDefault();

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
