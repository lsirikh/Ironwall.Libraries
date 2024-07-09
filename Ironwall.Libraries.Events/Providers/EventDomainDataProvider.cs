using Dapper;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Devices.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Devices.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Events.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public class EventDomainDataProvider
        : TaskService, IDataProviderService
    {
        #region - Ctors -
        public EventDomainDataProvider(
            IDbConnection dbConnection
            , EventSetupModel setupModel
            , DeviceSetupModel deviceSetupModel
            , DetectionEventProvider detectionEventProvider
            , MalfunctionEventProvider malfunctionEventProvider
            , ConnectionEventProvider connectionEventProvider
            , ControllerDeviceProvider controllerDeviceProvider
            , ActionEventProvider actionEventProvider

            , EventProvider eventProvider
            , SensorDeviceProvider sensorDeviceProvider

            , EventDbService eventDbService
            //, DeviceDbService deviceDbService
            )
        {
            _dbConnection = dbConnection;
            _setupModel = setupModel;
            _deviceSetupModel = deviceSetupModel;

            _detectionEventProvider = detectionEventProvider;
            _malfunctionEventProvider = malfunctionEventProvider;
            _connectionEventProvider = connectionEventProvider;
            _eventProvider = eventProvider;
            _actionEventProvider = actionEventProvider;


            _controllerProvider = controllerDeviceProvider;
            _sensorProvider = sensorDeviceProvider;

            _eventDbService = eventDbService;
            //_deviceDbService = deviceDbService;

            //_deviceDbService.FetchFinshed += DeviceDbService_FetchFinished;
        }

        //private void DeviceDbService_FetchFinished()
        //{
        //    _cts.Cancel();
        //}




        #endregion
        #region - Implementation of Interface -

        #endregion
        #region - Overrides -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await Task.Run(delegate
            {
                BuildSchemeAsync();
            }).ContinueWith(async delegate
                {
                    await FetchAsync();
                }, TaskContinuationOptions.ExecuteSynchronously, token);
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

                //Create Connection Event DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableConnection} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            
                                            eventid TEXT UNIQUE NOT NULL ,
                                            eventgroup TEXT,
                                            device TEXT,
                                            messagetype INTEGER,

                                            status INTEGER,
                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();


                //Create Detection Event DB Table
                //cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                //                            id TEXT PRIMARY KEY NOT NULL,
                //                            eventgroup TEXT,
                //                            controller INTEGER,
                //                            sensor INTEGER,
                //                            unittype INTEGER,
                //                            result INTEGER,
                //                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                //                           )";

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableDetection} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            
                                            eventid TEXT UNIQUE NOT NULL ,
                                            eventgroup TEXT,
                                            device TEXT,
                                            messagetype INTEGER,

                                            result INTEGER,
                                            status INTEGER,
                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();

                //Create Malfunction Event DB Table
                //cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {tableUser} (
                //                            id TEXT PRIMARY KEY NOT NULL,
                //                            eventgroup TEXT,
                //                            controller INTEGER,
                //                            sensor INTEGER,
                //                            unittype INTEGER,
                //                            reason INTEGER, 
                //                            firststart INTEGER,  
                //                            firstend INTEGER, 
                //                            secondstart INTEGER, 
                //                            secondend INTEGER,
                //                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                //                        )";

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableMalfunction} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,

                                            eventid TEXT UNIQUE NOT NULL ,
                                            eventgroup TEXT,
                                            device TEXT,
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


                //Create Connection Event DB Table
                /*dbTable = _setupModel.TableConnection;
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            userid TEXT NOT NULL,
                                            userlevel INTEGER,
                                            clientid INTEGER,
                                            mode INTEGER,
                                            timecreated DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();*/

                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableAction} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            eventid TEXT UNIQUE NOT NULL ,
                                            
                                            
                                            fromeventid TEXT UNIQUE NOT NULL ,
                                            content TEXT,
                                            user TEXT,

                                            datetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Task CreateEntity()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();


                    //탐지 등록
                    await Task.Run(async () =>
                    {
                        try
                        {
                            if (_dbConnection.State != ConnectionState.Open)
                                await (_dbConnection as DbConnection).OpenAsync();

                            for (int i = 1; i < 4; i++)
                            {
                                /*var preSql = $@"
                                        SELECT COUNT(*) FROM { _setupModel.TableDetection}
                                        WHERE eventid = 'd{i}'
                                        ";
                                var count = (await _conn.QueryAsync<int>(preSql))?.FirstOrDefault();
                                if (count > 0)
                                    continue;*/

                                var id = 0;
                                try
                                {
                                    id = (await _dbConnection.QueryAsync<int>(
                                        $@"
                                            SELECT max(id) 
                                            FROM {_setupModel.TableDetection} 
                                          ")).FirstOrDefault();
                                    //WHERE id = max(id)
                                }
                                catch
                                {
                                }


                                //Detection Event Insert SQL
                                var sql = $@"
                                        INSERT INTO {_setupModel.TableDetection} 
                                        (eventid, eventgroup, device, messagetype, result, datetime) 
                                        VALUES 
                                        ('d{id + 1}', '{i}', 's{i}', '90', '6', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}')
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
                    });


                    //조치보고 등록
                    await Task.Run(async () =>
                    {
                        try
                        {
                            if (_dbConnection.State != ConnectionState.Open)
                                await (_dbConnection as DbConnection).OpenAsync();

                            //Device Insert SQL
                            for (int i = 1; i < 4; i++)
                            {
                                var sql = $@"
                                        INSERT INTO {_setupModel.TableAction} 
                                        (eventid, fromeventid, content, user, datetime) 
                                        VALUES 
                                        ('{IdCodeGenerator.GenIdCode()}', 'd{i}', '{i}테스트', 'sensorway', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}')
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
                    });
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }

        private async Task FetchAsync()
        {
            try
            {
                var startDate = (DateTime.Now - TimeSpan.FromDays(1)).ToString("yyyy-MM-dd HH:mm:ss");
                var endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                ///////////////////////////////////////////////////////////////////
                ///                   Detection Event
                ///////////////////////////////////////////////////////////////////
                // MessageType을 통해서 Detection Event 식별
                await _eventDbService.FetchDetectionEvents(from: startDate, to: endDate);

                ///////////////////////////////////////////////////////////////////
                ///                   Malfunction Event
                ///////////////////////////////////////////////////////////////////
                // MessageType을 통해서 Detection Event 식별
                await _eventDbService.FetchMalfunctionEvents(from: startDate, to: endDate);


                ///////////////////////////////////////////////////////////////////
                ///                   Fetch Action Event
                ///////////////////////////////////////////////////////////////////
                //MessageType을 통해서 Action for Detection과 Action for Malfunction을 구분
                await _eventDbService.FetchActionEvents(from: startDate, to: endDate);

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
        private EventSetupModel _setupModel;
        private DeviceSetupModel _deviceSetupModel;
        private IDbConnection _dbConnection;

        private DetectionEventProvider _detectionEventProvider;
        //private ActionDetectionEventProvider _actionDetectionEventProvider;
        //private ActionMalfunctionEventProvider _actionMalfunctionEventProvider;

        private MalfunctionEventProvider _malfunctionEventProvider;
        private ConnectionEventProvider _connectionEventProvider;
        private EventProvider _eventProvider;
        private ActionEventProvider _actionEventProvider;
        private ControllerDeviceProvider _controllerProvider;
        private SensorDeviceProvider _sensorProvider;
        private EventDbService _eventDbService;
        private DeviceDbService _deviceDbService;

        //private CancellationTokenSource _cts;
        #endregion

    }
}
