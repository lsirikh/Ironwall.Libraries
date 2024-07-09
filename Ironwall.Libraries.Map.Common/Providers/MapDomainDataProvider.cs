using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Map.Common.Models;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Libraries.Map.Common.Services;
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

namespace Ironwall.Libraries.Map.Common.Providers
{
    public class MapDomainDataProvider
        : TaskService, IMapDomainDataProvider
    {

        #region - Ctors -
        public MapDomainDataProvider(MapSetupModel setupModel
                                    , MapProvider mapProvider
                                    , IDbConnection dbConnection
                                    , MapDbService mapDbService)
        {
            _dbConnection = dbConnection;
            _setupModel = setupModel;
            _mapProvider = mapProvider;
            _mapDbService = mapDbService;
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
        private async void BuildSchemeAsync()
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                using (var cmd = _dbConnection.CreateCommand())
                {
                    cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableMaps} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            mapname TEXT NOT NULL,
                                            mapnumber INTEGER,
                                            filename TEXT,
                                            filetype TEXT,
                                            url TEXT,
                                            width REAL,
                                            height REAL,
                                            used INTEGER,
                                            visibility INTEGER,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TablePoints} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            pointgroup INTEGER,
                                            sequence INTEGER,
                                            x REAL,
                                            y REAL,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableSymbols} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            x REAL,
                                            y REAL,
                                            z REAL,

                                            width REAL,
                                            height REAL,
                                            angle REAL,
                                            isshowlable INTEGER,
                                            lable TEXT,
                                            fontsize REAL,
                                            fontcolor TEXT,
                                            typeshape INTEGER,

                                            isvisible INTEGER,
                                            layer INTEGER,
                                            map INTEGER,
                                            isused INTEGER,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableGeometries} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            x REAL,
                                            y REAL,
                                            z REAL,
                                            width REAL,
                                            height REAL,
                                            angle REAL,
                                            isshowlable INTEGER,
                                            lable TEXT,
                                            fontsize REAL,
                                            fontcolor TEXT,
                                            typeshape INTEGER,
                                            
                                            shapefill TEXT,
                                            shapestroke TEXT,
                                            shapestrokethick REAL,

                                            isvisible INTEGER,
                                            layer INTEGER,
                                            map INTEGER,
                                            isused INTEGER,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @$"CREATE TABLE IF NOT EXISTS {_setupModel.TableObjects} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            x REAL,
                                            y REAL,
                                            z REAL,
                                            width REAL,
                                            height REAL,
                                            angle REAL,
                                            isshowlable INTEGER,
                                            lable TEXT,
                                            fontsize REAL,
                                            fontcolor TEXT,
                                            typeshape INTEGER,
                                            
                                            shapefill TEXT,
                                            shapestroke TEXT,
                                            shapestrokethick REAL,

                                            idcontroller INTEGER,    
                                            idsensor INTEGER,
                                            namearea TEXT,
                                            namedevice TEXT,
                                            typedevice INTEGER,

                                            isvisible INTEGER,
                                            layer INTEGER,
                                            map INTEGER,
                                            isused INTEGER,
                                            time_created DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();


                    cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableObjectInfo} (
                                            map INTEGER,
                                            symbol INTEGER,
                                            shapesymbol INTEGER,
                                            objectshape INTEGER,
                                            updatetime DATETIME NOT NULL DEFAULT (DATETIME('NOW', 'LOCALTIME'))
                                           )";
                    cmd.ExecuteNonQuery();
                    

                }

            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"Raised SQLiteException in {nameof(BuildSchemeAsync)} :{ex.Message}");
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
                await _mapDbService.FetchSymbolInfo();
                await _mapDbService.FetchMapObject();
                await _mapDbService.FetchAllSymbols();

                var matchResult = await _mapDbService.MatchInfo();
                if (!matchResult)
                {
                    await _mapDbService.DeleteSymbolInfo();
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
        private IDbConnection _dbConnection;
        private MapSetupModel _setupModel;
        private MapProvider _mapProvider;
        private MapDbService _mapDbService;
        #endregion
    }
}
