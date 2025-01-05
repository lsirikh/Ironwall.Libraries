using Caliburn.Micro;
using Dapper;
using Ironwall.Libraries.RTSP.DataProviders;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Services
{
    public class CameraDbService
    {

        #region - Ctors -
        public CameraDbService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);

            SetupModel = IoC.Get<CameraSetupModel>();

            DbConnection = IoC.Get<IDbConnection>();
            DeviceProvider = IoC.Get<CameraDeviceProvider>();
            PresetProvider = IoC.Get<CameraPresetProvider>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task SaveDevice()
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TableCameraDevice;

                    //DB 내용 DELETE
                    var sql = @$"DELETE FROM {table}";
                    commitResult = conn.Execute(sql);

                    ////DB 레코드 INSERT
                    //foreach (var item in DeviceProvider)
                    //{
                    //    commitResult = conn.Execute(@$"INSERT INTO {table} 
                    //                (id, name, typedevice, ipaddress, port, username, password, firmwareversion, hardwareid, devicemodel, serialnumber, manufacturer, profile, uri, type, hostname, rtspuri, rtspport, mac, mode, used) VALUES (@Id, @Name, @TypeDevice, @IpAddress, @Port, @UserName, @Password, @FirmwareVersion, @HardwareId, @DeviceModel, @SerialNumber, @Manufacturer, @Profile, @Uri, @Type, @HostName, @RtspUri, @RtspPort, @Mac, @Mode, 1)", item);

                    //    commitCount += commitResult;
                    //}

                    foreach (var item in DeviceProvider.CollectionEntity)
                    {
                        var parameters = new
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Typedevice = item.TypeDevice,
                            IpAddress = item.IpAddress,
                            Port = item.Port,
                            UserName = item.UserName,
                            Password = item.Password,
                            FirmwareVersion = item.FirmwareVersion,
                            HardwareId = item.HardwareId,
                            DeviceModel = item.DeviceModel,
                            SerialNumber = item.SerialNumber,
                            Manufacturer = item.Manufacturer,
                            Profile = item.Profile,
                            Uri = item.Uri,
                            Type = item.Type,
                            HostName = item.HostName,
                            RtspUri = item.RtspUri,
                            RtspPort = item.RtspPort,
                            Mac = item.Mac,
                            Mode = item.Mode
                        };

                        commitResult = conn.Execute(@$"INSERT INTO {table} 
                        (id, name, typedevice, ipaddress, port, username, password, firmwareversion, hardwareid, devicemodel, serialnumber, manufacturer, profile, uri, type, hostname, rtspuri, rtspport, mac, mode, used) 
                        VALUES 
                        (@Id, @Name, @Typedevice, @IpAddress, @Port, @UserName, @Password, @FirmwareVersion, @HardwareId, @DeviceModel, @SerialNumber, @Manufacturer, @Profile, @Uri, @Type, @HostName, @RtspUri, @RtspPort, @Mac, @Mode, 1)",
                                    parameters);

                        commitCount += commitResult;
                    }

                    Debug.WriteLine($"({commitCount}) rows was updated in DB[{table}]");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Task to insert DB data Error: " + ex.Message);
                }
            });

        }

        public async Task FetchDevice()
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (DbConnection.State != ConnectionState.Open)
                        await (DbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    //                           CameraDevice
                    ///////////////////////////////////////////////////////////////////
                    var sql = @$"SELECT * FROM {SetupModel.TableCameraDevice}";

                    //DeviceProvider 초기화
                    await Task.Run(() => DeviceProvider.Clear());

                    /// 로직 설명 생략
                    await Task.Run(() =>
                    {
                        foreach (var viewmodel in (DbConnection
                            .Query<CameraDeviceModel>(sql)
                            .Select((item) => new CameraDeviceViewModel(item))))
                        {
                            if (DeviceProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
                                continue;

                            DeviceProvider.Add(viewmodel);
                        };
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchDevice: {ex.Message}");
                }
            });
        }

        public async Task SavePreset()
        {
            int commitResult = 0;
            int commitCount = 0;

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var conn = DbConnection as SQLiteConnection;
                    var table = SetupModel.TableCameraPreset;

                    //DB 내용 DELETE
                    var sql = @$"DELETE FROM {table}";
                    commitResult = conn.Execute(sql);

                    //DB 레코드 INSERT
                    foreach (var item in PresetProvider.CollectionEntity)
                    {
                        commitResult = conn.Execute(@$"INSERT INTO {table} 
                                    (id, namearea, idcontroller, idsensorbgn, idsensorend, camerafirst, typedevicefirst, homepresetfirst, targetpresetfirst, camerasecond, typedevicesecond, homepresetsecond, targetpresetsecond, controltime, used) VALUES (@Id, @NameArea, @IdController, @IdSensorBgn, @IdSensorEnd, @CameraFirst,  @TypeDeviceFirst, @HomePresetFirst, @TargetPresetFirst, @CameraSecond,  @TypeDeviceSecond, @HomePresetSecond, @TargetPresetSecond, @ControlTime, 1)", item);

                        commitCount += commitResult;
                    }
                    Debug.WriteLine($"({commitCount}) rows was updated in DB[{table}]");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine("Task to insert DB data Error: " + ex.Message);
                }
            });

        }

        public async Task FetchPreset()
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (DbConnection.State != ConnectionState.Open)
                        await (DbConnection as DbConnection).OpenAsync();

                    ///////////////////////////////////////////////////////////////////
                    //                           CameraPreset
                    ///////////////////////////////////////////////////////////////////
                    var sql = @$"SELECT * FROM {SetupModel.TableCameraPreset}";

                    //DeviceProvider 초기화
                    await Task.Run(() => PresetProvider.Clear());

                    /// 로직 설명 생략
                    await Task.Run(() =>
                    {
                        foreach (var viewmodel in (DbConnection
                            .Query<CameraPresetModel>(sql)
                            .Select((item) => new CameraPresetViewModel(item))))
                        {
                            if (PresetProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
                                continue;

                            PresetProvider.Add(viewmodel);
                        };
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FetchPreset: {ex.Message}");
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public CameraSetupModel SetupModel { get; }
        public IDbConnection DbConnection { get; }
        public CameraDeviceProvider DeviceProvider { get; }
        public CameraPresetProvider PresetProvider { get; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
