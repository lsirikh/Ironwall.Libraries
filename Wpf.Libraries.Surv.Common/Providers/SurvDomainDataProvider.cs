using Ironwall.Libraries.Base.Services;
using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Sdk;
using Wpf.Libraries.Surv.Common.Services;

namespace Wpf.Libraries.Surv.Common.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/1/2023 2:40:47 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvDomainDataProvider : TaskService
    {

        #region - Ctors -
        public SurvDomainDataProvider(ILogService log
                                    , IDbConnection dbConnection
                                    , SurvSetupModel setupModel 
                                    , SurvDbService dbService
                                    , SurvApiService survApiService)
        {
            _log = log;
            _setupModel = setupModel;
            _dbConnection = dbConnection;
            _dbService = dbService;
            _apiService = survApiService;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task RunTask(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                await BuildSchemeAsync();
                await FetchAsync();
            }, token);
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task BuildSchemeAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    using var cmd = _dbConnection.CreateCommand();

                    ////Create TableSurvApi DB Table
                    var dbTable = _setupModel.TableSurvApi;
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            apiaddress TEXT,
                                            apiport INTEGER,
                                            username TEXT,
                                            password TEXT
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableSurvEvent DB Table
                    dbTable = _setupModel.TableSurvEvent;
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            channel INTEGER,
                                            ipaddress TEXT,
                                            eventname TEXT,
                                            ison INTEGER,
                                            eventid INTEGER,
                                            apiid INTEGER,
                                            cameraid INTEGER
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableSurvMapping DB Table
                    dbTable = _setupModel.TableSurvMapping;
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            groupnumber INTEGER,
                                            groupname TEXT,
                                            eventid INTEGER
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableSurvCamera DB Table
                    dbTable = _setupModel.TableSurvCamera;
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            devicename TEXT,
                                            ipaddress TEXT,
                                            port INTEGER,
                                            username TEXT,
                                            password TEXT,
                                            mode INTEGER,
                                            rtspurl TEXT
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableSurvCamera DB Table
                    dbTable = _setupModel.TableSurvSensor;
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            groupname TEXT,
                                            devicename TEXT,
                                            controllerid INTEGER,
                                            sensorid INTEGER,
                                            devicetype INTEGER
                                           )";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    _dbConnection.Close();

                    _log.Error($"Raised {nameof(SQLiteException)} in {nameof(BuildSchemeAsync)} of {nameof(SurvDomainDataProvider)} : {ex.Message}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(BuildSchemeAsync)} of {nameof(SurvDomainDataProvider)} : {ex.Message}");
                }
            });

        }
        private Task FetchAsync()
        {
            return Task.Run(async () =>
            {
                try
                {
                    await _dbService.FetchSurvApiModel();
                    await _dbService.FetchSurvEventModel();
                    await _dbService.FetchSurvMappingModel();
                    await _dbService.FetchSurvCameraModel();
                    await _dbService.FetchSurvSensorModel();
                    _apiService.CreateLookupTable();
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(FetchAsync)} of {nameof(SurvDomainDataProvider)} : {ex.Message}");
                }
            });

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private SurvSetupModel _setupModel;
        private IDbConnection _dbConnection;
        private SurvDbService _dbService;
        private SurvApiService _apiService;
        #endregion
    }
}
