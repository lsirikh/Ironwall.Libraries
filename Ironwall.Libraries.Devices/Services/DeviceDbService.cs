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

namespace Ironwall.Libraries.Devices.Services
{
    public class DeviceDbService
    {
        
        #region - Ctors -
        public DeviceDbService(
            IDbConnection dbConnection
            , ILogService log
            , DeviceProvider deviceProvider
            , DeviceSetupModel deviceSetupModel
            , ControllerDeviceProvider controllerDeviceProivder
            , SensorDeviceProvider sensorDeviceProvider
            , DeviceInfoModel deviceInfoModel
            , CameraPresetProvider cameraPresetProvider
            , CameraProfileProvider cameraProfileProvider
            , CameraMappingProvider cameraMappingProvider
            , MappingInfoModel mappingInfoModel
            )
        {
            _dbConnection = dbConnection;
            _log = log;
            _setupModel = deviceSetupModel;
            _deviceProvider = deviceProvider;
            _controllerProvider = controllerDeviceProivder;
            _sensorProvider = sensorDeviceProvider;

            _presetProvider = cameraPresetProvider;
            _profileProvider = cameraProfileProvider;

            _mappingProvider = cameraMappingProvider;

            _deviceInfoModel = deviceInfoModel;
            _mappingInfoModel = mappingInfoModel;
        }
        #endregion
        #region - Implementation of Interface -
        public async Task FetchDeviceInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Device
                ///////////////////////////////////////////////////////////////////

                //DeviceType에 따라서 다른 방식으로 Fetch해온다.
                //var sql = $@"SELECT deviceid, devicenumber, devicename, devicetype, ipaddress, port, version, status FROM {_setupModel.TableController}";
                var sql = $@"SELECT * FROM {_setupModel.TableDeviceInfo}";


                foreach (var model in (await _dbConnection
                    .QueryAsync<DeviceInfoTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested)
                        break;

                    var deviceDetailModel = ModelFactory.Build<DeviceDetailModel>(model);
                    _deviceInfoModel.Update(deviceDetailModel);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDeviceInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDeviceInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task DeleteDeviceInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableDeviceInfo;

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Device
                ///////////////////////////////////////////////////////////////////

                var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                
                int commitResult = 0;
                if (count > 0)
                { 
                    commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                    _log.Info($"DELETE {table} for not being matched device data in DB");
                    if (!(commitResult > 0))
                        throw new Exception($"Raised exception during deleting Table({table}).");

                    _deviceInfoModel.Clear();
                }
                else
                {
                    _log.Info($"{table} was empty Table in DB");
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchDeviceInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchDeviceInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public Task UpdateDeviceInfo(IDeviceDetailModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDeviceInfo;

                    var mapper = MapperFactory.Build<DeviceInfoTableMapper>(model);

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if(count == 0)
                    {
                        commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (controller, sensor, camera, updatetime) 
                        VALUES
                        (@Controller, @Sensor, @Camera, @UpdateTime)", mapper);
                    }
                    else
                    {
                        commitResult = await conn.ExecuteAsync(
                            $@"UPDATE {table} 
                            SET
                            controller = @Controller, sensor = @Sensor, camera = @Camera, updatetime = @UpdateTime" , mapper);
                    }

                    _deviceInfoModel.Update(model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : Controller({model.Controller}ea), Sensor({model.Sensor}ea), Camera({model.Camera}ea), UpdateTime({model.UpdateTime})");
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateDeviceInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateDeviceInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        //public async Task FetchControllerDevice(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        //{
        //    try
        //    {
        //        if (_dbConnection.State != ConnectionState.Open)
        //            await (_dbConnection as DbConnection).OpenAsync();

        //        ///////////////////////////////////////////////////////////////////
        //        ///                   CONTROLLER Device
        //        ///////////////////////////////////////////////////////////////////

        //        //DeviceType에 따라서 다른 방식으로 Fetch해온다.
        //        //var sql = $@"SELECT deviceid, devicenumber, devicename, devicetype, ipaddress, port, version, status FROM {_setupModel.TableController}";
        //        var sql = $@"SELECT * FROM {_setupModel.TableController}";

        //        await Task.Delay(100, token);

        //        int count = 0;

        //        foreach (var model in (await _dbConnection
        //            .QueryAsync<ControllerTableMapper>(sql)).ToList())
        //        {
        //            if (token.IsCancellationRequested)
        //                break;

        //            var controllerModel = ModelFactory.Build<ControllerDeviceModel>(model);
        //            _deviceProvider.Add(controllerModel);
        //            count++;
        //        }


        //        if (isFinished)
        //            await _deviceProvider.Finished();

        //        tcs?.SetResult(true);

        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        _log.Info($"Task was cancelled in {nameof(FetchControllerDevice)}: " + ex.Message);
        //        tcs?.SetException(ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Info($"Raised Exception for Task to insert DB data in {nameof(FetchControllerDevice)}: " + ex.Message);
        //        tcs?.SetException(ex);
        //    }
        //}

        public async Task<IControllerDeviceModel> FetchControllerDevice(IControllerDeviceModel model, CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////

                string sql = @$"SELECT * FROM {_setupModel.TableController} 
                            WHERE devicegroup = '{model.DeviceGroup}' 
                            AND devicenumber = '{model.DeviceNumber}'
                            AND devicename = '{model.DeviceName}'
                            AND DeviceType = '{model.DeviceType}'
                            ";

                var fetchedModel = (await _dbConnection.QueryAsync<ControllerDeviceModel>(sql)).FirstOrDefault();
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

        public Task<IControllerDeviceModel> SaveControllerDevice(IControllerDeviceModel model, CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableController;

                    //Mapper를 쓸 이유가 없다.
                    //var mapper = MapperFactory.Build<ControllerTableMapper>(model);

                    commitResult = await conn.ExecuteAsync(
                    $@"INSERT INTO {table} 
                    (id, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port) 
                    VALUES
                    (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port)", model);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    var fetchedModel = await FetchControllerDevice(model);
                    if (fetchedModel == null) throw new Exception($"Fail to fetch {model.Token} from {table}");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveControllerDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveControllerDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }
        
        public Task SaveControllers(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableController;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    
                    if(count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        _log.Info($"DELETE {_setupModel.TableController} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (var item in _deviceProvider.OfType<IControllerDeviceModel>().ToList())
                        {
                            if (token.IsCancellationRequested)  throw new TaskCanceledException();

                            //if (DeviceHelper.IsControllerCategory(item.DeviceType))
                            //{
                            //    var mapper = MapperFactory.Build<ControllerTableMapper>(item as IControllerDeviceModel);

                            //    commitResult += await conn.ExecuteAsync(
                            //        $@"INSERT INTO {table} 
                            //        (id, devicenumber, devicegroup, devicename, devicetype, version, status, ipaddress, port) 
                            //        VALUES
                            //        (@Id, @DeviceNumber, @DeviceGroup, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port)", item);
                            //}

                            commitResult += await conn.ExecuteAsync($@"INSERT INTO {table} 
                            (id, devicenumber, devicegroup, devicename, devicetype, version, status, ipaddress, port) 
                            VALUES
                            (@Id, @DeviceNumber, @DeviceGroup, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port)", item);
                        }
                    }
                    _log.Info($"ControllerDevice was inserted in DB[{table}] : {commitResult}ea");
                    
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

        public async Task FetchSensorDevice(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> task = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Sensor Device
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableSensor}";

                await Task.Delay(100, token);

                int count = 0;

                foreach (var model in (await _dbConnection
                    .QueryAsync<SensorTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var controllerModel = _controllerProvider.Where(t => t.DeviceNumber == model.Controller).FirstOrDefault() as ControllerDeviceModel;

                    var sensorModel = ModelFactory.Build<SensorDeviceModel>(model, controllerModel);
                    _deviceProvider.Add(sensorModel);

                    count++;
                }

                if (isFinished)
                    await _deviceProvider.Finished();

                task?.SetResult(true);

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSensorDevice)}: " + ex.Message);
                task?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSensorDevice)}: " + ex.Message);
                task?.SetException(ex);
            }
        }

        public Task SaveSensorDevice(ISensorDeviceModel model, CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableSensor;

                    var mapper = MapperFactory.Build<SensorTableMapper>(model);

                    commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (deviceid, devicegroup, controller, devicenumber, devicename, devicetype, version, status) 
                        VALUES
                        (@Id, @DeviceGroup, @Controller, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    await _deviceProvider.InsertedItem(model);

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveSensorDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveSensorDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task SaveSensors(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;
            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableSensor;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");
                        _log.Info($"DELETE {table} for saving new Data in DB");

                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _deviceProvider)
                    {
                        if (token.IsCancellationRequested)
                            throw new TaskCanceledException();
                        
                        if(DeviceHelper.IsSensorCategory(item.DeviceType))
                        {
                            var mapper = MapperFactory.Build<SensorTableMapper>(item as ISensorDeviceModel);

                            commitResult = await conn.ExecuteAsync(
                                $@"INSERT INTO {table} 
                                (deviceid, devicegroup, controller, devicenumber, devicename, devicetype, version, status) 
                                VALUES
                                (@Id, @DeviceGroup, @Controller, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status)", mapper);

                            updatedRecord++;
                        }

                    }
                    _log.Info($"SensorDevice was inserted in DB[{table}] : {updatedRecord}ea");

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

        public async Task FetchCameraPreset(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> task = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Caemra Preset
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableDeviceCameraPreset}";

                await Task.Delay(100, token);

                int count = 0;

                foreach (var mapper in (await _dbConnection
                    .QueryAsync<PresetTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var model = ModelFactory.Build<CameraPresetModel>(mapper);
                    _presetProvider.Add(model);

                    count++;
                }

                if (isFinished)
                    await _presetProvider.Finished();

                task?.SetResult(true);

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameraPreset)}: " + ex.Message);
                task?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameraPreset)}: " + ex.Message);
                task?.SetException(ex);
            }
        }

        public Task SaveCameraPreset(List<CameraPresetModel> presets, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDeviceCameraPreset;

                    var preset = presets.FirstOrDefault();
                    if (preset != null)
                    {
                        foreach (var item in _presetProvider.ToList())
                        {
                            if (item.ReferenceId == preset.ReferenceId)
                                await _presetProvider.DeletedItem(item);
                        }

                        var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                FROM {table}
                                                                                WHERE referenceid = '{preset.ReferenceId}'"));

                        if (count > 0)
                        {
                            commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} 
                                                                WHERE referenceid = '{preset.ReferenceId}'");

                            _log.Info($"DELETE {table} for not being matched device data in DB");
                            if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
                        }
                    }

                    int updatedRecord = 0;
                    foreach (var item in presets)
                    {
                        var mapper = MapperFactory.Build<PresetTableMapper>(item);

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                        (optionid, referenceid, presetname, ishome, pan, tilt, zoom, delay)  
                        VALUES
                        (@OptionId, @ReferenceId, @PresetName, @IsHome, @Pan, @Tilt, @Zoom, @Delay)", mapper);

                        await _presetProvider.InsertedItem(item);
                        updatedRecord++;
                    }

                    _log.Info($"CameraPreset was inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task SaveCameraPresets(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDeviceCameraPreset;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                FROM {table}"));

                    if(count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        _log.Info($"DELETE {table} data in DB");
                        if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _presetProvider)
                    {
                        var mapper = MapperFactory.Build<PresetTableMapper>(item);

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                        (optionid, referenceid, presetname, ishome, pan, tilt, zoom, delay)  
                        VALUES
                        (@OptionId, @ReferenceId, @PresetName, @IsHome, @Pan, @Tilt, @Zoom, @Delay)", mapper);

                        updatedRecord++;
                    }

                    _log.Info($"CameraPreset was inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task FetchCameraProfile(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> task = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Caemra Profile
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableDeviceCameraProfile}";

                await Task.Delay(100, token);

                int count = 0;

                foreach (var mapper in (await _dbConnection
                    .QueryAsync<ProfileTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    var model = ModelFactory.Build<CameraProfileModel>(mapper);
                    _profileProvider.Add(model);

                    count++;
                }

                if (isFinished)
                    await _profileProvider.Finished();

                task?.SetResult(true);

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameraProfile)}: " + ex.Message);
                task?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameraProfile)}: " + ex.Message);
                task?.SetException(ex);
            }
        }

        public Task SaveCameraProfile(List<CameraProfileModel> profiles, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableDeviceCameraProfile;

                    var profile = profiles.FirstOrDefault();
                    if(profile != null)
                    {
                        foreach (var item in _profileProvider.ToList())
                        {
                            if (item.ReferenceId == profile.ReferenceId) 
                                await _profileProvider.DeletedItem(item);
                        }

                        var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                FROM {table}
                                                                                WHERE referenceid = '{profile.ReferenceId}'"));

                        if (count > 0)
                        {
                            commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} 
                                                                WHERE referenceid = '{profile.ReferenceId}'");
                            
                            _log.Info($"DELETE {table} for not being matched device data in DB");
                            if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
                        }
                    }

                    int updatedRecord = 0;
                    foreach (var item in profiles)
                    {
                        var mapper = MapperFactory.Build<ProfileTableMapper>(item);

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                                        (optionid, referenceid, profile) 
                                                        VALUES
                                                        (@OptionId, @ReferenceId, @Profile)", mapper);

                        await _profileProvider.InsertedItem(item);
                        updatedRecord++;
                    }

                    _log.Info($"CameraProfile was inserted in DB[{table}] : {updatedRecord}ea");
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraProfile)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraProfile)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task FetchCameraDevice(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> task = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await(_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Caemra Device
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableCamera}";

                await Task.Delay(100, token);

                int count = 0;

                foreach (var model in (await _dbConnection
                    .QueryAsync<CameraTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    List<CameraPresetModel> presets = _presetProvider.Where(t => t.ReferenceId == model.Id).OfType<CameraPresetModel>().ToList();
                    List<CameraProfileModel> profiles = _profileProvider.Where(t => t.ReferenceId == model.Id).OfType<CameraProfileModel>().ToList();

                    var cameraModel = ModelFactory.Build<CameraDeviceModel>(model, presets, profiles);
                    _deviceProvider.Add(cameraModel);

                    count++;
                }

                if (isFinished)
                    await _deviceProvider.Finished();

                task?.SetResult(true);

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchSensorDevice)}: " + ex.Message);
                task?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchSensorDevice)}: " + ex.Message);
                task?.SetException(ex);
            }
        }

        public Task SaveCameraDevice(ICameraDeviceModel model, CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableCamera;

                    await SaveCameraPreset(model?.Presets, token);
                    await SaveCameraProfile(model?.Profiles, token);

                    var mapper = MapperFactory.Build<CameraTableMapper>(model);
                    commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (deviceid, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port, username, password, category, devicemodel, rtspuri, rtspport, mode) 
                        VALUES
                        (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port, @Username, @Password, @Category, @DeviceModel, @RtspUri, @RtspPort, @Mode)", mapper);

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    await _deviceProvider.InsertedItem(model);

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraDevice)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task SaveCameras(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;
            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableCamera;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");
                        _log.Info($"DELETE {table} for saving new Data in DB");

                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _deviceProvider.OfType<ICameraDeviceModel>().ToList())
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        await SaveCameraPreset(item.Presets, token);
                        await SaveCameraProfile(item.Profiles, token);

                        var mapper = MapperFactory.Build<CameraTableMapper>(item);
                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                (deviceid, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port, username, password, category, devicemodel, rtspuri, rtspport, mode) 
                                VALUES
                                (@Id, @DeviceGroup, @DeviceNumber, @DeviceName, @DeviceType, @Version, @Status, @IpAddress, @Port, @Username, @Password, @Category, @DeviceModel, @RtspUri, @RtspPort, @Mode)", mapper);

                        updatedRecord++;
                    }

                    _log.Info($"CameraDevice was inserted in DB[{table}] : {updatedRecord}ea");

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

        public async Task FetchMappingInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Mapping
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableMappingInfo}";

                if (token.IsCancellationRequested) throw new TaskCanceledException();

                var model = (await _dbConnection.QueryAsync<MappingInfoModel>(sql))?.FirstOrDefault();
                if(model != null)
                {
                    _mappingInfoModel.Update(model);
                    tcs?.SetResult(true);
                }
                else
                {
                    tcs?.SetResult(false);
                }

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchMappingInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchMappingInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task DeleteMappingInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableMappingInfo;

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Mapping
                ///////////////////////////////////////////////////////////////////

                var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                int commitResult = 0;
                if (count > 0)
                {
                    commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                    _log.Info($"DELETE {table} for not being matched mapping if data in DB");
                    if (!(commitResult > 0))
                        throw new Exception($"Raised exception during deleting Table({table}).");

                    _mappingInfoModel.Clear();
                    tcs?.SetResult(true);
                }
                else
                {
                    _log.Info($"{table} was empty Table in DB");
                    tcs?.SetResult(false);
                }
                
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(DeleteMappingInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(DeleteMappingInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public Task UpdateMappingInfo(IMappingInfoModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    ///////////////////////////////////////////////////////////////////
                    ///                   INFORMATION Mapping
                    ///////////////////////////////////////////////////////////////////
                    
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableMappingInfo;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count == 0)
                    {
                        commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (mapping, updatetime) 
                        VALUES
                        (@Mapping, @UpdateTime)", model);
                    }
                    else
                    {
                        commitResult = await conn.ExecuteAsync(
                            $@"UPDATE {table} 
                            SET
                            mapping = @Mapping, updatetime = @UpdateTime", model);
                    }

                    _log.Info($"({commitResult}) rows was updated in DB[{table}] : Mapping({model.Mapping}ea), UpdateTime({model.UpdateTime})");

                    _mappingInfoModel.Update(model);
                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(UpdateMappingInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(UpdateMappingInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task FetchCameraMapping(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> task = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Caemra Mapping
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableCameraMapping}";

                await Task.Delay(100, token);

                int count = 0;

                foreach (var mapper in (await _dbConnection
                    .QueryAsync<MappingTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;


                    var sensor = _sensorProvider.OfType<SensorDeviceModel>().Where(entity => entity.Id == mapper.Sensor).FirstOrDefault();
                    var preset1 = _presetProvider.OfType<CameraPresetModel>().Where(entity => entity.Id == mapper.FirstPreset).FirstOrDefault();
                    var preset2 = _presetProvider.OfType<CameraPresetModel>().Where(entity => entity.Id == mapper.SecondPreset).FirstOrDefault();

                    var model = ModelFactory.Build<CameraMappingModel>(mapper, sensor, preset1, preset2);
                    _mappingProvider.Add(model);

                    count++;
                }

                if (isFinished)
                    await _mappingProvider.Finished();

                task?.SetResult(true);

            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchCameraMapping)}: " + ex.Message);
                task?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchCameraMapping)}: " + ex.Message);
                task?.SetException(ex);
            }
        }

        public Task SaveCameraMapping(CameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableCameraMapping;

                    if (model != null)
                    {
                        await CheckTable(table, model.Id);

                        foreach (var item in _mappingProvider.ToList())
                        {
                            if (item.Id == model.Id)
                                await _mappingProvider.DeletedItem(item);
                        }
                    }

                    int updatedRecord = 0;
                    var mapper = MapperFactory.Build<MappingTableMapper>(model);

                    commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                    (mapperid, groupid, sensor, firstpreset, secondpreset)  
                    VALUES
                    (@Id, @GroupId, @Sensor, @FirstPreset, @SecondPreset)", mapper);

                    await _mappingProvider.InsertedItem(model);
                    updatedRecord++;

                    _log.Info($"CameraPreset was inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(SaveCameraPreset)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task SaveCameraMappings(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;
            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableCameraMapping;

                    await CheckTable(table);

                    int updatedRecord = 0;
                    foreach (var item in _mappingProvider.ToList())
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        var mapper = MapperFactory.Build<MappingTableMapper>(item);

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                    (mapperid, groupid, sensor, firstpreset, secondpreset)  
                    VALUES
                    (@Id, @GroupId, @Sensor, @FirstPreset, @SecondPreset)", mapper);

                        updatedRecord++;
                    }

                    _log.Info($"CameraDevice was inserted in DB[{table}] : {updatedRecord}ea");

                    if (isFinished)
                        await _mappingProvider.Finished();

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

        public Task<bool> MatchDeviceInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            return Task.Run(() => 
            {
                try
                {
                    var controller = _deviceProvider.OfType<IControllerDeviceModel>().Count();
                    var sensor = _deviceProvider.OfType<ISensorDeviceModel>().Count();
                    var camera = _deviceProvider.OfType<ICameraDeviceModel>().Count();

                    if(controller != _deviceInfoModel.Controller
                        || sensor != _deviceInfoModel.Sensor
                        || camera != _deviceInfoModel.Camera)
                    {
                        tcs?.SetResult(false);
                        return false;
                    }

                    tcs?.SetResult(true);
                    return true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(MatchDeviceInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(MatchDeviceInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
            });
        }

        public Task<bool> MatchMappingInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var mapping = _mappingProvider.Count();

                    if (mapping != _mappingInfoModel.Mapping)
                    {
                        tcs?.SetResult(false);
                        return false;
                    }

                    tcs?.SetResult(true);
                    return true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(MatchMappingInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to insert DB data in {nameof(MatchMappingInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
            });
        }

        public void Finished()
        {
            FetchFinshed?.Invoke();
        }




        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task CheckTable(string table, int? id = null)
        {
            var commitResult = 0;
            var conn = _dbConnection as SQLiteConnection;

            if(id != null)
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

        public async Task<int> GetMaxId()
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
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableDeviceCameraPreset}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableDeviceCameraProfile}
                                UNION ALL
                                SELECT MAX(id) AS max_id FROM {_setupModel.TableCameraMapping}
                            ) AS combined_max_ids;";

                var maxId = (await _dbConnection.QueryAsync<int>(query)).FirstOrDefault();

                return maxId;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsIdExists(int id)
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
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableDeviceCameraPreset} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableDeviceCameraProfile} WHERE id = @Id
                                UNION ALL
                                SELECT 1 FROM {_setupModel.TableCameraMapping} WHERE id = @Id
                            ) AS id_exists;";

                var exists = await _dbConnection.QueryFirstOrDefaultAsync<bool>(query, new { Id = id });

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
                    var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { Id = id, TableName = tableName });

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

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private ILogService _log;
        private DeviceSetupModel _setupModel;
        private DeviceProvider _deviceProvider;
        private ControllerDeviceProvider _controllerProvider;
        private SensorDeviceProvider _sensorProvider;
        private CameraPresetProvider _presetProvider;
        private CameraProfileProvider _profileProvider;
        private CameraMappingProvider _mappingProvider;

        public DeviceInfoModel _deviceInfoModel { get; private set; }

        private MappingInfoModel _mappingInfoModel;

        public delegate void FetchFinished();
        public event FetchFinished FetchFinshed;
        #endregion
    }
}
