using Ironwall.Framework.Models.Vms;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Common;
using Ironwall.Libraries.VMS.Common.Models;
using Dapper;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using System.Collections.Generic;
using System.Linq;
using Ironwall.Framework.Models.Mappers.Vms;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Framework.Helpers;

namespace Ironwall.Libraries.VMS.Common.Services
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/11/2024 2:10:59 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    internal class VmsDbService : TaskService, IVmsDbService
    {
        #region - Ctors -
        public VmsDbService(ILogService log
                            , IDbConnection dbConnection
                            , VmsSetupModel vmsSetupModel
                            , VmsApiProvider vmsApiProvider
                            , VmsSensorProvider vmsSensorProvider
                            , VmsMappingProvider vmsMappingProvider
                            , DeviceProvider deviceProvider)
        {
            _conn = dbConnection;
            _log = log;
            _setupModel = vmsSetupModel;
            _vmsApiProvider = vmsApiProvider;
            _vmsSensorProvider = vmsSensorProvider;
            _vmsMappingProvider = vmsMappingProvider;

            _deviceProvider = deviceProvider;
        }
        #endregion
        #region - Implementation of Interface -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await BuildSchemeAsync(token);

            await FetchFullInstanceAsync(token);
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task BuildSchemeAsync(CancellationToken token = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                var cmd = _conn.CreateCommand();

                //Create TableController Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableVmsApiSetting} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            apiaddress TEXT,
                                            apiport INTEGER,
                                            username TEXT,
                                            password TEXT
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableVmsApiMapping Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableVmsApiMapping} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            groupnumber INTEGER,
                                            eventid INTEGER,  
                                            status INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableCamera Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableVmsApiSensor} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            groupnumber INTEGER,
                                            device INTEGER,
                                            status INTEGER
                                           )";
                cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                _log.Error($"Task was cancelled in {nameof(BuildSchemeAsync)}: " + ex.Message);
            }
        }


        private async Task FetchFullInstanceAsync(CancellationToken token)
        {
            var apiSettings = await FetchVmsApiSettings(token);
            foreach (var item in apiSettings)
            {
                _vmsApiProvider.Add(item);
            }

            var apiMappings = await FetchVmsApiMappings(token);
            foreach (var item in apiMappings)
            {
                _vmsMappingProvider.Add(item);
            }

            var apiSensors = await FetchVmsApiSensors(token);
            foreach (var item in apiSensors)
            {
                _vmsSensorProvider.Add(item);
            }
        }

        #region VMS API SETTING DB CRUD
        public async Task<IVmsApiModel> FetchVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiSetting;

                ///////////////////////////////////////////////////////////////////
                ///                   API SETTING TABLE
                ///////////////////////////////////////////////////////////////////

                string sql = @$"SELECT * FROM {table} 
                    WHERE apiaddress = @ApiAddress
                    AND apiport = @ApiPort
                    AND username = @Username
                    AND password = @Password
                    ";

                var fetchedModel = (await _conn.QueryAsync<VmsApiModel>(sql)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Api Server({model.ApiAddress}:{model.ApiPort}) is not exist.");

                tcs?.SetResult(true);
                return fetchedModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiSetting)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiSetting)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IVmsApiModel>> FetchVmsApiSettings(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiSetting;

                ///////////////////////////////////////////////////////////////////
                ///                   API SETTING TABLE
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<IVmsApiModel>();
                foreach (var model in (await _conn.QueryAsync<VmsApiModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchVmsApiSettings)}의 테스크가 강제 종료 되었습니다.");
                    entities.Add(model);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiSettings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiSettings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<IVmsApiModel> SaveVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSetting;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API SETTING TABLE
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetMaxId(table);
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, apiaddress, apiport, username, password) 
                    VALUES
                    (@Id, @ApiAddress, @ApiPort, @Username, @Password)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchVmsApiSetting(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.ApiAddress}:{model.ApiPort}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiSetting)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiSetting)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveVmsApiSettings(List<IVmsApiModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSetting;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API SETTING TABLE
                    ///////////////////////////////////////////////////////////////////

                    var count = Convert.ToInt32(await _conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    if (count > 0)
                    {
                        commitResult = await _conn.ExecuteAsync($@"DELETE FROM {table}");

                        _log.Info($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    using (var transaction = _conn.BeginTransaction())
                    {
                        foreach (var item in models)
                        {
                            if (token.IsCancellationRequested) throw new TaskCanceledException();

                            commitResult += await _conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, apiaddress, apiport, username, password) 
                            VALUES
                            (@Id, @ApiAddress, @ApiPort, @Username, @Password)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(VmsApiModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiSettings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiSettings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSetting;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API SETTING TABLE
                    ///////////////////////////////////////////////////////////////////
                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET apiaddress = @ApiAddress, 
                                                                  apiport = @ApiPort, 
                                                                  username = @Username, 
                                                                  password = @Password
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateVmsApiSetting)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateVmsApiSetting)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSetting;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API SETTING TABLE
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteVmsApiSetting)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteVmsApiSetting)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion

        #region VMS API MAPPING DB CRUD
        public async Task<IVmsMappingModel> FetchVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiMapping;

                ///////////////////////////////////////////////////////////////////
                ///                   API MAPPING TABLE
                ///////////////////////////////////////////////////////////////////

                string sql = @$"SELECT * FROM {table} 
                    WHERE groupNumber = @GroupNumber
                    AND eventid = @EventId
                    AND status = @Status
                    ";

                var fetchedModel = (await _conn.QueryAsync<VmsMappingModel>(sql)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Api Mapping({model.GroupNumber}) is not exist.");

                tcs?.SetResult(true);
                return fetchedModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiMapping)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiMapping)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IVmsMappingModel>> FetchVmsApiMappings(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiMapping;

                ///////////////////////////////////////////////////////////////////
                ///                   API MAPPING TABLE
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<IVmsMappingModel>();
                foreach (var model in (await _conn.QueryAsync<VmsMappingModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchVmsApiMappings)}의 테스크가 강제 종료 되었습니다.");
                    entities.Add(model);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiMappings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiMappings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<IVmsMappingModel> SaveVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API MAPPING TABLE
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetMaxId(table);
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, apiaddress, apiport, username, password) 
                    VALUES
                    (@Id, @ApiAddress, @ApiPort, @Username, @Password)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchVmsApiMapping(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.GroupNumber}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiMapping)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiMapping)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveVmsApiMappings(List<IVmsMappingModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API MAPPING TABLE
                    ///////////////////////////////////////////////////////////////////

                    var count = Convert.ToInt32(await _conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    if (count > 0)
                    {
                        commitResult = await _conn.ExecuteAsync($@"DELETE FROM {table}");

                        _log.Info($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    using (var transaction = _conn.BeginTransaction())
                    {
                        foreach (var item in models)
                        {
                            if (token.IsCancellationRequested) throw new TaskCanceledException();

                            commitResult += await _conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, groupnumber, eventid, status) 
                            VALUES
                            (@Id, @GroupNumber, @EventId, @Status)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(VmsMappingModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiMappings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiMappings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API MAPPING TABLE
                    ///////////////////////////////////////////////////////////////////
                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET groupnumber = @GroupNumber, 
                                                                  eventid = @EventId, 
                                                                  status = @Status
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateVmsApiMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateVmsApiMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   API MAPPING TABLE
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteVmsApiMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteVmsApiMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion

        #region VMS API SENSOR DB CRUD
        public async Task<IVmsSensorModel> FetchVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiSensor;

                ///////////////////////////////////////////////////////////////////
                ///                   SENSOR Device
                ///////////////////////////////////////////////////////////////////


                var mapper = new VmsSensorMapper(model);

                string sql = @$"SELECT * FROM {table} 
                    WHERE groupnumber = @@GroupNumber
                    AND device = @@Device
                    AND status = @@Status";

                var fetchedModel = (await _conn.QueryAsync<VmsSensorMapper>(sql, mapper)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Sensor({model.Device}) is not exist.");
                var device = _deviceProvider.Where(entity => entity.Id == fetchedModel.Device).FirstOrDefault();

                var createdModel = new VmsSensorModel(model.Id, model.GroupNumber, device, EnumHelper.SetStatusType(fetchedModel.Status));

                tcs?.SetResult(true);
                return createdModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiSensor)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiSensor)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IVmsSensorModel>> FetchVmsApiSensors(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableVmsApiSensor;

                ///////////////////////////////////////////////////////////////////
                ///                   SENSOR Device
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<IVmsSensorModel>();
                foreach (var model in (await _conn.QueryAsync<VmsSensorMapper>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchVmsApiSensors)}의 테스크가 강제 종료 되었습니다.");
                    var device = _deviceProvider.Where(entity => entity.Id == model.Device).FirstOrDefault();

                    entities.Add(new VmsSensorModel(model.Id, model.GroupNumber, device, EnumHelper.SetStatusType(model.Status)));
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchVmsApiSensors)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchVmsApiSensors)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<IVmsSensorModel> SaveVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSensor;

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetMaxId(table);
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new VmsSensorMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, groupnumber, device, status) 
                    VALUES
                    (@Id, @GroupNumber, @Device, @Status)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchVmsApiSensor(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.GroupNumber}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiSensor)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiSensor)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveVmsApiSensors(List<IVmsSensorModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSensor;

                    var count = Convert.ToInt32(await _conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    if (count > 0)
                    {
                        commitResult = await _conn.ExecuteAsync($@"DELETE FROM {table}");

                        _log.Info($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    using (var transaction = _conn.BeginTransaction())
                    {
                        foreach (var item in models)
                        {
                            if (token.IsCancellationRequested) throw new TaskCanceledException();

                            var mapper = new VmsSensorMapper(item);

                            commitResult = await _conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, groupnumber, device, status) 
                            VALUES
                            (@Id, @GroupNumber, @Device, @Status)", mapper);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(VmsSensorModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveVmsApiSensors)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveVmsApiSensors)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSensor;

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET groupnumber = @GroupNumber, 
                                                                  device = @Device, 
                                                                  status = @Status
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateVmsApiSensor)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateVmsApiSensor)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableVmsApiSensor;

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteVmsApiSensor)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteVmsApiSensor)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion

        public async Task<int?> GetMaxId(string tableName)
        {
            try
            {
                var query = $@"SELECT MAX(id) FROM {tableName}";

                var maxId = (await _conn.QueryAsync<int?>(query))?.FirstOrDefault();

                return maxId;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private Task DeleteRecordOrTable(string table, string id = default, string refer = "id")
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
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(DeleteRecordOrTable)}: " + ex.Message);
            }

            return Task.CompletedTask;
        }

        private async void DeleteTable(string table)
        {
            var commitResult = 0;
            var count = Convert.ToInt32(await _conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

            if (count > 0)
            {
                commitResult = await _conn.ExecuteAsync($@"DELETE FROM {table}");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Info($"No data from {table}");
            }
        }

        private async void DeleteRecordFromId(string table, string id, string refer)
        {
            var commitResult = 0;
            var count = Convert.ToInt32(await _conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                        FROM {table}
                                                                        WHERE {refer} = '{id}'"));

            if (count > 0)
            {
                commitResult = await _conn.ExecuteAsync($@"DELETE FROM {table} 
                                                          WHERE {refer} = '{id}'");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Info($"Not Exist for Matched ID({id})");
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private VmsSetupModel _setupModel;
        private VmsApiProvider _vmsApiProvider;
        private VmsSensorProvider _vmsSensorProvider;
        private VmsMappingProvider _vmsMappingProvider;
        private DeviceProvider _deviceProvider;
        private IDbConnection _conn;
        private ILogService _log;
        #endregion
    }
}