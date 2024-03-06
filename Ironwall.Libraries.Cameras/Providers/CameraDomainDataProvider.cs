using Caliburn.Micro;
using Dapper;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.Providers.Models;
using Ironwall.Libraries.Cameras.Services;
using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Providers
{
    public class CameraDomainDataProvider
        : TaskService, IDataProviderService
    {
        #region - Ctors -
        public CameraDomainDataProvider(
            IEventAggregator eventAggregator
            , CameraSetupModel cameraSetupModel
            , IDbConnection dbConnection
            , CameraDeviceProvider cameraDeviceProvider
            , CameraPresetProvider cameraPresetProvider
            , CameraDbService cameraDbService
            )
        {
            SetupModel = cameraSetupModel;
            _dbConnection = dbConnection;
            _eventAggregator = eventAggregator;
            //_cameraDeviceProvider = IoC.Get<CameraDeviceDataProvider>();
            //_cameraPresetProvider = IoC.Get<CameraPresetDataProvider>();
            _cameraDeviceProvider = cameraDeviceProvider;
            _cameraPresetProvider = cameraPresetProvider;
            _dbService = cameraDbService;
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

                using var cmd = _dbConnection.CreateCommand();

                //Create CameraDevice DB Table
                var dbTable = SetupModel.TableCameraDevice;
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
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
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {dbTable} (
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
                ///////////////////////////////////////////////////////////////////
                //                           CameraDevice
                ///////////////////////////////////////////////////////////////////
                #region Deprecated
                //if (_dbConnection.State != ConnectionState.Open)
                //    await (_dbConnection as DbConnection).OpenAsync();

                //var sql = $@"SELECT * FROM {SetupModel.TableCameraDevice}";


                //foreach (var viewmodel in (_dbConnection
                //    .Query<CameraDeviceModel>(sql)
                //    .Select((item) => new CameraDeviceViewModel(item))))
                //{
                //    _cameraDeviceProvider.Add(viewmodel);
                //};
                #endregion
                await _dbService.FetchDevice(isFinished:true);

                ///////////////////////////////////////////////////////////////////
                //                           CameraPreset
                ///////////////////////////////////////////////////////////////////
                #region Deprecated
                //sql = $@"SELECT * FROM {SetupModel.TableCameraPreset}";

                //foreach (var viewmodel in (_dbConnection
                //            .Query<CameraPresetModel>(sql)
                //            .Select((item) => new CameraPresetViewModel(item))))
                //{
                //    if (_cameraPresetProvider.Where(t => t.Id == viewmodel.Id).Count() > 0)
                //        continue;

                //    _cameraPresetProvider.Add(viewmodel);
                //};
                #endregion
                await _dbService.FetchPreset(isFinished: true);

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
        private CameraDbService _dbService;
        #endregion
    }
}
