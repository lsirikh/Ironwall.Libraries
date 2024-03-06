using Dapper;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Libraries.Enums;
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

namespace Ironwall.Libraries.Devices.Providers
{
    public class DeviceDomainDataProvider
        : TaskService, IDataProviderService
    {

        #region - Ctors -
        public DeviceDomainDataProvider(
            IDbConnection dbConnection
            , DeviceSetupModel setupModel
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            , DeviceDbService deviceDbService
            )
        {
            _dbConnection = dbConnection;
            _setupModel = setupModel;

            _controllerDeviceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeviceProvider = cameraDeviceProvider;


            _deviceDbService = deviceDbService;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task RunTask(CancellationToken token = default)
        {
            return Task.Run(async () => 
            { 
                await Task.Run(() => { BuildSchemeAsync(); });
                //await CreateEntity();
                await FetchAsync();
            });
        }



        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
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

                var cmd = _dbConnection.CreateCommand();

                //Create TableController Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableController} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            deviceid TEXT UNIQUE NOT NULL ,
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
                                            deviceid TEXT UNIQUE NOT NULL ,
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
                                            deviceid TEXT UNIQUE NOT NULL ,
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
                                            optionid TEXT UNIQUE NOT NULL ,
                                            referenceid TEXT NOT NULL ,
                                            profile TEXT
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceCameraPreset Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDeviceCameraPreset} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            optionid TEXT UNIQUE NOT NULL ,
                                            referenceid TEXT NOT NULL ,
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
                                            mapperid TEXT UNIQUE NOT NULL,
                                            groupid TEXT,
                                            sensor TEXT,
                                            firstpreset TEXT,
                                            secondpreset TEXT
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceInfo Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDeviceInfo} (
                                            controller INTEGER,
                                            sensor INTEGER,
                                            camera INTEGER,
                                            updatetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableDeviceInfo Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableMappingInfo} (
                                            mapping INTEGER,
                                            updatetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                //_dbConnection.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private async Task CreateControllerDevice()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();
                /*
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                    deviceid TEXT UNIQUE NOT NULL ,
                    devicenumber INTEGER,
                    devicename TEXT,
                    devicetype INTEGER,
                    version TEXT,
                    status INTEGER,
                    ipaddress TEXT,
                    port INTEGER
                */
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableController;

                var isExist = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                if (isExist > 0)
                    await conn.ExecuteAsync($@"DELETE FROM {table}");

                var id = 0;
                for (int i = 1; i < 10; i++)
                {

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    if (count > 0)
                        id = (await _dbConnection.QueryAsync<int>($@"SELECT max(id) FROM {table}")).FirstOrDefault();


                    //Device Insert SQL
                    var sql = $@"
                                INSERT INTO {table} 
                                (deviceid, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port) 
                                VALUES 
                                ('c{id + 1}', '0', '{i}', '{i}번_제어기', '1', 'v1.0', '1', '192.168.1.{i}', '80')
                                ";

                    //SQL Execution
                    await _dbConnection.ExecuteAsync(sql);
                }

            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine(result);
            }
        }

        private async Task CreateSensorDevice()
        {
            try
            {

                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                /*
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                    deviceid TEXT UNIQUE NOT NULL ,
                    controller INTEGER,  
                    devicenumber INTEGER,
                    devicename TEXT,
                    devicetype INTEGER,
                    version TEXT,
                    status INTEGER
                */

                //Device Insert SQL
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSensor;

                var isExist = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                if (isExist > 0)
                    await conn.ExecuteAsync($@"DELETE FROM {table}");

                var id = 0;
                for (int i = 1; i < 10; i++)
                {
                    for (int j = 1; j < 190; j++)
                    {
                        if (j > 80 && j < 180) continue;

                        var preSql = $@"SELECT COUNT(*) FROM {table} 
                                    WHERE controller = '{i}' AND devicenumber = '{j}';
                                    ";
                        var count = (await _dbConnection.QueryAsync<int>(preSql))?.FirstOrDefault();
                        if (count > 0) continue;

                        count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                        if (count > 0)
                            id = (await _dbConnection.QueryAsync<int>($@"SELECT max(id) FROM {table}")).FirstOrDefault();

                        var deviceType = j >= 180 ? (int)EnumDeviceType.Multi : (int)EnumDeviceType.Fence;

                        Debug.WriteLine($@"('s{id + 1}', '{i}', '{j}', '{j}번_센서', '{deviceType}', 'v1.0', '1')");

                        var sql = $@"INSERT INTO {table} 
                                (deviceid, controller, devicegroup, devicenumber, devicename, devicetype, version, status) 
                                VALUES 
                                ('s{id + 1}', '{i}', '{i}','{j}', '{j}번_센서', '{deviceType}', 'v1.0', '1')
                                ";

                        //SQL Execution
                        await _dbConnection.ExecuteAsync(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine(result);
            }
        }

        private async Task CreateCameraPreset()
        {
            try
            {

                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                /*
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                    cameraid TEXT UNIQUE NOT NULL ,
                    presetname TEXT,
                    ishome INTEGER,
                    pan REAL,
                    tilt REAL,
                    zoom REAL,
                    delay INTEGER
                */

                //Device Insert SQL
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableDeviceCameraPreset;

                var isExist = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                if (isExist > 0)
                    await conn.ExecuteAsync($@"DELETE FROM {table}");

                var id = 1;
                for (int i = 1; i < 10; i++)
                {
                    for(int j = 1; j < 3; j++)
                    {

                        var isHome = j == 1 ? 1 : 0;
                        Debug.WriteLine($@"('v{i}', 'p{j}', '{isHome}' '0.0', '0.0', '0.0', '3' )");

                        var sql = $@"INSERT INTO {table} 
                            (optionid, referenceid, presetname, ishome, pan, tilt, zoom, delay) 
                            VALUES
                            ('o{id++}', 'v{i}', 'p{j}', '{isHome}', '0.0', '0.0', '0.0', '3' )";

                        //SQL Execution
                        await _dbConnection.ExecuteAsync(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine(result);
            }
        }

        private async Task CreateCameraProfile()
        {
            try
            {

                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                /*
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                    cameraid TEXT UNIQUE NOT NULL ,
                    profile TEXT
                */

                //Device Insert SQL
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableDeviceCameraProfile;

                var isExist = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                if (isExist > 0)
                    await conn.ExecuteAsync($@"DELETE FROM {table}");

                var id = 1;
                for (int i = 1; i < 10; i++)
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

                        Debug.WriteLine($@"('v{i}', 'profile_{p}')");
                        var sql = $@"INSERT INTO {table} 
                            (optionid, referenceid, profile) 
                            VALUES
                            ('o{id++}', 'v{i}', 'profile_{p}')";

                        //SQL Execution
                        await _dbConnection.ExecuteAsync(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine(result);
            }
        }

        private async Task CreateCameraDevice()
        {
            try
            {

                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                /*
                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                    deviceid TEXT UNIQUE NOT NULL ,
                    controller INTEGER,  
                    devicenumber INTEGER,
                    devicename TEXT,
                    devicetype INTEGER,
                    version TEXT,
                    status INTEGER
                */

                //Device Insert SQL
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableCamera;

                var isExist = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                if (isExist > 0)
                    await conn.ExecuteAsync($@"DELETE FROM {table}");

                var id = 0;
                for (int i = 1; i < 10; i++)
                {
                    
                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));
                    if (count > 0)
                        id = (await _dbConnection.QueryAsync<int>($@"SELECT max(id) FROM {table}")).FirstOrDefault();


                    Debug.WriteLine($@"('v{id + 1}', '{0}', '{i}', 'Cam{i}', '{(int)EnumDeviceType.IpCamera}', {"v1.0"}, '0', '192.168.202.{i}', '80', 'sensorway', 'pass', 1, 'N/A', 'http://rtsp.url.com', '554','0')");

                    var sql = $@"INSERT INTO {table} 
                        (deviceid, devicegroup, devicenumber, devicename, devicetype, version, status, ipaddress, port, username, password, category, devicemodel, rtspuri, rtspport, mode) 
                        VALUES
                        ('v{id + 1}', '{0}', '{i}', 'Cam{i}', '{(int)EnumDeviceType.IpCamera}', 'v1.0', '0', '192.168.202.{i}', '80', 'sensorway', 'pass', 1, 'N/A', 'http://rtsp.url.com', '554','0')";

                    //SQL Execution
                    await _dbConnection.ExecuteAsync(sql);
                }
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine(result);
            }
        }

        private async Task CreateEntity()
        {

            try
            {
                //제어기 등록
                //await CreateControllerDevice();

                //센서 등록
                //await CreateSensorDevice();

                //프리셋 등록
                await CreateCameraPreset();

                //프로파일 등록
                await CreateCameraProfile();

                //카메라 등록
                await CreateCameraDevice();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task FetchAsync()
        {
            try
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                ///////////////////////////////////////////////////////////////////
                ///                   Device Info
                ///////////////////////////////////////////////////////////////////
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchDeviceInfo)}======================");
                await _deviceDbService.FetchDeviceInfo();
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchDeviceInfo)}======================");
                ///////////////////////////////////////////////////////////////////
                ///                   Camera Device
                ///////////////////////////////////////////////////////////////////
                await _deviceDbService.FetchCameraPreset(isFinished: true);
                await _deviceDbService.FetchCameraProfile(isFinished: true);
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchCameraDevice)}======================");
                await _deviceDbService.FetchCameraDevice(isFinished: false);
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchCameraDevice)}======================");

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////
                
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchControllerDevice)}======================");
                await _deviceDbService.FetchControllerDevice(isFinished: true);
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchControllerDevice)}======================");

                ///////////////////////////////////////////////////////////////////
                ///                   Sensor Device
                ///////////////////////////////////////////////////////////////////
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchSensorDevice)}======================");
                await _deviceDbService.FetchSensorDevice(isFinished: true);
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchSensorDevice)}======================");

                var matchResult = await _deviceDbService.MatchDeviceInfo();

                if (!matchResult)
                {
                    await _deviceDbService.DeleteDeviceInfo();
                }
                
                ///////////////////////////////////////////////////////////////////
                ///                   Mapping Info
                ///////////////////////////////////////////////////////////////////
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchMappingInfo)}======================");
                await _deviceDbService.FetchMappingInfo(tcs:tcs);
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchMappingInfo)}======================");
                if (tcs.Task.Result)
                {
                    Debug.WriteLine($"{nameof(MappingInfoModel)}를 정상적으로 불러왔다.");
                }
                else
                {
                    Debug.WriteLine($"{nameof(MappingInfoModel)}가 존재하지 않는다.");
                }

                ///////////////////////////////////////////////////////////////////
                ///                   Camera Mapping
                ///////////////////////////////////////////////////////////////////
                Debug.WriteLine($"==================Start:{nameof(_deviceDbService.FetchCameraMapping)}======================");
                await _deviceDbService.FetchCameraMapping(isFinished: true);
                Debug.WriteLine($"==================Finish:{nameof(_deviceDbService.FetchCameraMapping)}======================");

                matchResult = await _deviceDbService.MatchMappingInfo();
                if (!matchResult)
                {
                    await _deviceDbService.DeleteMappingInfo();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchAsync)}: " + ex.Message);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        #endregion
        #region - Attributes -
        private ControllerDeviceProvider _controllerDeviceProvider;
        private SensorDeviceProvider _sensorDeviceProvider;
        private CameraDeviceProvider _cameraDeviceProvider;
        private DeviceDbService _deviceDbService;
        private IDbConnection _dbConnection;
        private DeviceSetupModel _setupModel;

        public delegate void FetchFinished();
        #endregion


    }
}
