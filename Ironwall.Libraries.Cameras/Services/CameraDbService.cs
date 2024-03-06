using Caliburn.Micro;
using Dapper;
using Ironwall.Libraries.Cameras.Providers;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Cameras.Providers.Models;
using System.Diagnostics.Eventing;
using Newtonsoft.Json.Linq;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Cameras.Services
{
    public class CameraDbService
    {

        #region - Ctors -
        public CameraDbService(
            IEventAggregator eventAggregator
            , ILogService log
            , IDbConnection dbConnection
            , CameraSetupModel cameraSetupModel
            , CameraDeviceProvider deviceProvider
            , CameraPresetProvider presetProvider)
        {
            _log = log;
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);

            SetupModel = cameraSetupModel;
            _dbConnection = dbConnection;
            _deviceProvider = deviceProvider;
            _presetProvider = presetProvider;

            //DeviceProvider = IoC.Get<CameraDeviceDataProvider>();
            //PresetProvider = IoC.Get<CameraPresetDataProvider>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task FetchDevice(CancellationToken token = default, bool isFinished = false)
        {
            await Task.Run(async () =>
            {
                try
                {
                    ///////////////////////////////////////////////////////////////////
                    //                           CameraDevice
                    ///////////////////////////////////////////////////////////////////
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    var sql = $@"SELECT * FROM {SetupModel.TableCameraDevice}";

                    await Task.Delay(100, token);
                    /// 로직 설명 생략
                    foreach (var viewmodel in (await _dbConnection
                            .QueryAsync<CameraDeviceModel>(sql)).ToList())
                            //.Select((item) => new CameraDeviceViewModel(item))))
                    {
                        if (token.IsCancellationRequested)
                            break;

                        _deviceProvider.Add(viewmodel);
                    };

                    if (isFinished)
                        await _deviceProvider.Finished();
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(FetchDevice)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to fetch DB data in {nameof(FetchDevice)}: {ex.Message}");
                }
            }, token);
        }

        //public async Task FetchDevice(CancellationToken token = default, bool isFinished = false)
        //{
        //    await Task.Run(async () =>
        //    {
        //        try
        //        {
        //            ///////////////////////////////////////////////////////////////////
        //            //                           CameraDevice
        //            ///////////////////////////////////////////////////////////////////
        //            if (_dbConnection.State != ConnectionState.Open)
        //                await (_dbConnection as DbConnection).OpenAsync();

        //            var sql = $@"SELECT * FROM {SetupModel.TableCameraDevice}";

        //            //DeviceProvider 초기화
        //            await Task.Run(() => DeviceProvider.Clear(), token);

        //            /// 로직 설명 생략
        //            await Task.Run(() =>
        //            {
        //                foreach (var viewmodel in (_dbConnection
        //                    .Query<CameraDeviceModel>(sql)
        //                    .Select((item) => new CameraDeviceViewModel(item))))
        //                {
        //                    if (token.IsCancellationRequested)
        //                        break;

        //                    if (DeviceProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
        //                        continue;

        //                    DeviceProvider.Add(viewmodel);
        //                };
        //            }, token);
        //        }
        //        catch (TaskCanceledException ex)
        //        {
        //            Debug.WriteLine($"Task was cancelled in {nameof(FetchDevice)}: " + ex.Message);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception for Task to fetch DB data in {nameof(FetchDevice)}: {ex.Message}");
        //        }
        //    }, token);
        //}

        public async Task SaveDevice(CancellationToken token = default, bool isFinished = false)
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = SetupModel.TableCameraDevice;

                    //DB 내용 DELETE
                    var sql = $@"DELETE FROM {table}";
                    commitResult = await conn.ExecuteAsync(sql);

                    //DB 레코드 INSERT
                    foreach (var item in _deviceProvider.ToList())
                    {
                        if (token.IsCancellationRequested)
                            break;

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                    (id, name, typedevice, ipaddress, port, username, password, firmwareversion, hardwareid, devicemodel, serialnumber, manufacturer, profile, uri, type, hostname, rtspuri, rtspport, mac, mode, used) 
                                    VALUES
                                    (@Id, @Name, @Typedevice, @IpAddress, @Port, @UserName, @Password, @FirmwareVersion, @HardwareId, @DeviceModel, @SerialNumber, @Manufacturer, @Profile, @Uri, @Type, @HostName, @RtspUri, @RtspPort, @Mac, @Mode, 1)", item);

                        commitCount += commitResult;
                    }

                    if (isFinished)
                        await _deviceProvider.Finished();

                    _log.Info($"({commitCount}) rows was updated in DB[{table}]");
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SaveDevice)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to fetch DB data in {nameof(SaveDevice)}: {ex.Message}");
                }
            }, token);

        }
        
        public async Task FetchPreset(CancellationToken token = default, bool isFinished = false)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    //                           CameraPreset
                    ///////////////////////////////////////////////////////////////////
                    var sql = $@"SELECT * FROM {SetupModel.TableCameraPreset}";

                    await Task.Delay(100, token);

                    /// 로직 설명 생략
                    foreach (var viewmodel in (await _dbConnection
                        .QueryAsync<CameraPresetModel>(sql)).ToList())
                        //.Select((item) => new CameraPresetViewModel(item))))
                    {
                        if (token.IsCancellationRequested)
                            break;

                        if (_presetProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
                            continue;

                        _presetProvider.Add(viewmodel);
                    };

                    if (isFinished)
                        await _presetProvider.Finished();
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(FetchPreset)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to fetch DB data in {nameof(FetchPreset)}: {ex.Message}");
                }
            }, token);
        }

        public async Task SavePreset(CancellationToken token = default, bool isFinished = false)
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = SetupModel.TableCameraPreset;

                    //DB 내용 DELETE
                    var sql = $@"DELETE FROM {table}";
                    commitResult = await conn.ExecuteAsync(sql);

                    //DB 레코드 INSERT
                    foreach (var item in _presetProvider.ToList())
                    {
                        if (token.IsCancellationRequested)
                            break;

                        commitResult = conn.Execute($@"INSERT INTO {table} 
                                    (id, namearea, idcontroller, idsensorbgn, idsensorend, camerafirst, typedevicefirst, homepresetfirst, targetpresetfirst, camerasecond, typedevicesecond, homepresetsecond, targetpresetsecond, controltime, used) VALUES (@Id, @NameArea, @IdController, @IdSensorBgn, @IdSensorEnd, @CameraFirst,  @TypeDeviceFirst, @HomePresetFirst, @TargetPresetFirst, @CameraSecond,  @TypeDeviceSecond, @HomePresetSecond, @TargetPresetSecond, @ControlTime, 1)", item);

                        commitCount += commitResult;
                    }

                    if (isFinished)
                        await _presetProvider.Finished();

                    _log.Info($"({commitCount}) rows was updated in DB[{table}]");
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Task was cancelled in {nameof(SavePreset)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception for Task to fetch DB data in {nameof(SavePreset)}: {ex.Message}");
                }
            }, token);

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public CameraSetupModel SetupModel { get; }
        //public CameraDeviceDataProvider DeviceProvider { get; }
        //public CameraPresetDataProvider PresetProvider { get; }
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private CameraDeviceProvider _deviceProvider;
        private CameraPresetProvider _presetProvider;
        private ILogService _log;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
