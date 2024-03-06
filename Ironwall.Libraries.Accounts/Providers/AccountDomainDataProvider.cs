using Dapper;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Accounts.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Accounts.Providers
{
    public class AccountDomainDataProvider
        : TaskService, IDataProviderService
    {

        #region - Ctors -
        public AccountDomainDataProvider(
            IDbConnection dbConnection
            , AccountSetupModel setupModel
            , SessionProvider sessionProvider
            , LoginProvider loginProvider
            , UserProvider userProvider)
        {
            _dbConnection = dbConnection;
            SetupModel = setupModel;

            _sessionProvider = sessionProvider;
            _loginProvider = loginProvider;
            _userProvider = userProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await Task.Run(delegate
            {
                BuildSchemeAsync();
            })
                .ContinueWith(delegate
                {
                    FetchAsync();
                }, TaskContinuationOptions.ExecuteSynchronously, token);
        }
        public override void Stop()
        {

        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async void BuildSchemeAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                using var cmd = _dbConnection.CreateCommand();

                //Create Session DB Table
                var dbTable = SetupModel.TableSession;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            userid TEXT NOT NULL,
                                            userpass TEXT NOT NULL,
                                            token TEXT NOT NULL,
                                            timecreated DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME')),
                                            timeexpired DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                //Create User DB Table
                var tableUser = SetupModel.TableUser;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {tableUser} (
                                        id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                        iduser TEXT NOT NULL UNIQUE,
                                        level INTEGER DEFAULT 2,
                                        name TEXT NOT NULL,
                                        password TEXT NOT NULL,
                                        employeenumber TEXT,
                                        birth TEXT,
                                        phone TEXT,
                                        address TEXT,
                                        email TEXT,
                                        image TEXT,
                                        position TEXT,
                                        department TEXT,
                                        company TEXT,
                                        used INTEGER DEFAULT 1,
                                        timecreated DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                        )";
                cmd.ExecuteNonQuery();


                //Create Login DB Table
                dbTable = SetupModel.TableLogin;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            userid TEXT NOT NULL,
                                            userlevel INTEGER,
                                            clientid INTEGER,
                                            mode INTEGER,
                                            timecreated DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                _dbConnection.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async void FetchAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                         Session
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {SetupModel.TableSession}";

                foreach (var model in (_dbConnection
                    .Query<LoginSessionModel>(sql).ToList()
                    /*.Select((item) => new LoginAccountModel()
                    {
                        UserId = item.UserId,
                        UserPass = item.UserPass,
                        Token = item.Token,
                        TimeCreated = item.TimeCreated,
                        TimeExpired = item.TimeExpired
                    })*/)
                    )
                {
                    _sessionProvider.Add(model);
                }

                ///////////////////////////////////////////////////////////////////
                ///                         UserAccount
                ///////////////////////////////////////////////////////////////////
                sql = $@"SELECT * FROM {SetupModel.TableUser}";

                foreach (var model in (_dbConnection
                    .Query<UserModel>(sql).ToList()
                    /*.Select((item) => new LoginAccountModel()
                    {
                        UserId = item.UserId,
                        UserPass = item.UserPass,
                        Token = item.Token,
                        TimeCreated = item.TimeCreated,
                        TimeExpired = item.TimeExpired
                    }))*/
                    ))
                {
                    _userProvider.Add(model);
                }


                ///////////////////////////////////////////////////////////////////
                ///                         Login
                ///////////////////////////////////////////////////////////////////
                sql = $@"SELECT * FROM {SetupModel.TableLogin}";

                foreach (var model in (_dbConnection
                    .Query<LoginUserModel>(sql).ToList()
                    /*.Select((item) => new LoginAccountModel()
                    {
                        UserId = item.UserId,
                        UserPass = item.UserPass,
                        Token = item.Token,
                        TimeCreated = item.TimeCreated,
                        TimeExpired = item.TimeExpired
                    }))*/
                    ))
                {
                    model.TimeCreated = DateTime.Parse(model.TimeCreated).ToString("yyyy-MM-dd HH:mm:ss");
                    _loginProvider.CollectionEntity.Insert(0, model);
                }

                _dbConnection.Close();
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
        public AccountSetupModel SetupModel { get; }
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private SessionProvider _sessionProvider;
        private LoginProvider _loginProvider;
        private UserProvider _userProvider;
        #endregion

    }
}
