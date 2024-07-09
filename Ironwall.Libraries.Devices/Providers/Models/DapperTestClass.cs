using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Providers
{
    public class DapperTestClass
    { }
    /*: TaskService, IDataProviderService
    {

        #region - Ctors -
        public DeviceDomainDataProvider(
            IDbConnection dbConnection
            , DeviceSetupModel setupModel
            , ControllerDeviceProvider controllerDeviceProvider
            , SensorDeviceProvider sensorDeviceProvider
            , CameraDeviceProvider cameraDeviceProvider
            )
        {
            _conn = dbConnection;
            _setupModel = setupModel;

            _controllerDeivceProvider = controllerDeviceProvider;
            _sensorDeviceProvider = sensorDeviceProvider;
            _cameraDeviceProvider = cameraDeviceProvider;
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
                    CreateEntity();
                }, TaskContinuationOptions.ExecuteSynchronously, token)
                .ContinueWith(delegate
                {
                    FetchAsync();
                }, TaskContinuationOptions.ExecuteSynchronously, token);
        }



        public override void Stop()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async void BuildSchemeAsync()
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                using var cmd = _conn.CreateCommand();

                //Create TableDevice Device DB Table
                var dbTable = _setupModel.TableDevice;

                //관계형 DB 타입
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            devicenumber INTEGER,
                                            devicename TEXT,
                                            devicetype INTEGER,
                                            version TEXT,
                                            status INTEGER
                                           )";

                //비관계형 DB 타입
                *//*cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            devicenumber INTEGER,
                                            devicename TEXT,
                                            devicetype INTEGER,
                                            version TEXT,
                                            status INTEGER
                                           )";*//*
                cmd.ExecuteNonQuery();

                //Create TableController Device DB Table
                dbTable = _setupModel.TableController;
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            controller INTEGER NOT NULL UNIQUE,
                                            ipaddress TEXT,
                                            port INTEGER,
                                            FOREIGN KEY(controller) REFERENCES {_setupModel.TableDevice}(id)
	                                        ON UPDATE CASCADE ON DELETE CASCADE
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableSensor Device DB Table
                dbTable = _setupModel.TableSensor;
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            sensor INTEGER NOT NULL UNIQUE, 
                                            controller INTEGER NOT NULL UNIQUE,
                                            FOREIGN KEY(sensor) REFERENCES {_setupModel.TableDevice}(id)
	                                        ON UPDATE CASCADE ON DELETE CASCADE
                                            FOREIGN KEY(controller) REFERENCES {_setupModel.TableController}(id)
	                                        ON UPDATE CASCADE ON DELETE CASCADE
                                           )";
                cmd.ExecuteNonQuery();

                //Create TableCamera Device DB Table
                *//*dbTable = _setupModel.TableCamera;
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            camera INTEGER NOT NULL UNIQUE,  
                                            ipaddress TEXT,
                                            port INTEGER,
                                            FOREIGN KEY(camera) REFERENCES {_setupModel.TableDevice}(id)
	                                        ON UPDATE CASCADE ON DELETE CASCADE
                                           )";
                cmd.ExecuteNonQuery();*//*

                _conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private async void CreateEntity()
        {
            try
            {
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                //제어기 등록
                await Task.Run(async () =>
                {
                    try
                    {
                        var conn = _conn as SQLiteConnection;

                        for (int i = 1; i < 10; i++)
                        {
                            //Device Id Create
                            int id = 1;
                            try
                            {
                                id = (await _conn
                                        .QueryAsync<int>($"SELECT MAX(id) FROM {_setupModel.TableDevice};"))
                                        .FirstOrDefault() + 1;
                            }
                            catch
                            {
                            }


                            //Device Insert SQL
                            var sql = $@"insert into {_setupModel.TableDevice} 
                                    (id, devicenumber, devicename, devicetype, version, status) 
                                    values 
                                    ('{id}', '{i}', 'controller', '1', 'v1.0', '1')";

                            //SQL Execution
                            await conn.ExecuteAsync(sql);

                            //Get controller Id
                            var conId = (await _conn
                                        .QueryAsync<int>($@"
                                        SELECT d.id 
                                        FROM {_setupModel.TableDevice} d
                                        WHERE d.devicenumber = '{id}' AND d.devicetype = '1'"))
                                        .FirstOrDefault();

                            //Sensor Insert SQL
                            sql = $@"INSERT INTO {_setupModel.TableController} 
                                        (controller, ipaddress, port) 
                                        VALUES 
                                        ('{conId}', '192.168.1.{i}', '80')";
                            await conn.ExecuteAsync(sql);
                        }

                    }
                    catch (Exception ex)
                    {
                        var result = ex.Message;
                        Debug.WriteLine(result);
                    }
                });


                //센서 등록
                await Task.Run(async () =>
                {
                    try
                    {
                        var conn = _conn as SQLiteConnection;

                        for (int i = 1; i < 10; i++)
                        {
                            for (int j = 1; j < 10; j++)
                            {
                                //Device Id Create
                                int id = 1;
                                try
                                {
                                    id = (await _conn
                                            .QueryAsync<int>($"SELECT MAX(id) FROM {_setupModel.TableDevice};"))
                                            .FirstOrDefault() + 1;
                                }
                                catch
                                {
                                }

                                //Device Insert SQL
                                var sql = $@"insert into {_setupModel.TableDevice} 
                                    (id, devicenumber, devicename, devicetype, version, status) 
                                    values 
                                    ('{id}', '{j}', '센서', '3', 'v0.1', '1')";

                                //SQL Execution
                                await conn.ExecuteAsync(sql);

                                //Get controller Id
                                var conId = (await _conn
                                            .QueryAsync<int>($@"
                                        SELECT d.id 
                                        FROM {_setupModel.TableController} d
                                        WHERE d.id = '{i}' AND d.devicetype = '1'"))
                                            .FirstOrDefault();

                                //Sensor Insert SQL
                                sql = $@"INSERT INTO {_setupModel.TableSensor} 
                                        (sensor, controller) 
                                        VALUES 
                                        ('{id}', '{conId}')";

                                await conn.ExecuteAsync(sql);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var result = ex.Message;
                        Debug.WriteLine(result);
                    }
                });

                _conn.Close();
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
                if (_conn.State != ConnectionState.Open)
                    await (_conn as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   CONTROLLER Device
                ///////////////////////////////////////////////////////////////////

                //DeviceType에 따라서 다른 방식으로 Fetch해온다.

                var sql = $@"
                            SELECT * FROM {_setupModel.TableController}  c
                            INNER JOIN {_setupModel.TableDevice} d ON d.id = c.controller
                            ";

                foreach (var model in (_conn
                    .Query<ControllerDeviceModel>(sql).ToList()))
                {
                    _controllerDeivceProvider.Add(model);
                }

                ///////////////////////////////////////////////////////////////////
                ///                   Body Device
                ///////////////////////////////////////////////////////////////////

                //DeviceType에 따라서 다른 방식으로 Fetch해온다.

                sql = $@"
                        SELECT * FROM {_setupModel.TableDevice} d
                        INNER JOIN {_setupModel.TableCamera} a ON d.id = a.camera
                        WHERE devicetype = '10'";

                foreach (var model in (_conn
                    .Query<CameraDeviceModel>(sql).ToList()))
                {
                    _cameraDeviceProvider.Add(model);
                }

                ///////////////////////////////////////////////////////////////////
                ///                   Sensor Device
                ///////////////////////////////////////////////////////////////////
                *//*sql = $@"SELECT * FROM {_setupModel.TableSensor}";

                foreach (var model in (_conn
                    .Query<SensorDeviceModel>(sql).ToList()))
                {
                    _sensorDeviceProvider.Add(model);
                }*//*

                //DeviceType에 따라서 다른 방식으로 Fetch해온다.

                //sql = $@"SELECT  FROM {_setupModel.TableSensor} s
                //         INNER JOIN {_setupModel.TableController} c ON s.controller = c.devicenumber";
                *//*sql = $@"SELECT d.id, d.devicenumber, d.devicename, d.devicetype, d.version, d.status, s.controller
                         FROM {_setupModel.TableSensor} s
                         INNER JOIN {_setupModel.TableDevice} d ON d.id = s.sensor
                         INNER JOIN {_setupModel.TableController} c ON c.id = s.controller ;
                         ";*//*

                sql = $@"SELECT *
                         FROM {_setupModel.TableDevice} d 
                         LEFT JOIN {_setupModel.TableController} c ON c.controller = d.id
                         LEFT JOIN {_setupModel.TableSensor} s ON d.id = s.sensor
                         ;";

                var dt = new DataTable();
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT *
                         FROM {_setupModel.TableDevice} d 
                         LEFT JOIN {_setupModel.TableController} c ON c.controller = d.id
                         LEFT JOIN {_setupModel.TableSensor} s ON d.id = s.sensor
                         ;";
                    dt.Load(cmd.ExecuteReader());
                }

                foreach (DataRow item in dt.Rows)
                {
                    Debug.WriteLine($"{item["id"] as int?}");
                }

                *//*foreach (var model in await (_conn
                    .QueryAsync<SensorDeviceModel, ControllerDeviceModel, SensorDeviceModel>(sql, (sensor, controller) =>
                    //.QueryAsync<SensorDeviceModel>(sql)))
                    {
                        sensor.ControllerDeviceModel = controller;
                        return sensor;
                    }, splitOn: "id")))
                {
                    _sensorDeviceProvider.Add(model);
                }*//*


                _conn.Close();
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
        public DeviceSetupModel _setupModel { get; }

        #endregion
        #region - Attributes -
        private ControllerDeviceProvider _controllerDeivceProvider;
        private SensorDeviceProvider _sensorDeviceProvider;
        private CameraDeviceProvider _cameraDeviceProvider;
        private IDbConnection _conn;
        #endregion


    }*/
}
