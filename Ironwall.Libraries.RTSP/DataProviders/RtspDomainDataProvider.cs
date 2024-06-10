using Caliburn.Micro;
using Dapper;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.DataProviders
{
    public class RtspDomainDataProvider
        : TaskService, IDataProviderService
    {
        #region - Ctors -
        public RtspDomainDataProvider()
        {
            SetupModel = IoC.Get<CameraSetupModel>();
            _dbConnection = IoC.Get<IDbConnection>();
            _eventAggregator = IoC.Get<IEventAggregator>();
            _cameraDeviceProvider = IoC.Get<CameraDeviceProvider>();
            _cameraPresetProvider = IoC.Get<CameraPresetProvider>();
        }



        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await Task.Run(delegate {
                BuildSchemeAsync();
            })
                .ContinueWith(delegate {
                    FetchAsync();
                }, TaskContinuationOptions.ExecuteSynchronously, token);
        }

        protected override Task ExitTask(CancellationToken token=default)
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

                using var cmd = _dbConnection.CreateCommand();

                //Create CameraDevice DB Table
                var dbTable = SetupModel.TableCameraDevice;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            name TEXT,
                                            typedevice INTEGER,
                                            ipaddress TEXT,
                                            port INTEGER,
                                            username TEXT,
                                            password TEXT,

                                            firmwareversion TEXT,
                                            hardwareid TEXT,
                                            devicemodel TEXT,
                                            manufacturer TEXT,
                                            serialnumber TEXT,

                                            profile INTEGER,
                                            uri TEXT,
                                            type TEXT,
                                            
                                            hostname TEXT,
                                            rtspuri TEXT,
                                            rtspport INTEGER,
                                            mac TEXT,
                                            mode INTEGER,

                                            used BOOLEAN DEFAULT TRUE,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();


                //Create CameraPreset DB Table
                dbTable = SetupModel.TableCameraPreset;
                cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {dbTable} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            namearea TEXT,

                                            idcontroller INTEGER,
                                            idsensorbgn INTEGER,
                                            idsensorend INTEGER,

                                            camerafirst TEXT,
                                            typedevicefirst INTEGER,
                                            homepresetfirst TEXT,
                                            targetpresetfirst TEXT,

                                            camerasecond TEXT,
                                            typedevicesecond INTEGER,
                                            homepresetsecond TEXT,
                                            targetpresetsecond TEXT,

                                            controltime INTEGER,

                                            used BOOLEAN DEFAULT TRUE,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"BuildSchemeAsync: {ex.Message}");
            }
        }

        private async void FetchAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                //                           CameraDevice
                ///////////////////////////////////////////////////////////////////
                var sql = @$"SELECT * FROM {SetupModel.TableCameraDevice}";


                /// 로직 설명 생략
                foreach (var viewmodel in (_dbConnection
                    .Query<CameraDeviceModel>(sql)
                    .Select((item) => new CameraDeviceViewModel(item))))
                {
                    _cameraDeviceProvider.Add(viewmodel);
                };

                ///////////////////////////////////////////////////////////////////
                //                           CameraPreset
                ///////////////////////////////////////////////////////////////////
                sql = @$"SELECT * FROM {SetupModel.TableCameraPreset}";

                foreach (var viewmodel in (_dbConnection
                            .Query<CameraPresetModel>(sql)
                            .Select((item) => new CameraPresetViewModel(item))))
                {
                    if (_cameraPresetProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
                        continue;

                    _cameraPresetProvider.Add(viewmodel);
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FetchAsync: {ex.Message}");
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public CameraSetupModel SetupModel { get; }
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private IEventAggregator _eventAggregator;
        private CameraDeviceProvider _cameraDeviceProvider;
        private CameraPresetProvider _cameraPresetProvider;
        #endregion
    }
}
