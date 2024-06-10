using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wpf.AxisAudio.Common.Models;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Client.UI.Models;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Caliburn.Micro;
using System.Data.SQLite;
using Ironwall.Libraries.Base.Services;

namespace Wpf.AxisAudio.Common.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/15/2023 6:10:07 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/
    
    public class AudioDomainDataProvider : TaskService
    {

        #region - Ctors -
        public AudioDomainDataProvider(IDbConnection dbConnection
                                        , ILogService log
                                        , AudioSetupModel setupModel
                                        , AudioDbService audioDbService
                                        , AxisApiService axisApiService
                                        , AudioSymbolProvider audioSymbolProvider)
        {
            _dbConnection = dbConnection;
            _log = log;
            _setupModel = setupModel;
            _dbService = audioDbService;
            _axisApiService = axisApiService;
            _audioSymbolProvider = audioSymbolProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task RunTask(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                await BuildSchemeAsync();
                await FetchAsync();
            }, token);
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task BuildSchemeAsync()
        {
            return Task.Run(async () => 
            {
                try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await(_dbConnection as DbConnection).OpenAsync();

                    using var cmd = _dbConnection.CreateCommand();

                    ////Create TableAudioMultiGroup DB Table
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableAudioMultiGroup} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            audioid INTEGER,
                                            groupid INTEGER
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableAudio DB Table
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableAudio} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            devicename TEXT NOT NULL,
                                            username TEXT,
                                            password TEXT,
                                            ipaddress TEXT,
                                            port TEXT
                                           )";
                    cmd.ExecuteNonQuery();

                    ////Create TableAudioGroup DB Table
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableAudioGroup} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            groupnumber INTEGER,
                                            groupname TEXT
                                           )";
                    cmd.ExecuteNonQuery();

                    

                    ////Create TableAudioSensor DB Table
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableAudioSensor} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            groupnumber TEXT,
                                            devicename TEXT,
                                            controllerid INTEGER,
                                            sensorid INTEGER,
                                            devicetype INTEGER
                                           )";
                    cmd.ExecuteNonQuery();

                    ///Speaker 테이블
                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableAudioSymbol} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            namearea TEXT,
                                            typedevice INTEGER,
                                            namedevice TEXT,
                                            idcontroller INTEGER,    
                                            idsensor INTEGER,
                                            typeshape INTEGER,
                                            x1 REAL,
                                            y1 REAL,
                                            x2 REAL,
                                            y2 REAL,
                                            width REAL,
                                            height REAL,
                                            angle REAL,
                                            map INTEGER,
                                            used INTEGER,
                                            visibility INTEGER,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    _dbConnection.Close();

                    _log.Error($"Raised {nameof(SQLiteException)} in {nameof(BuildSchemeAsync)} of {nameof(AudioDomainDataProvider)} : {ex.Message}", true);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(BuildSchemeAsync)} of {nameof(AudioDomainDataProvider)} : {ex.Message}", true);
                }
            });
            
        }
        private Task FetchAsync()
        {
            return Task.Run(async () => 
            {
                try
                {
                    await _dbService.FetchAudioGroupModel();
                    await _dbService.FetchAudioModel();
                    await _dbService.FetchAudioSensorModel();
                    await _dbService.FetchAudioSymbolModel();
                    await _dbService.CreateAudioGroupModel();
                    _axisApiService.CreateLookupTable();
                    _axisApiService.CreateVisualLookupTable();
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(FetchAsync)} of {nameof(AudioDomainDataProvider)} : {ex.Message}");
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private ILogService _log;
        private AudioSetupModel _setupModel;
        private AudioDbService _dbService;
        private AxisApiService _axisApiService;
        private AudioSymbolProvider _audioSymbolProvider;
        #endregion
    }
}
