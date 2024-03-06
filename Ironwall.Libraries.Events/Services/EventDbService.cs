using Caliburn.Micro;
using Dapper;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers;
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
    public class EventDbService
    {
        #region - Ctors -
        public EventDbService(
            IDbConnection dbConnection
            , DetectionEventProvider detectionEventProvider
            , ActionEventProvider actionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider
            , ConnectionEventProvider connectionEventProvider
            , EventProvider eventProvider
            , EventSetupModel eventSetupModel

            , ControllerDeviceProvider controllerDeviceProivder
            , SensorDeviceProvider sensorDeviceProvider
            )
        {
            _dbConnection = dbConnection;
            _setupModel = eventSetupModel;

            _detectionEventProvider = detectionEventProvider;
            _actionEventProvider = actionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _connectionEventProvider = connectionEventProvider;

            _eventProvider = eventProvider;

            _controllerProvider = controllerDeviceProivder;
            _sensorProvider = sensorDeviceProvider;
        }
        #endregion
        #region - Implementation of Interface -

        public Task FetchDetectionEvent(string from = null, string to = null, CancellationToken token = default, bool isFinished = false)
        {
            return Task.Run(async () =>
            {
                #region Deprecated code
                /*try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    //Provider 초기화
                    await Task.Run(() => _detectionEventProvider.Clear());

                    var sql = $@"SELECT * FROM {_setupModel.TableDetection} d
                         INNER JOIN {_deviceSetupModel.TableController} c ON d.controller = c.devicenumber
                         INNER JOIN {_deviceSetupModel.TableSensor} s ON d.sensor = s.devicenumber AND d.controller = s.controller";

                    foreach (var model in await (_dbConnection
                        .QueryAsync<DetectionEventModel, SensorDeviceModel, ControllerDeviceModel, DetectionEventModel>(sql, (detection, sensor, controller) =>
                        {
                            detection.ControllerDeviceModel = controller;
                            detection.SensorDeviceModel = sensor;
                            return detection;
                        })))
                    {
                        _detectionEventProvider.Add(model);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchDetectionEvent)}: " + ex.Message);
                }*/
                #endregion
                try
                {
                    string sql = "";
                    if (from != null && to != null)
                        sql = $@"SELECT * FROM {_setupModel.TableDetection} WHERE datetime BETWEEN '{from}' and '{to}'";
                    else
                        sql = $@"SELECT * FROM {_setupModel.TableDetection}";

                    foreach (var model in (await _dbConnection
                        .QueryAsync<DetectionEventMapper>(sql)).ToList())
                    {

                        if (token.IsCancellationRequested)
                            break;

                        var eventModel = _eventProvider?.Where(entity => entity.Id == model.EventId)?.FirstOrDefault();
                        if (eventModel != null) continue;

                        IBaseDeviceModel device = null;

                        switch (model?.Device[0])
                        {
                            case 'c':
                                device = _controllerProvider
                                .Where(item => item.Id == model.Device)
                                .FirstOrDefault() as ControllerDeviceModel;
                                break;
                            case 's':
                                device = _sensorProvider
                                .Where(item => item.Id == model.Device)
                                .FirstOrDefault() as SensorDeviceModel;
                                break;
                            default:
                                break;
                        }

                        var detection = ModelFactory.Build<DetectionEventModel>(model, device);


                        _eventProvider.Add(detection);
                    }
                    //Console.WriteLine($"========> Event Detection : {_eventProvider.Count()}ea");

                    if(isFinished)
                        await _eventProvider.Finished();
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(FetchDetectionEvent)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchDetectionEvent)}: " + ex.Message);
                }
                
            }, token);
        }
        public Task SaveDetectionEvent(IDetectionEventModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDetection;

                    var mapper = MapperFactory.Build<DetectionEventMapper>(model);

                    commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (eventid, eventgroup, device, messagetype, result, status, datetime) 
                        VALUES
                        (@EventId, @EventGroup, @Device, @MessageType, @Result, @Status, @DateTime)", mapper);

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    await _eventProvider.InsertedItem(model);
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveDetectionEvent)}: " + ex.Message);
                }
            });
        }
        public Task UpdateDetectionEvent(IDetectionEventModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDetection;

                    var mapper = MapperFactory.Build<DetectionEventMapper>(model);

                    commitResult = await conn.ExecuteAsync($@"UPDATE {table} SET
                            eventgroup = @EventGroup, device = @Device
                            , messagetype = @MessageType, result = @Result
                            , status = @Status, datetime = @DateTime 
                            WHERE eventid = @EventId", mapper);

                    await _eventProvider.UpdatedItem(model);

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(UpdateDetectionEvent)}: " + ex.Message);
                }
            });
        }
        public Task FetchMalfunctionEvent(string from = null, string to = null, CancellationToken token = default, bool isFinished = false)
        {
            return Task.Run(async () =>
            {

                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    string sql = "";
                    if (from != null && to != null)
                        sql = $@"SELECT * FROM {_setupModel.TableMalfunction} WHERE datetime BETWEEN '{from}' and '{to}'";
                    else
                        sql = $@"SELECT * FROM {_setupModel.TableMalfunction}";


                    var list = (await _dbConnection
                        .QueryAsync<MalfunctionEventMapper>(sql)).ToList();
                    
                    foreach (var model in list)
                    {
                        if (token.IsCancellationRequested) break;

                        var checkModel = _eventProvider?.Where(entity => entity.Id == model.EventId)?.FirstOrDefault();
                        if (checkModel != null) continue;

                        IBaseDeviceModel device = null;

                        switch (model?.Device[0])
                        {
                            case 'c':
                                device = _controllerProvider
                                .Where(item => item.Id == model.Device)
                                .FirstOrDefault() as ControllerDeviceModel;
                                break;
                            case 's':
                                device = _sensorProvider
                                .Where(item => item.Id == model.Device)
                                .FirstOrDefault() as SensorDeviceModel;
                                break;
                            default:
                                break;
                        }

                        var eventModel = ModelFactory.Build<MalfunctionEventModel>(model, device);
                        _eventProvider.Add(eventModel);
                    }
                    if (isFinished)
                        await _eventProvider.Finished();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchMalfunctionEvent)}: " + ex.Message);
                }
            }, token);
        }
        public Task SaveMalfunctionEvent(IMalfunctionEventModel model)
        {
            int commitResult = 0;
            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableMalfunction;

                    var mapper = MapperFactory.Build<MalfunctionEventMapper>(model);

                    commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (eventid, eventgroup, device, messagetype, reason, firststart,  firstend, secondstart, secondend, status, datetime) 
                                        VALUES
                                        (@EventId, @EventGroup, @Device, @MessageType, @Reason, @FirstStart,  @FirstEnd, @SecondStart, @SecondEnd, @Status, @DateTime)", mapper);
                    
                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                    await _eventProvider.InsertedItem(model);
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMalfunctionEvent)}: " + ex.Message);
                }
            });
        }
        public Task UpdateMalfunctionEvent(IMalfunctionEventModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableMalfunction;

                    var mapper = MapperFactory.Build<MalfunctionEventMapper>(model);
                    //reason, firststart,  firstend, secondstart, secondend
                    //@Reason, @FirstStart,  @FirstEnd, @SecondStart, @SecondEnd,
                    commitResult = await conn.ExecuteAsync($@"UPDATE {table} SET
                            eventgroup = @EventGroup, device = @Device
                            , messagetype = @MessageType, reason = @Reason
                            , firststart = @FirstStart, firstend = @FirstEnd
                            , secondstart = @SecondStart, secondend = @SecondEnd
                            , status = @Status, datetime = @DateTime 
                            WHERE eventid = @EventId", mapper);

                    await _eventProvider.UpdatedItem(model);

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(UpdateDetectionEvent)}: " + ex.Message);
                }
            });
        }
        public Task FetchActionEvent(string from = null, string to = null, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    //await _actionEventProvider.ClearData();

                    string sql = "";
                    if (from != null && to != null)
                        sql = $@"SELECT * FROM {_setupModel.TableAction} WHERE datetime BETWEEN '{from}' and '{to}'";
                    else
                        sql = $@"SELECT * FROM {_setupModel.TableAction}";

                    foreach (var model in (await _dbConnection
                        .QueryAsync<ReportEventMapper>(sql)).ToList())
                    {
                        if (token.IsCancellationRequested) break;

                        var checkModel = _actionEventProvider.Where(entity => entity.Id == model.EventId).FirstOrDefault();
                        if (checkModel != null) continue;

                        var metaEvent = _eventProvider
                                    .Where(item => item.Id == model.FromEventId)
                                    .FirstOrDefault() as IMetaEventModel;

                        if (metaEvent == null) continue;

                        ActionEventModel action = null;
                        switch (metaEvent?.MessageType)
                        {
                            case (int)EnumEventType.Intrusion:
                                {
                                    action = ModelFactory
                                    .Build<ActionEventModel>(model, metaEvent as DetectionEventModel);
                                }
                                break;
                            case (int)EnumEventType.Fault:
                                {
                                    action = ModelFactory
                                    .Build<ActionEventModel>(model, metaEvent as MalfunctionEventModel);
                                }
                                break;
                            default:
                                break;
                        }

                        if (action == null)
                            continue;

                        _actionEventProvider.Add(action);
                    }

                    await _actionEventProvider.Finished();
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(FetchActionEvent)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchActionEvent)}: " + ex.Message);
                }
            }, token);
        }
        public Task SaveActionEvent(IActionEventModel model)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableAction;

                    #region Deprecated Code
                    /*var sql = $@"INSERT INTO {table} 
                                (eventid, fromeventid, content, user, datetime) 
                                VALUES 
                                ('{mapper.EventId}', '{mapper.FromEventId}', '{mapper.Content}', '{mapper.Content}', '{mapper.User}', '{mapper.DateTime}')";
                    
                    commitResult = await conn.ExecuteAsync(sql);*/
                    #endregion

                    var mapper = MapperFactory.Build<ReportEventMapper>(model);

                    commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (eventid, fromeventid, content, user, datetime) 
                                        VALUES
                                        (@EventId, @FromEventId, @Content, @User, @DateTime)", mapper);

                    //_actionEventProvider.Add(model);
                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                    await _actionEventProvider.InsertedItem(model);
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveActionEvent)}: " + ex.Message);
                }
                
            });
        }

        public Task FetchEvent(string startDate = null, string endDate = null, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await _eventProvider.ClearData();
                    await _actionEventProvider.ClearData();

                    await FetchDetectionEvent(startDate, endDate, token, true);
                    await Task.Delay(500, token);
                    await FetchMalfunctionEvent(startDate, endDate, token, true);
                    await Task.Delay(500, token);
                    await FetchActionEvent(startDate, endDate, token);
                    await Task.Delay(500, token);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(FetchEvent)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchEvent)}: " + ex.Message);
                }
            }, token);
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
        private EventSetupModel _setupModel;
        private ControllerDeviceProvider _controllerProvider;
        private SensorDeviceProvider _sensorProvider;
        private DetectionEventProvider _detectionEventProvider;
        private ActionEventProvider _actionEventProvider;
        private MalfunctionEventProvider _malfunctionEventProvider;
        private ConnectionEventProvider _connectionEventProvider;
        private EventProvider _eventProvider;
        #endregion
    }
}
