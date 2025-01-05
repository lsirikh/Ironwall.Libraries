using Dapper;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Events.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Services
{
    public class EventDbService : TaskService, IEventDbService
    {
        #region - Ctors -
        public EventDbService(
            IDbConnection dbConnection
            , ILogService log
            , DetectionEventProvider detectionEventProvider
            , ActionEventProvider actionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider
            , ConnectionEventProvider connectionEventProvider
            , EventProvider eventProvider
            , EventSetupModel eventSetupModel

            , DeviceProvider deviceProvider
            )
        {
            _conn = dbConnection;
            _log = log;
            _setupModel = eventSetupModel;

            _detectionEventProvider = detectionEventProvider;
            _actionEventProvider = actionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _connectionEventProvider = connectionEventProvider;

            _eventProvider = eventProvider;

            _deviceProvider = deviceProvider;
            _class = this.GetType();

            TableNames = new List<string>
            {
                _setupModel.TableConnection,
                _setupModel.TableDetection,
                _setupModel.TableMalfunction,
                _setupModel.TableAction
            };
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

        private async Task BuildSchemeAsync(CancellationToken token = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                var cmd = _conn.CreateCommand();

                //Create Connection Event DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableConnection} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            
                                            eventgroup TEXT,
                                            device INTEGER,
                                            messagetype INTEGER,

                                            status INTEGER,
                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDetection} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            
                                            eventgroup TEXT,
                                            device INTEGER,
                                            messagetype INTEGER,

                                            result INTEGER,
                                            status INTEGER,
                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableMalfunction} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,

                                            eventgroup TEXT,
                                            device INTEGER,
                                            messagetype INTEGER,

                                            reason INTEGER, 
                                            firststart INTEGER,  
                                            firstend INTEGER, 
                                            secondstart INTEGER, 
                                            secondend INTEGER,

                                            status INTEGER,
                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                        )";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableAction} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            
                                            fromeventid INTEGER ,
                                            content TEXT,
                                            user TEXT,

                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _log.Error($"Task was cancelled in {nameof(BuildSchemeAsync)}: " + ex.Message, _class);
            }
        }

        private async Task FetchFullInstanceAsync(CancellationToken token)
        {
            try
            {
                var startDate = (DateTime.Now - TimeSpan.FromDays(1)).ToString("yyyy-MM-dd HH:mm:ss");
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var tcs = new TaskCompletionSource<bool>();
                var detections = await FetchDetectionEvents(startDate, endDate, token, tcs);
                var result = await tcs.Task;
                if (!result) throw new Exception($"Fail to execute {nameof(FetchDetectionEvents)}!.");
                foreach (var item in detections)
                {
                    _eventProvider.Add(item);
                }

                tcs = new TaskCompletionSource<bool>();
                var malfunctions = await FetchMalfunctionEvents(startDate, endDate, token, tcs);
                result = await tcs.Task;
                if (!result) throw new Exception($"Fail to execute {nameof(FetchMalfunctionEvents)}!.");
                foreach (var item in malfunctions)
                {
                    _eventProvider.Add(item);
                }

                tcs = new TaskCompletionSource<bool>();
                var actions = await FetchActionEvents(startDate, endDate, token, tcs);
                result = await tcs.Task;
                if (!result) throw new Exception($"Fail to execute {nameof(FetchActionEvents)}!.");

                foreach (var item in actions)
                {
                    _actionEventProvider.Add(item);
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        #region DETECTION PART

        public async Task<IMetaEventModel> FetchMetaEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH META EVENT
                ///////////////////////////////////////////////////////////////////
                var table = await GetTableNameForId(id);

                IMetaEventModel createdModel = null;
                if (table == _setupModel.TableConnection)
                {
                    //Not defined yet!    
                }
                else if (table == _setupModel.TableDetection)
                {

                    createdModel = await CreateEventModel<DetectionEventMapper, DetectionEventModel>(id);
                    //var fetchedModel = (await _conn.QueryAsync<DetectionEventMapper>(sql, parameter))
                    //    .FirstOrDefault();
                    //if (fetchedModel == null) throw new Exception(message: $"Event({id}) is not exist.");
                    //var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                    //createdModel = new DetectionEventModel(fetchedModel, device);
                }
                else if (table == _setupModel.TableMalfunction)
                {
                    createdModel = await CreateEventModel<MalfunctionEventMapper, MalfunctionEventModel>(id);
                }

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        private async Task<T2> CreateEventModel<T1, T2>(int id) where T1 : MetaEventMapper
        {
            try
            {
                var table = await GetTableNameForId(id);
                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT * FROM {table} 
                                WHERE id = @Id
                                ";
                var parameter = new { Id = id };

                var fetchedModel = (await _conn.QueryAsync<T1>(sql, parameter)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({id}) is not exist.");
                var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                var instance = (T2)Activator.CreateInstance(typeof(T2), new object[] { fetchedModel, device });
                return instance;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IDetectionEventModel> FetchDetectionEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDetection;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH DETECTION EVENT
                ///////////////////////////////////////////////////////////////////

                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} 
                                WHERE id = @Id
                                ";
                var parameter = new { Id = id };
                var fetchedModel = (await _conn.QueryAsync<DetectionEventMapper>(sql, parameter)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({id}) is not exist.");

                var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                var createdModel = new DetectionEventModel(fetchedModel, device);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<IDetectionEventModel> FetchDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDetection;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH DETECTION EVENT
                ///////////////////////////////////////////////////////////////////

                var mapper = new DetectionEventMapper(model);
                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} 
                    WHERE eventgroup = @EventGroup
                    AND device = @Device
                    AND messagetype = @MessageType
                    AND result = @Result
                    AND datetime = @DateTime
                ";

                var fetchedModel = (await _conn.QueryAsync<DetectionEventMapper>(sql, mapper)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({model.Id}) is not exist.");

                var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                var createdModel = new DetectionEventModel(fetchedModel, device);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IDetectionEventModel>> FetchDetectionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDetection;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH DETECTION EVENT
                ///////////////////////////////////////////////////////////////////

                string sql = "";
                if (from != null && to != null)
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} WHERE datetime BETWEEN '{from}' and '{to}'";
                else
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table}";

                List<IDetectionEventModel> fetches = new List<IDetectionEventModel>();

                foreach (var model in (await _conn
                    .QueryAsync<DetectionEventMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var device = _deviceProvider.FirstOrDefault(entity => entity.Id == model.Device);

                    var detection = new DetectionEventModel(model, device);

                    fetches.Add(detection);
                }
                tcs?.SetResult(true);
                return fetches;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to fetch DB data in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }


        public Task<IDetectionEventModel> SaveDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDetection;

                    ///////////////////////////////////////////////////////////////////
                    ///                   SAVE DETECTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetEventMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new DetectionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (id, eventgroup, device, messagetype, result, status, datetime) 
                        VALUES
                        (@Id, @EventGroup, @Device, @MessageType, @Result, @Status, @DateTime)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);

                    var fetchedModel = await FetchDetectionEvent(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.Id}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }
        public Task UpdateDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDetection;

                    ///////////////////////////////////////////////////////////////////
                    ///                   UPDATE DETECTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    var mapper = new DetectionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"UPDATE {table} SET
                            eventgroup = @EventGroup
                            , device = @Device
                            , result = @Result
                            , status = @Status
                            WHERE id = @Id", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDetection;

                    ///////////////////////////////////////////////////////////////////
                    ///                   DELETE DETECTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteDetectionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        #endregion

        #region MALFUCTION PART
        public async Task<IMalfunctionEventModel> FetchMalfunctionEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableMalfunction;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH MALFUNCTION EVENT
                ///////////////////////////////////////////////////////////////////

                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} 
                                WHERE id = @Id
                                ";
                var parameter = new { Id = id };
                var fetchedModel = (await _conn.QueryAsync<MalfunctionEventMapper>(sql, parameter)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({id}) is not exist.");

                var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                var createdModel = new MalfunctionEventModel(fetchedModel, device);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDetectionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<IMalfunctionEventModel> FetchMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableMalfunction;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH MALFUNCTION EVENT
                ///////////////////////////////////////////////////////////////////

                var mapper = new MalfunctionEventMapper(model);
                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} 
                    WHERE eventgroup = @EventGroup
                    AND device = @Device
                    AND messagetype = @MessageType

                    AND reason = @Result
                    AND firststart = @Result
                    AND firstend = @Result
                    AND sencondstart = @Result
                    AND secondend = @Result
                    AND datetime = @DateTime
                ";

                var fetchedModel = (await _conn.QueryAsync<MalfunctionEventMapper>(sql, mapper)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({model.Id}) is not exist.");

                var device = _deviceProvider.FirstOrDefault(entity => entity.Id == fetchedModel.Device);
                var createdModel = new MalfunctionEventModel(fetchedModel, device);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchMalfunctionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchMalfunctionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IMalfunctionEventModel>> FetchMalfunctionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableMalfunction;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH MALFUNCTION EVENT
                ///////////////////////////////////////////////////////////////////

                string sql = "";
                if (from != null && to != null)
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} WHERE datetime BETWEEN '{from}' and '{to}'";
                else
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table}";

                List<IMalfunctionEventModel> fetches = new List<IMalfunctionEventModel>();

                foreach (var model in (await _conn
                    .QueryAsync<MalfunctionEventMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var device = _deviceProvider.FirstOrDefault(entity => entity.Id == model.Device);

                    var Malfunction = new MalfunctionEventModel(model, device);

                    fetches.Add(Malfunction);
                }
                tcs?.SetResult(true);
                return fetches;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchMalfunctionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to fetch DB data in {nameof(FetchMalfunctionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }


        public Task<IMalfunctionEventModel> SaveMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableMalfunction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   SAVE MALFUNCTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetEventMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new MalfunctionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (id, eventgroup, device, messagetype, reason, firststart, firstend, secondstart, secondend, status, datetime) 
                        VALUES
                        (@Id, @EventGroup, @Device, @MessageType, @Reason, @FisrtStart, @FirstEnd, @SecondStart, @SecondEnd, @Status, @DateTime)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);

                    var fetchedModel = await FetchMalfunctionEvent(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.Id}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }
        public Task UpdateMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableMalfunction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   UPDATE MALFUNCTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    var mapper = new MalfunctionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"UPDATE {table} SET
                            eventgroup = @EventGroup
                            , device = @Device

                            , reason = @Reason
                            , firststart = @FirstStart
                            , firstend = @FirstEnd
                            , secondstart = @SecondStart
                            , secondend = @SecondEnd
                            , status = @Status
                            WHERE id = @Id", mapper);


                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableMalfunction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   DELETE MALFUNCTION EVENT
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteMalfunctionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        #endregion


        #region ACTION PART

        public async Task<IActionEventModel> FetchActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableAction;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH Action EVENT
                ///////////////////////////////////////////////////////////////////

                var mapper = new ActionEventMapper(model);

                //id, fromeventid, content, user, datetime
                //Status는 조회 속성에서 제외한다.
                string sql = @$"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime  FROM {table} 
                    WHERE id = @Id
                    AND fromeventid = @FromEventId
                    AND content = @Content
                    AND user = @User
                    AND datetime = @DateTime
                ";

                var fetchedModel = (await _conn.QueryAsync<ActionEventMapper>(sql, mapper)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Event({model.Id}) is not exist.");

                var originEvent = await FetchMetaEvent(fetchedModel.FromEventId);
                //var originEvent = _eventProvider.FirstOrDefault(entity => entity.Id == fetchedModel.FromEventId);
                var createdModel = new ActionEventModel(fetchedModel, originEvent);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchActionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchActionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IActionEventModel>> FetchActionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableAction;

                ///////////////////////////////////////////////////////////////////
                ///                   FETCH Action EVENT
                ///////////////////////////////////////////////////////////////////

                string sql = "";
                if (from != null && to != null)
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table} WHERE datetime BETWEEN '{from}' and '{to}'";
                else
                    sql = $@"SELECT *, FORMAT(datetime, 'yyyy-MM-ddTHH:mm:ss.ff') as datetime FROM {table}";

                List<IActionEventModel> fetches = new List<IActionEventModel>();

                foreach (var model in (await _conn
                    .QueryAsync<ActionEventMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var referTable = await GetTableNameForId(model.FromEventId);
                    IMetaEventModel metaEvent = null;
                    if (referTable == _setupModel.TableConnection)
                    {
                        //metaEvent = await FetchConnectionEvent(model.FromEventId);
                    }
                    else if (referTable == _setupModel.TableDetection)
                    {
                        metaEvent = await FetchDetectionEvent(model.FromEventId);
                    }
                    else if (referTable == _setupModel.TableMalfunction)
                    {
                        metaEvent = await FetchMalfunctionEvent(model.FromEventId);
                    }

                    var Action = new ActionEventModel(model, metaEvent);
                    fetches.Add(Action);
                }
                tcs?.SetResult(true);
                return fetches;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchActionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to fetch DB data in {nameof(FetchActionEvent)}: " + ex.Message, _class);
                tcs?.SetException(ex);
                return null;
            }
        }


        public Task<IActionEventModel> SaveActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableAction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   SAVE Action EVENT
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetMaxId(table);
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new ActionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (id, fromeventid, content, user, datetime) 
                        VALUES
                        (@Id, @FromEventId, @Content, @User, @DateTime)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);

                    var fetchedModel = await FetchActionEvent(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.Id}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }
        public Task UpdateActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableAction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   UPDATE Action EVENT
                    ///////////////////////////////////////////////////////////////////

                    var mapper = new ActionEventMapper(model);

                    commitResult = await _conn.ExecuteAsync(
                        $@"UPDATE {table} SET
                            fromeventid = @FromEventId
                            , content = @Content
                            , user = @User
                            WHERE id = @Id", mapper);


                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", _class);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableAction;

                    ///////////////////////////////////////////////////////////////////
                    ///                   DELETE Action EVENT
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteActionEvent)}: " + ex.Message, _class);
                    tcs?.SetException(ex);
                }
            });
        }

        #endregion
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task CheckTable(string table, int? id = null)
        {
            var commitResult = 0;
            var conn = _conn as SQLiteConnection;

            if (id != null)
            {
                var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                FROM {table}
                                                                                WHERE id = '{id}'"));
                if (count > 0)
                {
                    commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} 
                                                                WHERE id = '{id}'");

                    _log.Info($"DELETE a record in {table} for being replaced to new record in DB", _class);
                    if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
                }
            }
            else
            {
                var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                FROM {table}"));

                if (count > 0)
                {
                    commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                    _log.Info($"DELETE a {table} for being replaced to new Table in DB", _class);
                    if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
                }

            }

        }

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

        public async Task<int?> GetEventMaxId()
        {
            try
            {
                var query = $@"
                            SELECT MAX(max_id)
                            FROM (
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableConnection}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableDetection}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableMalfunction}
                            ) AS combined_max_ids;";

                var maxId = (await _conn.QueryAsync<int?>(query))?.FirstOrDefault();

                return maxId;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<bool> IsEventIdExists(int id)
        {
            try
            {
                var query = $@"
                            SELECT EXISTS (
                                SELECT 1 FROM {_setupModel.TableConnection} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableDetection} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableMalfunction} WHERE id = @Id
                                
                            ) AS id_exists;";

                var exists = await _conn.QueryFirstOrDefaultAsync<bool>(query, new { Id = id });

                return exists;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> GetTableNameForId(int id)
        {
            try
            {


                foreach (var tableName in TableNames)
                {
                    var query = $@"SELECT CASE WHEN EXISTS (SELECT 1 FROM {tableName} WHERE id = @Id) THEN @TableName ELSE NULL END";
                    var result = await _conn.QueryFirstOrDefaultAsync<string>(query, new { Id = id, TableName = tableName });

                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }

                return null; // 해당하는 테이블이 없는 경우
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
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(DeleteRecordOrTable)}: " + ex.Message, _class);
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
                _log.Info($"No data from {table}", _class);
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
                _log.Info($"Not Exist for Matched ID({id})", _class);
            }
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public List<string> TableNames { get; private set; }
        #endregion
        #region - Attributes -
        private IDbConnection _conn;
        private ILogService _log;
        private EventSetupModel _setupModel;
        //private ControllerDeviceProvider _controllerProvider;
        //private SensorDeviceProvider _sensorProvider;
        private DetectionEventProvider _detectionEventProvider;
        private ActionEventProvider _actionEventProvider;
        private MalfunctionEventProvider _malfunctionEventProvider;
        private ConnectionEventProvider _connectionEventProvider;
        private EventProvider _eventProvider;
        private DeviceProvider _deviceProvider;
        private Type _class;
        #endregion
    }
}
