using Dapper;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Devices.Helpers;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Base.Services;
using System.IO;
using System.Data.Odbc;

namespace Ironwall.Libraries.Devices.Services
{
    internal class DeviceDbService : TaskService, IDeviceDbService
    {

        #region - Ctors -
        public DeviceDbService(
            IDbConnection dbConnection
            , ILogService log
            , DeviceSetupModel deviceSetupModel
            , DeviceProvider deviceProvider
            , ControllerDeviceProvider controllerDeviceProivder
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            , CameraOptionProvider cameraOptionProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraProfileProvider cameraProfileProvider
            , CameraMappingProvider cameraMappingProvider

            )
        {
            _conn = dbConnection;
            _log = log;

            _setupModel = deviceSetupModel;

            _deviceProvider = deviceProvider;
            _controllerProvider = controllerDeviceProivder;
            _sensorProvider = sensorDeviceProvider;
            _cameraProvider = cameraDeviceProvider;

            _cameraOptionProvider = cameraOptionProvider;
            _presetProvider = cameraPresetProvider;
            _profileProvider = cameraProfileProvider;

            _mappingProvider = cameraMappingProvider;

        }
        #endregion
        #region - Implementation of Interface -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await BuildSchemeAsync(token);

            await FetchFullInstanceAsync(token);
            
            //await CreateEntity();

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

                //Create TableController Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableController} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            devicegroup INTEGER,
                                            devicenumber INTEGER,
                                            devicename TEXT,
                                            devicetype INTEGER,
                                            version TEXT,
                                            status INTEGER,
                                            ipaddress TEXT,
                                            port INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableSensor Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableSensor} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            devicegroup INTEGER,
                                            controller INTEGER,  
                                            devicenumber INTEGER,
                                            devicename TEXT,
                                            devicetype INTEGER,
                                            version TEXT,
                                            status INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableCamera Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableCamera} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            devicegroup INTEGER,
                                            devicenumber INTEGER,
                                            devicename TEXT,
                                            devicetype INTEGER,
                                            version TEXT,
                                            status INTEGER,

                                            ipaddress TEXT,
                                            port INTEGER,
                                            username TEXT,
                                            password TEXT,
                                            category INTEGER,
                                            devicemodel TEXT,
                                            rtspuri TEXT,
                                            rtspport INTEGER,
                                            mode INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceCameraProfile Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDeviceCameraProfile} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            referenceid INTEGER ,
                                            profile TEXT
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceCameraPreset Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDeviceCameraPreset} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            referenceid INTEGER ,
                                            presetname TEXT,
                                            ishome INTEGER,
                                            pan REAL,
                                            tilt REAL,
                                            zoom REAL,
                                            delay INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceCameraPreset Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableCameraMapping} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            mappinggroup TEXT,
                                            sensor INTEGER,
                                            firstpreset INTEGER,
                                            secondpreset INTEGER
                                           )";
                cmd.ExecuteNonQuery();

                ////Create TableDeviceInfo Device DB Table
                //cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDeviceInfo} (
                //                            controller INTEGER,
                //                            sensor INTEGER,
                //                            entity INTEGER,
                //                            updatetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                //                           )";
                //cmd.ExecuteNonQuery();

                ////Create TableDeviceInfo Device DB Table
                //cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableMappingInfo} (
                //                            mapping INTEGER,
                //                            updatetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                //                           )";
                //cmd.ExecuteNonQuery();

                //_conn.Close();
            }
            catch (Exception ex)
            {
                _log.Error($"Task was cancelled in {nameof(BuildSchemeAsync)}: " + ex.Message);
            }
        }


        private async Task FetchFullInstanceAsync(CancellationToken token)
        {
            var controllers = await FetchControllerDevices(token);
            foreach (var item in controllers)
            {
                _deviceProvider.Add(item);
            }
            
            var sensors = await FetchSensorDevices(token);
            foreach (var item in sensors)
            {
                _deviceProvider.Add(item);
            }

            var presets = await FetchPresets(token);
            foreach (var item in presets)
            {
                _cameraOptionProvider.Add(item);
            }

            var profiles = await FetchProfiles(token);
            foreach (var item in profiles)
            {
                _cameraOptionProvider.Add(item);
            }

            var cameras = await FetchCameras(token);
            foreach (var item in cameras)
            {
                _deviceProvider.Add(item);
            }
        }

        #region CEATE DUMMY ENTITY
        private async Task CreateControllerDevice()
        {
            try
            {
                await DeleteRecordOrTable(_setupModel.TableController);

                for (int i = 1; i <= 8; i++)
                {
                    var entity = new ControllerDeviceModel()
                    {
                        DeviceGroup = 0,
                        DeviceNumber = i,
                        DeviceName = $"{i}번_제어기",
                        DeviceType = EnumDeviceType.Controller,
                        Version = "v1.0",
                        Status = EnumDeviceStatus.ACTIVATED,
                        IpAddress = $"192.168.1.{i}",
                        Port = 80,
                    };
                    var savedEntity = await SaveControllerDevice(entity);
                    //_deviceProvider.Add(savedEntity);
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task CreateSensorDevice()
        {
            try
            {
                await DeleteRecordOrTable(_setupModel.TableSensor);
                for (int i = 1; i <= 8; i++)
                {
                    for (int j = 1; j < 190; j++)
                    {
                        if (j > 80 && j < 180) continue;

                        var entity = new SensorDeviceModel()
                        {
                            DeviceGroup = 0,
                            DeviceNumber = j,
                            DeviceName = $"{j}번_센서",
                            DeviceType = j >= 180 ? EnumDeviceType.Multi : EnumDeviceType.Fence,
                            Version = "v1.0",
                            Status = EnumDeviceStatus.ACTIVATED,
                            Controller = _controllerProvider
                            .Where(e => e.DeviceNumber == i).OfType<IControllerDeviceModel>()
                            .FirstOrDefault() as ControllerDeviceModel
                        };
                        var savedEntity = await SaveSensorDevice(entity);
                        //_deviceProvider.Add(savedEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task CreateCameraPreset()
        {
            try
            {

                await DeleteRecordOrTable(_setupModel.TableDeviceCameraPreset);

                var getId = await GetDeviceMaxId();
                int id = 0;
                if (getId != null)
                    id = (int)getId;
                id = id + 1;

                for (int i = id; i < id + 10; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {

                        var entity = new CameraPresetModel() 
                        {
                            ReferenceId = i,
                            PresetName = $"Preset_{j}",
                            IsHome = j == 1 ? true : false,
                            Delay = 3
                        };

                        var savedEntity = await SavePreset(entity);
                       // _cameraOptionProvider.Add(savedEntity);

                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task CreateCameraProfile()
        {
            try
            {
                await DeleteRecordOrTable(_setupModel.TableDeviceCameraProfile);

                var getId = await GetDeviceMaxId();
                int id = 0;
                if (getId != null)
                    id = (int)getId;
                id = id + 1;

                for (int i = id; i < id+10; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        string p = null;
                        switch (j)
                        {
                            case 1:
                                p = "S";
                                break;
                            case 2:
                                p = "T";
                                break;
                            case 3:
                                p = "M";
                                break;
                            default:
                                break;
                        }

                        var entity = new CameraProfileModel()
                        {
                            ReferenceId = i,
                            Profile = p,
                        };

                        var savedEntity = await SaveProfile(entity);
                        //_cameraOptionProvider.Add(savedEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task CreateCameraDevice()
        {
            try
            {

                await DeleteRecordOrTable(_setupModel.TableCamera);

                for (int i = 1; i < 10; i++)
                {

                    var entity = new CameraDeviceModel()
                    {
                        DeviceGroup = 0,
                        DeviceNumber = i,
                        DeviceName = $"Cam{i}",
                        DeviceType = EnumDeviceType.IpCamera,
                        Version = "v1.0",
                        Status = EnumDeviceStatus.ACTIVATED,

                        IpAddress = $"192.168.202.{i}",
                        Port = 80,
                        UserName = "admin",
                        Password = "sensorway1",
                        Category = EnumCameraType.PTZ,
                        DeviceModel = "N/A",
                        RtspUri = "http://rtsp.url.com",
                        RtspPort = 554,
                        
                    };

                    var savedEntity = await SaveCamera(entity);
                    //_deviceProvider.Add(entity);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task CreateEntity()
        {

            try
            {
                //제어기 등록
                await CreateControllerDevice();

                //카메라 등록
                await CreateCameraDevice();

                //센서 등록
                await CreateSensorDevice();

                //프리셋 등록
                await CreateCameraPreset();

                //프로파일 등록
                await CreateCameraProfile();

                
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
        ////////////////////////////////////////////////////CONTROLLER PART////////////////////////////////////////////////////////
        #region CONTROLLER DB CRUD
        public async Task<IControllerDeviceModel> FetchControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableController;

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////

                //string sql = @$"SELECT * FROM {table} 
                //            WHERE devicegroup = '{model.DeviceGroup}' 
                //            AND devicenumber = '{model.DeviceNumber}'
                //            AND devicename = '{model.DeviceName}'
                //            AND devicetype = '{(int)model.DeviceType}'
                //            AND ipaddress = '{model.IpAddress}'
                //            AND port = '{model.Port}'
                //            ";
                string sql = @$"SELECT * FROM {table} 
                    WHERE devicegroup = @DeviceGroup
                    AND devicenumber = @DeviceNumber
                    AND devicename = @DeviceName
                    AND devicetype = @DeviceType
                    AND ipaddress = @IpAddress
                    AND port = @Port";


                var fetchedModel = (await _conn.QueryAsync<ControllerDeviceModel>(sql, model)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Device({model.DeviceName}) is not exist.");

                var createdModel = new ControllerDeviceModel(fetchedModel);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchControllerDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchControllerDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<IControllerDeviceModel> FetchControllerDevice(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableController;

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////

                string sql = @$"SELECT * FROM {table} 
                            WHERE id = '{id}'
                            ";

                var fetchedModel = (await _conn.QueryAsync<ControllerDeviceModel>(sql)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Device(Id:{id}) is not exist.");

                var createdModel = new ControllerDeviceModel(fetchedModel);

                tcs?.SetResult(true);
                return createdModel;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchControllerDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchControllerDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<IControllerDeviceModel>> FetchControllerDevices(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableController;

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                List<IControllerDeviceModel> entities = new List<IControllerDeviceModel>();
                foreach (var model in (await _conn.QueryAsync<ControllerDeviceModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchControllerDevices)}의 테스크가 강제 종료 되었습니다.");

                    entities.Add(model);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchControllerDevices)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchControllerDevices)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<IControllerDeviceModel> SaveControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableController;

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetDeviceMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id+1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port) 
                    VALUES
                    (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchControllerDevice(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch Device({model.DeviceName}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveControllerDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveControllerDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveControllers(List<IControllerDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableController;

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
                            (id, devicenumber, devicegroup, devicename, devicetype, version, status, ipaddress, port) 
                            VALUES
                            (@Id, @DeviceNumber, @DeviceGroup, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(ControllerDeviceModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveControllers)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveControllers)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableController;

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET devicegroup = @DeviceGroup, 
                                                                  devicenumber = @DeviceNumber, 
                                                                  devicename = @DeviceName, 
                                                                  devicetype = @DeviceType, 
                                                                  version = @Version, 
                                                                  status = @Status, 
                                                                  ipaddress = @IpAddress, 
                                                                  port = @Port 
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateControllerDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateControllerDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableController;

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteControllerDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteControllerDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////SENSOR PART////////////////////////////////////////////////////////
        #region SENSOR DB CRUD
        public async Task<ISensorDeviceModel> FetchSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableSensor;

                ///////////////////////////////////////////////////////////////////
                ///                   SENSOR Device
                ///////////////////////////////////////////////////////////////////


                var mapper = new SensorTableMapper(model);

                //string sql = @$"SELECT * FROM {table} 
                //            WHERE devicegroup = '{mapper.DeviceGroup}' 
                //            AND devicenumber = '{mapper.DeviceNumber}'
                //            AND devicename = '{mapper.DeviceName}'
                //            AND devicetype = '{mapper.DeviceType}'
                //            AND controller = '{mapper.Controller}'
                //            ";

                string sql = @$"SELECT * FROM {table} 
                    WHERE devicegroup = @DeviceGroup
                    AND devicenumber = @DeviceNumber
                    AND devicename = @DeviceName
                    AND devicetype = @DeviceType
                    AND controller = @Controller";

                var fetchedModel = (await _conn.QueryAsync<SensorTableMapper>(sql, mapper)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Device({model.DeviceName}) is not exist.");
                var controller = await FetchControllerDevice(fetchedModel.Controller, token, tcs);
                var createdModel = new SensorDeviceModel(fetchedModel, controller);

                tcs?.SetResult(true);
                return createdModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSensorDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSensorDevice)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<ISensorDeviceModel>> FetchSensorDevices(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableSensor;

                ///////////////////////////////////////////////////////////////////
                ///                   SENSOR Device
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<ISensorDeviceModel>();
                foreach (var model in (await _conn.QueryAsync<SensorTableMapper>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchSensorDevices)}의 테스크가 강제 종료 되었습니다.");
                    var controller = _controllerProvider.Where(entity => entity.Id == model.Controller).FirstOrDefault();
                    entities.Add(new SensorDeviceModel(model, controller));
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSensorDevices)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSensorDevices)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<ISensorDeviceModel> SaveSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableSensor;

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetDeviceMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new SensorTableMapper(model);
                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, devicegroup, devicenumber, devicename, devicetype, version, status, controller) 
                    VALUES
                    (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @Controller)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchSensorDevice(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch an entity({model.DeviceName}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveSensorDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveSensorDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveSensors(List<ISensorDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableSensor;

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

                            var mapper = new SensorTableMapper(item);
                            commitResult += await _conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, devicenumber, devicegroup, devicename, devicetype, version, status, controller) 
                            VALUES
                            (@Id, @DeviceNumber, @DeviceGroup, @DeviceName, @DeviceType, @Version, @Status, @Controller)", mapper);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(SensorDeviceModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveSensors)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveSensors)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableSensor;

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET devicegroup = @DeviceGroup, 
                                                                  devicenumber = @DeviceNumber, 
                                                                  devicename = @DeviceName, 
                                                                  devicetype = @DeviceType, 
                                                                  version = @Version, 
                                                                  status = @Status, 
                                                                  controller = @Controller
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateSensorDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateSensorDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableSensor;

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteSensorDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteSensorDevice)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////CAMERA PROFILE PART/////////////////////////////////////////////////////
        #region CAMERA PROFILE DB CRUD
        public async Task<ICameraProfileModel> FetchProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDeviceCameraProfile;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA PROFILE
                ///////////////////////////////////////////////////////////////////


                string sql = @$"SELECT * FROM {table} 
                            WHERE referenceid = @ReferenceId 
                            AND profile = @Profile
                            ";

                var fetchedModel = (await _conn.QueryAsync<CameraProfileModel>(sql, model)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Entity({model.Profile}) from {table} is not exist.");

                tcs?.SetResult(true);
                return fetchedModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchProfile)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchProfile)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<ICameraProfileModel>> FetchProfiles(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDeviceCameraProfile;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA PROFILE
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<ICameraProfileModel>();
                foreach (var model in (await _conn.QueryAsync<CameraProfileModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchProfiles)}의 테스크가 강제 종료 되었습니다.");

                    entities.Add(model);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchProfiles)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchProfiles)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<ICameraProfileModel> SaveProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraProfile;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PROFILE
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetOptionMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, referenceid, profile) 
                    VALUES
                    (@Id, @ReferenceId, @Profile)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchProfile(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch entity({model.Profile}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveProfile)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveProfile)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveProfiles(List<ICameraProfileModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraProfile;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PROFILE
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
                            (id, referenceid, profile) 
                            VALUES
                            (@Id, @ReferenceId, @Profile)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(CameraProfileModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveProfiles)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveProfiles)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraProfile;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PROFILE
                    ///////////////////////////////////////////////////////////////////

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET referenceid = @ReferenceId, 
                                                                  profile = @Profile
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateProfile)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateProfile)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraProfile;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PROFILE
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteProfile)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteProfile)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////CAMERA PRESET PART/////////////////////////////////////////////////////
        #region CAMERA PRESET DB CRUD
        public async Task<ICameraPresetModel> FetchPreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDeviceCameraPreset;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA PRESET
                ///////////////////////////////////////////////////////////////////


                string sql = @$"SELECT * FROM {table} 
                            WHERE referenceid = @ReferenceId 
                            AND presetname = @PresetName
                            AND ishome = @IsHome
                            AND pan = @Pan
                            AND tilt = @Tilt
                            AND zoom = @Zoom
                            AND delay = @Delay
                            ";

                var fetchedModel = (await _conn.QueryAsync<CameraPresetModel>(sql, model)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Entity({model.PresetName}) from {table} is not exist.");

                tcs?.SetResult(true);
                return fetchedModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchPreset)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchPreset)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<ICameraPresetModel>> FetchPresets(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableDeviceCameraPreset;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA PRESET
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<ICameraPresetModel>();
                foreach (var model in (await _conn.QueryAsync<CameraPresetModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchPresets)}의 테스크가 강제 종료 되었습니다.");

                    entities.Add(model);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchPresets)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchPresets)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<ICameraPresetModel> SavePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraPreset;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PRESET
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetOptionMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, referenceid, presetname, ishome, pan, tilt, zoom, delay) 
                    VALUES
                    (@Id, @ReferenceId, @PresetName, @IsHome, @Pan, @Tilt, @Zoom, @Delay)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchPreset(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch entity({model.PresetName}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SavePreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SavePreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SavePresets(List<ICameraPresetModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraPreset;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PRESET
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

                            var getId = await GetOptionMaxId();
                            int id = 0;
                            if (getId != null)
                                id = (int)getId;
                            item.Id = id + 1;

                            commitResult += await _conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, referenceid, presetname, ishome, pan, tilt, zoom, delay) 
                            VALUES
                            (@Id, @ReferenceId, @PresetName, @IsHome, @Pan, @Tilt, @Zoom, @Delay)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(CameraPresetModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SavePresets)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SavePresets)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdatePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraPreset;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PRESET
                    ///////////////////////////////////////////////////////////////////

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET referenceid = @ReferenceId, 
                                                                  presetname = @PresetName,
                                                                  ishome = @IsHome,
                                                                  pan = @Pan,
                                                                  tilt = @Tilt,
                                                                  zoom = @Zoom,
                                                                  delay = @Delay
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdatePreset)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdatePreset)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeletePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraPreset;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA PRESET
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeletePreset)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeletePreset)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////CAMERA DEVICE PART/////////////////////////////////////////////////////
        #region CAMERA PRESET DB CRUD
        public async Task<ICameraDeviceModel> FetchCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableCamera;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA DEVICE
                ///////////////////////////////////////////////////////////////////

                //string sql = @$"SELECT * FROM {table} 
                //            WHERE devicegroup = '{model.DeviceGroup}' 
                //            AND devicenumber = '{model.DeviceNumber}'
                //            AND devicename = '{model.DeviceName}'
                //            AND devicetype = '{model.DeviceType}'

                //            AND ipaddress = '{model.IpAddress}'
                //            AND port = '{model.Port}'
                //            AND username = '{model.UserName}'
                //            AND password = '{model.Password}'
                //            ";


                string sql = @$"SELECT * FROM {table} 
                    WHERE devicegroup = @DeviceGroup
                    AND devicenumber = @DeviceNumber
                    AND devicename = @DeviceName
                    AND devicetype = @DeviceType


                    AND ipaddress = @IpAddress
                    AND port = @Port
                    AND username = @UserName
                    AND password = @Password
                    ";



                var fetchedModel = (await _conn.QueryAsync<CameraDeviceModel>(sql, model)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Device({model.DeviceName}) is not exist.");

                var presets = _presetProvider.Where(t => t.ReferenceId == fetchedModel.Id).OfType<CameraPresetModel>().ToList();
                var profiles = _profileProvider.Where(t => t.ReferenceId == fetchedModel.Id).OfType<CameraProfileModel>().ToList();
                var createdModel = new CameraDeviceModel(fetchedModel, presets, profiles);

                tcs?.SetResult(true);
                return createdModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCamera)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCamera)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<ICameraDeviceModel>> FetchCameras(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableCamera;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA DEVICE
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<ICameraDeviceModel>();
                foreach (var model in (await _conn.QueryAsync<CameraDeviceModel>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchPresets)}의 테스크가 강제 종료 되었습니다.");

                    var presets = _presetProvider.Where(t => t.ReferenceId == model.Id).OfType<CameraPresetModel>().ToList();
                    var profiles = _profileProvider.Where(t => t.ReferenceId == model.Id).OfType<CameraProfileModel>().ToList();
                    var createdModel = new CameraDeviceModel(model, presets, profiles);

                    entities.Add(createdModel);
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameras)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameras)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<ICameraDeviceModel> SaveCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCamera;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA DEVICE
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetDeviceMaxId();
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    commitResult = await _conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port, username,
                    password, category, devicemodel, rtspuri, rtspport, mode) 
                    VALUES
                    (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port, @UserName, 
                    @Password, @Category, @DeviceModel, @RtspUri, @RtspPort, @Mode)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchCamera(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch entity({model.DeviceName}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCamera)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCamera)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveCameras(List<ICameraDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCamera;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA DEVICE
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

                            var getId = await GetDeviceMaxId();
                            int id = 0;
                            if (getId != null)
                                id = (int)getId;
                            item.Id = id + 1;

                            commitResult += await _conn.ExecuteAsync(
                             $@"INSERT INTO {table} 
                            (id, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port, username,
                            password, category, devicemodel, rtspuri, rtspport, mode) 
                            VALUES
                            (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port, @UserName, @Password, @Category, @DeviceModel, @RtspUri, @RtspPort, @Mode)", item);
                        }
                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(CameraPresetModel)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameras)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameras)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCamera;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA DEVICE
                    ///////////////////////////////////////////////////////////////////

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET devicegroup = @DeviceGroup, 
                                                                  devicenumber = @DeviceNumber, 
                                                                  devicename = @DeviceName, 
                                                                  devicetype = @DeviceType, 
                                                                  version = @Version, 
                                                                  status = @Status, 

                                                                  ipaddress = @IpAddress, 
                                                                  port = @Port, 
                                                                  username = @Username, 
                                                                  password = @Password, 
                                                                  category = @Category, 
                                                                  devicemodel = @DeviceModel, 
                                                                  rtspuri = @RtspUri, 
                                                                  rtspport = @RtspPort, 
                                                                  mode = @Mode
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateCamera)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateCamera)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableDeviceCameraPreset;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA DEVICE
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteCamera)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteCamera)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////CAMERA MAPPING PART/////////////////////////////////////////////////////
        #region CAMERA MAPPING DB CRUD
        public async Task<ICameraMappingModel> FetchCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableCameraMapping;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA MAPPING
                ///////////////////////////////////////////////////////////////////

                string sql = @$"SELECT * FROM {table} 
                            WHERE mappinggroup = @MappingGroup 
                            AND sensor = @Sensor
                            AND firstpreset = @FirstPreset
                            AND secondpreset = @SecondPreset
                            ";

                var fetchedModel = (await _conn.QueryAsync<CameraMappingModel>(sql, model)).FirstOrDefault();
                if (fetchedModel == null) throw new Exception(message: $"Device({model.MappingGroup}) is not exist.");

                tcs?.SetResult(true);
                return fetchedModel;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameraMapping)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameraMapping)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public async Task<List<ICameraMappingModel>> FetchCameraMappings(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();
                var table = _setupModel.TableCameraMapping;

                ///////////////////////////////////////////////////////////////////
                ///                   CAMERA MAPPING
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {table}";

                var commitResult = 0;
                var entities = new List<ICameraMappingModel>();
                foreach (var model in (await _conn.QueryAsync<MappingTableMapper>(sql)))    //1
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException($"{nameof(FetchPresets)}의 테스크가 강제 종료 되었습니다.");
                    var sensor = _deviceProvider.OfType<SensorDeviceModel>().Where(entity => entity.Id == model.Sensor).FirstOrDefault();
                    var firstPreset = _cameraOptionProvider.OfType<CameraPresetModel>().Where(entity => entity.Id == model.FirstPreset).FirstOrDefault();
                    var secondPreset = _cameraOptionProvider.OfType<CameraPresetModel>().Where(entity => entity.Id == model.SecondPreset).FirstOrDefault();
                    entities.Add(new CameraMappingModel(model.Id, model.MappingGroup, sensor, firstPreset, secondPreset));
                    commitResult++;
                };
                _log.Info($"({commitResult}) rows was fetched in DB[{table}].", true);

                tcs?.SetResult(true);
                return entities;

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameraMappings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameraMappings)}: " + ex.Message);
                tcs?.SetException(ex);
                return null;
            }
        }

        public Task<ICameraMappingModel> SaveCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCameraMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA MAPPING
                    ///////////////////////////////////////////////////////////////////

                    //model의 id는 없다.
                    //공통 조회 테이블
                    var getId = await GetMaxId(table);
                    int id = 0;
                    if (getId != null)
                        id = (int)getId;
                    model.Id = id + 1;

                    var mapper = new MappingTableMapper(model);
                    commitResult = await _conn.ExecuteAsync($@"INSERT INTO {table} 
                    (id, mappinggroup, sensor, firstpreset, secondpreset)  
                    VALUES
                    (@Id, @MappingGroup, @Sensor, @FirstPreset, @SecondPreset)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchCameraMapping(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch entity({model.MappingGroup}) from {table}");

                    tcs?.SetResult(true);
                    return fetchedModel;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraMapping)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraMapping)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return null;
                }
            });
        }

        public Task SaveCameraMappings(List<ICameraMappingModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCameraMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA MAPPING
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

                            // max_id 대신 id를 사용하여 최대 id 값을 가져옵니다.
                            var query = $@"SELECT MAX(id) FROM {table}";
                            var maxId = (await _conn.QueryAsync<int?>(query))?.FirstOrDefault();
                            int id = 0;
                            if (maxId != null)
                                id = (int)maxId;
                            item.Id = id + 1;

                            var mapper = new MappingTableMapper(item);
                            commitResult += await _conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, mappinggroup, sensor, firstpreset, secondpreset)  
                            VALUES
                            (@Id, @MappingGroup, @Sensor, @FirstPreset, @SecondPreset)", mapper);
                        }

                        // 트랜잭션 커밋
                        transaction.Commit();
                    }
                    _log.Info($"{nameof(SaveCameraMappings)} was inserted in DB[{table}] : {commitResult}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraMappings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraMappings)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task UpdateCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCameraMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA MAPPING
                    ///////////////////////////////////////////////////////////////////

                    commitResult = await _conn.ExecuteAsync($@"UPDATE {table}  
                                                              SET mappinggroup = @MappingGroup, 
                                                                  sensor = @DeviceNumber, 
                                                                  firstpreset = @Sensor, 
                                                                  secondpreset = @FirstPreset, 
                                                                  version = @SecondPreset
                                                              WHERE Id = @Id", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}", true);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateCameraMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateCameraMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task DeleteCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_conn.State != ConnectionState.Open)
                        await (_conn as DbConnection).OpenAsync();
                    var table = _setupModel.TableCameraMapping;

                    ///////////////////////////////////////////////////////////////////
                    ///                   CAMERA MAPPING
                    ///////////////////////////////////////////////////////////////////

                    await DeleteRecordOrTable(table, model.Id.ToString());

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(DeleteCameraMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to delete DB data in {nameof(DeleteCameraMapping)}: " + ex.Message, true);
                    tcs?.SetException(ex);
                }
            });
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

                    _log.Info($"DELETE a record in {table} for being replaced to new record in DB");
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

                    _log.Info($"DELETE a {table} for being replaced to new Table in DB");
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

        public async Task<int?> GetDeviceMaxId()
        {
            try
            {
                var query = $@"
                            SELECT MAX(max_id)
                            FROM (
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableController}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableSensor}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableCamera}
                            ) AS combined_max_ids;";

                var maxId = (await _conn.QueryAsync<int?>(query))?.FirstOrDefault();

                return maxId;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> GetOptionMaxId()
        {
            try
            {
                var query = $@"
                            SELECT MAX(max_id)
                            FROM (
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableDeviceCameraPreset}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableDeviceCameraProfile}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableCameraMapping}
                            ) AS combined_max_ids;";

                var maxId = (await _conn.QueryAsync<int?>(query))?.FirstOrDefault();

                return maxId;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsDeviceIdExists(int id)
        {
            try
            {
                var query = $@"
                            SELECT EXISTS (
                                SELECT 1 FROM {_setupModel.TableController} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableSensor} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableCamera} WHERE id = @Id
                                
                            ) AS id_exists;";

                var exists = await _conn.QueryFirstOrDefaultAsync<bool>(query, new { Id = id });

                return exists;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsOptionIdExists(int id)
        {
            try
            {
                var query = $@"
                            SELECT EXISTS (
                               
                                SELECT 1 FROM {_setupModel.TableDeviceCameraPreset} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableDeviceCameraProfile} WHERE id = @Id
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
                var tableNames = new List<string>
                {
                    _setupModel.TableController,
                    _setupModel.TableSensor,
                    _setupModel.TableCamera,
                    _setupModel.TableDeviceCameraPreset,
                    _setupModel.TableDeviceCameraProfile,
                    _setupModel.TableCameraMapping
                };

                foreach (var tableName in tableNames)
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
        private IDbConnection _conn;
        private ILogService _log;
        private DeviceSetupModel _setupModel;
        private DeviceProvider _deviceProvider;
        private ControllerDeviceProvider _controllerProvider;
        private SensorDeviceProvider _sensorProvider;
        private CameraDeviceProvider _cameraProvider;
        private CameraOptionProvider _cameraOptionProvider;
        private CameraPresetProvider _presetProvider;
        private CameraProfileProvider _profileProvider;
        private CameraMappingProvider _mappingProvider;
        #endregion
    }
}
