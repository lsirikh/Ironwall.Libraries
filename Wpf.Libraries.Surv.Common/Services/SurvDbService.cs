using Dapper;
using Ironwall.Libraries.Base.Services;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;

namespace Wpf.Libraries.Surv.Common.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/1/2023 2:41:02 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvDbService
    {

        #region - Ctors -
        public SurvDbService(ILogService log
                            , IDbConnection dbConnection
                            , SurvSetupModel setupModel
                            , SurvApiModelProvider survApiProvider
                            , SurvEventModelProvider survEventProvider
                            , SurvMappingModelProvider survMappingProvider
                            , SurvCameraModelProvider survCameraProvider
                            , SurvSensorModelProvider survSensorProvider)
        {
            _log = log;
            _setupModel = setupModel;
            _survApiProvider = survApiProvider;
            _survEventProvider = survEventProvider;
            _survMappingProvider = survMappingProvider;
            _survCameraProvider = survCameraProvider;
            _survSensorProvider = survSensorProvider;
            _dbConnection = dbConnection;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public async Task FetchSurvApiModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SurvApiModel
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableSurvApi}";

                _survApiProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<SurvApiModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _survApiProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSurvApiModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSurvApiModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertSurvApiModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSurvApi;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _survApiProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, apiaddress, apiport, username, password) 
                            VALUES
                            (@Id, @ApiAddress, @ApiPort, @UserName, @Password)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])", true);
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertSurvApiModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertSurvApiModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchSurvEventModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SurvEventModel
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableSurvEvent}";

                _survEventProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<SurvEventModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _survEventProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSurvEventModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSurvEventModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertSurvEventModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSurvEvent;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _survEventProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, channel, ipaddress, eventname, ison, eventid, apiid, cameraid) 
                            VALUES
                            (@Id, @Channel, @IpAddress, @EventName, @IsOn, @EventId, @ApiId, @CameraId)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])", true);

                await FetchSurvEventModel(token, tcs);
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertSurvEventModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertSurvEventModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchSurvMappingModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SurvMappingModel
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableSurvMapping}";

                _survMappingProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<SurvMappingModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _survMappingProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSurvMappingModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSurvMappingModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertSurvMappingModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSurvMapping;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _survMappingProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, groupnumber, groupname, eventid) 
                            VALUES
                            (@Id, @GroupNumber, @GroupName, @EventId)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])", true);
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertSurvMappingModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertSurvMappingModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchSurvCameraModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SurvMappingModel
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableSurvCamera}";

                _survCameraProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<SurvCameraModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _survCameraProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSurvCameraModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSurvCameraModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertSurvCameraModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSurvCamera;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _survCameraProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, devicename, ipaddress, port, username, password, mode, rtspurl) 
                            VALUES
                            (@Id, @DeviceName, @IpAddress, @Port, @UserName, @Password, @Mode, @RtspUrl)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])", true);
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertSurvCameraModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertSurvCameraModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchSurvSensorModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SurvSensor
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableSurvSensor}";

                _survSensorProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<SurvSensorModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _survSensorProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSurvSensorModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSurvSensorModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertSurvSensorModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSurvSensor;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _survSensorProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, groupname, devicename, controllerid, sensorid, devicetype) 
                            VALUES
                            (@Id, @GroupName, @DeviceName, @ControllerId, @SensorId, @DeviceType)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])", true);
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertSurvSensorModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertSurvSensorModel)}: " + ex.Message, true);
                tcs?.SetException(ex);
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task DeleteRecordOrTable(string table, int? id = default, string refer = "id")
        {
            try
            {
                if (id != null)
                {
                    DeleteRecordFromId(table, id, refer);
                }
                else
                {
                    DeleteTable(table);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(DeleteRecordOrTable)}: " + ex.Message, true);
            }

            return Task.CompletedTask;

        }

        private async void DeleteTable(string table)
        {
            var commitResult = 0;
            var conn = _dbConnection as SQLiteConnection;

            var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                    FROM {table}"));

            if (count > 0)
            {
                commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Error($"No data from {table}", true);
            }
        }

        private async void DeleteRecordFromId(string table, int? id, string refer)
        {
            var commitResult = 0;
            var conn = _dbConnection as SQLiteConnection;

            var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                        FROM {table}
                                                                        WHERE {refer} = '{id}'"));

            if (count > 0)
            {
                commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} 
                                                          WHERE {refer} = '{id}'");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Error($"Not Exist for Matched ID({id})", true);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private SurvSetupModel _setupModel;
        private SurvApiModelProvider _survApiProvider;
        private SurvEventModelProvider _survEventProvider;
        private SurvMappingModelProvider _survMappingProvider;
        private SurvCameraModelProvider _survCameraProvider;
        private SurvSensorModelProvider _survSensorProvider;
        private IDbConnection _dbConnection;
        #endregion
    }
}
