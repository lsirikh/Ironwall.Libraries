using Dapper;
using Ironwall.Libraries.Map.Common.Models;
using Ironwall.Libraries.Map.Common.Providers.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using Ironwall.Libraries.Enums;
using System.Windows;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models;
using System.Windows.Media;
using Ironwall.Libraries.Map.Common.Helpers;
using System.Windows.Documents;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Map.Common.Services
{
    public class MapDbService
    {
        
        #region - Ctors -
        public MapDbService(IDbConnection dbConnection
            , MapProvider mapProvider
            , SymbolProvider symbolProvider
            
            , TextSymbolProvider textSymbolProvider
            , RectangleShapeProvider rectangleShapeProvider
            , EllipseShapeProvider ellipseShapeProvider
            
            , ControllerObjectProvider controllerObjectProvider
            , CameraObjectProvider cameraObjectProvider
            , MultiSensorObjectProvider multiSensorObjectProvider
            , FenceObjectProvider fenceObjectProvider

            , PointProvider pointProvider
            , MapSetupModel mapSetupModel
            , SymbolInfoModel symbolInfoModel)
        {
            _dbConnection = dbConnection;
            _setupModel = mapSetupModel;

            _mapProvider = mapProvider;
            
            _symbolProvider = symbolProvider;

            _textSymbolProvider = textSymbolProvider;
            _cameraObjectProvider = cameraObjectProvider;
            _controllerObjectProvider = controllerObjectProvider;
            _multiSensorObjectProvider = multiSensorObjectProvider;
            _fenceObjectProvider = fenceObjectProvider;

            _rectangleShapeProvider = rectangleShapeProvider;
            _ellipseShapeProvider = ellipseShapeProvider;

            _pointProvider = pointProvider;
            _symbolInfoModel = symbolInfoModel;
        }
        #endregion
        #region - Implementation of Interface -
        public async Task FetchSymbolInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Device
                ///////////////////////////////////////////////////////////////////

                var table = _setupModel.TableObjectInfo;

                var sql = $@"SELECT * FROM {table}";
                foreach (var model in (await _dbConnection
                    .QueryAsync<SymbolInfoTableMapper>(sql)).ToList())
                {
                    if (token.IsCancellationRequested)
                        break;

                    var symbolDetailModel = ModelFactory.Build<SymbolDetailModel>(model);
                    _symbolInfoModel.Update(symbolDetailModel);

                }

                
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(FetchSymbolInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchSymbolInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task DeleteSymbolInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableObjectInfo;

                ///////////////////////////////////////////////////////////////////
                ///                   INFORMATION Symbol
                ///////////////////////////////////////////////////////////////////

                var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                int commitResult = 0;
                if (count > 0)
                {
                    commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                    Debug.WriteLine($"DELETE {table} for not being matched symbol data in DB");
                    if (!(commitResult > 0))
                        throw new Exception($"Raised exception during deleting Table({table}).");

                    _symbolInfoModel.Clear();
                }
                else
                {
                    Debug.WriteLine($"{table} was empty Table in DB");
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(DeleteSymbolInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(DeleteSymbolInfo)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public Task UpdateSymbolInfo(ISymbolDetailModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableObjectInfo;

                    var mapper = MapperFactory.Build<SymbolInfoTableMapper>(model);

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {_setupModel.TableObjectInfo}"));

                    if (count == 0)
                    {
                        commitResult = await conn.ExecuteAsync(
                        $@"INSERT INTO {table} 
                        (map, symbol, shapesymbol, objectshape, updatetime) 
                        VALUES
                        (@Map, @Symbol, @ShapeSymbol, @ObjectShape, @UpdateTime)", mapper);
                    }
                    else
                    {
                        commitResult = await conn.ExecuteAsync(
                        $@"UPDATE {table} 
                        SET
                        map = @Map, symbol = @Symbol, shapesymbol = @ShapeSymbol, objectshape = @ObjectShape, updatetime = @UpdateTime", mapper);
                    }

                    _symbolInfoModel.Update(model);

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(UpdateSymbolInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(UpdateSymbolInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task FetchMapObject(CancellationToken token = default, bool isFinshed = false)
        {
            return Task.Run(async () => 
            {
                try
                {
                    string sql = $@"SELECT * FROM { _setupModel.TableMaps}";

                    foreach (var model in (await _dbConnection.QueryAsync<MapModel>(sql)).OrderByDescending(t=>t.MapNumber).ToList())
                    {
                        if (token.IsCancellationRequested)
                            break;

                        _mapProvider.Add(model);
                    }

                    if (isFinshed)
                        await _mapProvider.Finished();
                }
                catch (Exception)
                {

                }            
            }, token);
        }

        public Task SaveMapObject(IMapModel model, CancellationToken token = default, bool isFinshed = false)
        {
            int commitResult = 0;
            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableMaps;


                    commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                    ( id, mapname, mapnumber, filename, filetype, url, width, height, used, visibility) 
                                    VALUES
                                    (@Id, @MapName, @MapNumber, @FileName, @FileType, @Url, @Width, @Height, @Used, @Visibility)", model);

                    Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveSinglePoint)} : " + ex.Message);
                }
            }, token);
        }

        public Task SaveMaps(CancellationToken token = default, bool isFinished = false, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableMaps;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        Debug.WriteLine($"DELETE {table} for saving new Data in DB");

                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _mapProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                    ( id, mapname, mapnumber, filename, filetype, url, width, height, used, visibility) 
                                    VALUES
                                    (@Id, @MapName, @MapNumber, @FileName, @FileType, @Url, @Width, @Height, @Used, @Visibility)", item);

                        updatedRecord++;

                    }
                    Debug.WriteLine($"MapObject was inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(SaveMaps)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMaps)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }
        

        public async Task FetchAllSymbols(CancellationToken token = default)
        {
            try
            {
                await FetchPointElement();
                await FetchPlainSymbol(isFinished: false);
                await FetchShapeSymbol(isFinished: false);
                await FetchObjectSymbol(isFinished: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchAllSymbols)}: " + ex.Message);
            }
        }

        public async Task FetchPointElement(CancellationToken token = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   SHAPE SYMBOL
                ///////////////////////////////////////////////////////////////////
                
                string sql = $@"SELECT * FROM {_setupModel.TablePoints}";
                await Task.Delay(100, token);


                foreach (var model in (await _dbConnection
                    .QueryAsync<PointClass>(sql)).ToList())
                {
                    if (token.IsCancellationRequested)
                        break;

                    _pointProvider.Add(model);
                }
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(FetchPointElement)}: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchPointElement)}: " + ex.Message);
            }
        }

        public async Task FetchPlainSymbol(CancellationToken token = default, bool isFinished = false)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();


                ///////////////////////////////////////////////////////////////////
                ///                   SHAPE SYMBOL
                ///////////////////////////////////////////////////////////////////
                ///
                string sql = $@"SELECT * FROM {_setupModel.TableSymbols}";
                await Task.Delay(100, token);

                foreach (var model in (await _dbConnection
                    .QueryAsync<ShapeSymbolModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested)
                        break;

                    _symbolProvider.Add(model);
                }

                if (isFinished)
                    await _symbolProvider.Finished();
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(FetchPlainSymbol)}: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchPlainSymbol)}: " + ex.Message);
            }
        }

        public async Task FetchShapeSymbol(CancellationToken token = default, bool isFinished = false)
        {
            try
                {
                    if (_dbConnection.State != ConnectionState.Open)
                        await (_dbConnection as DbConnection).OpenAsync();


                    ///////////////////////////////////////////////////////////////////
                    ///                   SHAPE SYMBOL
                    ///////////////////////////////////////////////////////////////////
                    ///
                    string sql = $@"SELECT * FROM {_setupModel.TableGeometries}";
                    await Task.Delay(100, token);

                    foreach (var model in (await _dbConnection
                        .QueryAsync<ShapeSymbolModel>(sql)).ToList())
                    {
                        if (token.IsCancellationRequested)
                            break;

                    if ((EnumShapeType)model.TypeShape == EnumShapeType.POLYLINE)
                    {
                        var points = _pointProvider.Where(p => p.PointGroup == model.Id).ToList().OrderBy(p => p.Sequence);
                        var pointList = new List<Point>();
                        pointList.AddRange(points.Select(p => new Point(p.X, p.Y)));
                        model.Points = new System.Windows.Media.PointCollection(pointList);
                        model.Points.Freeze();
                    }

                    _symbolProvider.Add(model);
                    }

                    if (isFinished)
                        await _symbolProvider.Finished();
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(FetchShapeSymbol)}: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchShapeSymbol)}: " + ex.Message);
                }
        }

        public async Task FetchObjectSymbol(CancellationToken token = default, bool isFinished = false)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   OBJECT SYMBOL
                ///////////////////////////////////////////////////////////////////
                string sql = $@"SELECT * FROM {_setupModel.TableObjects}";
                await Task.Delay(100, token);


                foreach (var model in (await _dbConnection
                    .QueryAsync<ObjectShapeModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested)
                        break;

                    if((EnumShapeType)model.TypeShape == EnumShapeType.FENCE)
                    {
                        var points = _pointProvider.Where(p => p.PointGroup == model.Id).ToList().OrderBy(p=> p.Sequence);
                        var pointList = new List<Point>();
                        pointList.AddRange(points.Select(p => new Point(p.X, p.Y)));
                        model.Points = new System.Windows.Media.PointCollection(pointList);
                        model.Points.Freeze();
                    }

                    _symbolProvider.Add(model);
                }

                if (isFinished)
                    await _symbolProvider.Finished();
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(FetchObjectSymbol)}: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(FetchObjectSymbol)}: " + ex.Message);
            }
        }

        public async Task SaveAllSymbols(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                //await _dbConnection.ExecuteAsync($@"DELETE FROM {_setupModel.TableMaps}");
                await _dbConnection.ExecuteAsync($@"DELETE FROM {_setupModel.TablePoints}");
                await _dbConnection.ExecuteAsync($@"DELETE FROM {_setupModel.TableSymbols}");
                await _dbConnection.ExecuteAsync($@"DELETE FROM {_setupModel.TableGeometries}");
                await _dbConnection.ExecuteAsync($@"DELETE FROM {_setupModel.TableObjects}");

                foreach (var item in _symbolProvider)
                {
                    if (token.IsCancellationRequested) throw new TaskCanceledException();

                    switch ((EnumShapeType)item.TypeShape)
                    {
                        case EnumShapeType.NONE:
                            break;
                        case EnumShapeType.TEXT:
                            {
                                await SaveSymbol((item as ISymbolModel), token);
                            }
                            break;
                        case EnumShapeType.LINE:
                        case EnumShapeType.TRIANGLE:
                        case EnumShapeType.RECTANGLE:
                        case EnumShapeType.POLYGON:
                        case EnumShapeType.ELLIPSE:
                        case EnumShapeType.POLYLINE:
                            {
                                await SaveShapeSymbol((item as IShapeSymbolModel), token);
                            }
                            break;
                        case EnumShapeType.FENCE:
                            {
                                var obj = item as IObjectShapeModel;
                                await SaveMultiPoints(obj.Id, obj?.Points, token);
                                //var sequence = 0;
                                //foreach (Point point in obj?.Points)
                                //{
                                //    await SaveSinglePoint(new PointClass()
                                //    {
                                //        PointGroup = obj.Id,
                                //        Sequence = sequence++,
                                //        X = point.X,
                                //        Y = point.Y,
                                //    }, token);
                                //}
                                await SaveObjectShape((item as IObjectShapeModel), token);
                            }
                            break;
                        case EnumShapeType.CONTROLLER:
                        case EnumShapeType.MULTI_SNESOR:
                        case EnumShapeType.FENCE_SENSOR:
                        case EnumShapeType.UNDERGROUND_SENSOR:
                        case EnumShapeType.CONTACT_SWITCH:
                        case EnumShapeType.PIR_SENSOR:
                        case EnumShapeType.IO_CONTROLLER:
                        case EnumShapeType.LASER_SENSOR:
                        case EnumShapeType.CABLE:
                        case EnumShapeType.IP_CAMERA:
                        case EnumShapeType.FIXED_CAMERA:
                        case EnumShapeType.PTZ_CAMERA:
                        case EnumShapeType.SPEEDDOM_CAMERA:
                            {
                                await SaveObjectShape((item as IObjectShapeModel), token);
                            }
                            break;
                        default:
                            break;
                    }
                }

                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Task was cancelled in {nameof(SaveAllSymbols)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveAllSymbols)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task SaveSinglePoint(IPointClass model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TablePoints;


                var prevPoint = _pointProvider.Where(point => point.Id == model.Id).FirstOrDefault();
                if(prevPoint != null)   
                    _pointProvider.Remove(prevPoint);

                commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        ( pointgroup, sequence, x, y) 
                                        VALUES
                                        (@PointGroup, @Sequence, @X, @Y)", model);

                _pointProvider.Add(model);
                Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");

                tcs?.SetResult(true);
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveSinglePoint)} : " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public Task SaveMultiPoints(int pGroup, PointCollection points, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;
            
            return Task.Run(async () => 
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TablePoints;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table} WHERE pointgroup = {pGroup}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} WHERE pointgroup = {pGroup}");
                        Debug.WriteLine($"DELETE {table} WHERE pointgroup = {pGroup} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    _pointProvider.RemoveAll(entity => entity.PointGroup == pGroup);

                    int updatedRecord = 0;
                    DispatcherService.Invoke((System.Action)(async () =>
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            foreach (var item in points)
                            {
                                if (token.IsCancellationRequested) throw new TaskCanceledException();

                                var pointClass = new PointClass()
                                {
                                    PointGroup = pGroup,
                                    Sequence = updatedRecord++,
                                    X = item.X,
                                    Y = item.Y,
                                };

                                commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                                ( pointgroup, sequence, x, y) 
                                                VALUES
                                                (@PointGroup, @Sequence, @X, @Y)", pointClass);

                                _pointProvider.Add(pointClass);
                            }
                            transaction.Commit();
                        }
                    }));
                    Debug.WriteLine($"PointSymbols were inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(SaveMultiPoints)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMultiPoints)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task SaveSymbol(ISymbolModel model, CancellationToken token = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableSymbols;

                commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                                         isvisible, layer, map, isused) 
                                        VALUES
                                        (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                                         @IsVisible, @Layer, @Map, @IsUsed)", model);

                Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveSymbol)} : " + ex.Message);
            }
        }

        public Task SaveMultiSymbols(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableSymbols;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        Debug.WriteLine($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _symbolProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();
                        if (!SymbolHelper.IsSymbolCategory(item.TypeShape)) continue;

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                        (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                            isvisible, layer, map, isused) 
                        VALUES
                        (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                            @IsVisible, @Layer, @Map, @IsUsed)", item);

                        updatedRecord++;
                    }

                    Debug.WriteLine($"Symbols were inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(SaveMultiSymbols)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMultiSymbols)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task SaveShapeSymbol(IShapeSymbolModel model, CancellationToken token = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableGeometries;

                commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                                         shapefill, shapestroke, shapestrokethick,
                                         isvisible, layer, map, isused) 
                                        VALUES
                                        (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                                         @ShapeFill, @ShapeStroke, @ShapeStrokeThick,
                                         @IsVisible, @Layer, @Map, @IsUsed)", model);

                Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveShapeSymbol)} : " + ex.Message);
            }
        }

        
        
        public Task SaveMultiShapes(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableGeometries;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        Debug.WriteLine($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    int updatedRecord = 0;
                    foreach (var item in _symbolProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();
                        if (!SymbolHelper.IsShapeCategory(item.TypeShape)) continue;

                        var shapeItem = item as IShapeSymbolModel;

                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                    (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                                        shapefill, shapestroke, shapestrokethick, isvisible, layer, map, isused) 
                                    VALUES
                                    (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                                        @ShapeFill, @ShapeStroke, @ShapeStrokeThick, @IsVisible, @Layer, @Map, @IsUsed)", shapeItem);

                        updatedRecord++;

                        if (shapeItem.TypeShape == (int)EnumShapeType.POLYLINE)
                        {
                            var pList = new List<PointClass>();
                            await SaveMultiPoints(shapeItem.Id, shapeItem.Points, token);
                        }
                    }

                    Debug.WriteLine($"Symbols were inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(SaveMultiShapes)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMultiShapes)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public async Task SaveObjectShape(IObjectShapeModel model, CancellationToken token = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableObjects;

                commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                                         shapefill, shapestroke, shapestrokethick,
                                         idcontroller, idsensor, namearea, namedevice, typedevice,
                                         isvisible, layer, map, isused) 
                                        VALUES
                                        (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                                         @ShapeFill, @ShapeStroke, @ShapeStrokeThick,
                                         @IdController, @IdSensor, @NameArea, @NameDevice, @TypeDevice,
                                         @IsVisible, @Layer, @Map, @IsUsed)", model);

                Debug.WriteLine($"({commitResult}) rows was updated in DB[{table}] : {model}");
            }
            catch (Exception ex)
            {
                var result = ex.Message;
                Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveObjectShape)} : " + ex.Message);
            }
        }

        public Task SaveMultiObjects(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            int commitResult = 0;

            return Task.Run(async () =>
            {
                try
                {
                    var conn = _dbConnection as SQLiteConnection;
                    var table = _setupModel.TableObjects;

                    var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) FROM {table}"));

                    if (count > 0)
                    {
                        commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                        Debug.WriteLine($"DELETE {table} for saving new Data in DB");
                        if (!(commitResult > 0))
                            throw new Exception($"Raised exception during deleting Table({table}).");
                    }

                    //FENCE = 20,
                    //CONTROLLER = 21,
                    //MULTI_SNESOR = 22,
                    //FENCE_SENSOR = 23,
                    //UNDERGROUND_SENSOR = 24,
                    //CONTACT_SWITCH = 25,
                    //PIR_SENSOR = 26,
                    //IO_CONTROLLER = 27,
                    //LASER_SENSOR = 28,
                    //CABLE = 29,

                    //IP_CAMERA = 30,
                    //FIXED_CAMERA = 31,
                    //PTZ_CAMERA = 32,
                    //SPEEDDOM_CAMERA = 33

                    int updatedRecord = 0;
                    foreach (var item in _symbolProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();
                        
                        if (!SymbolHelper.IsObjectCategory(item.TypeShape)) continue;
                        
                        var objectItem = item as IObjectShapeModel;
                        commitResult = await conn.ExecuteAsync($@"INSERT INTO {table} 
                                        (id, x, y, z, width, height, angle, isshowlable, lable, fontsize, fontcolor, typeshape, 
                                         shapefill, shapestroke, shapestrokethick, isvisible, layer, map, isused) 
                                        VALUES
                                        (@Id, @X, @Y, @Z, @Width, @Height, @Angle, @IsShowLable, @Lable, @FontSize, @FontColor, @TypeShape,
                                         @ShapeFill, @ShapeStroke, @ShapeStrokeThick, @IsVisible, @Layer, @Map, @IsUsed)", objectItem);

                        updatedRecord++;

                        if(objectItem.TypeShape == (int)EnumShapeType.FENCE)
                        {
                            var pList = new List<PointClass>();

                            await SaveMultiPoints(objectItem.Id, objectItem.Points, token);
                        }
                    }
                    Debug.WriteLine($"Symbols were inserted in DB[{table}] : {updatedRecord}ea");

                    tcs?.SetResult(true);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(SaveMultiObjects)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(SaveMultiObjects)}: " + ex.Message);
                    tcs?.SetException(ex);
                }
            });
        }

        public Task<bool> MatchInfo(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var map = _mapProvider.Count();
                    var symbol = _symbolProvider
                    .Where(entity => SymbolHelper.IsSymbolCategory(entity.TypeShape) == true).Count();
                    var shapeSymbol = _symbolProvider
                    .Where(entity => SymbolHelper.IsShapeCategory(entity.TypeShape) == true).Count();
                    var objectShape = _symbolProvider
                    .Where(entity => SymbolHelper.IsObjectCategory(entity.TypeShape) == true).Count();

                    if (map != _symbolInfoModel.Map
                        ||symbol != _symbolInfoModel.Symbol
                        || shapeSymbol != _symbolInfoModel.ShapeSymbol
                        || objectShape != _symbolInfoModel.ObjectShape)
                    {
                        tcs?.SetResult(false);
                        return false;
                    }

                    tcs?.SetResult(true);
                    return true;
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Task was cancelled in {nameof(MatchInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception for Task to insert DB data in {nameof(MatchInfo)}: " + ex.Message);
                    tcs?.SetException(ex);
                    return false;
                }
            });
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private MapSetupModel _setupModel;
        private MapProvider _mapProvider;
        private SymbolProvider _symbolProvider;
        private TextSymbolProvider _textSymbolProvider;
        private CameraObjectProvider _cameraObjectProvider;
        private ControllerObjectProvider _controllerObjectProvider;
        private MultiSensorObjectProvider _multiSensorObjectProvider;
        private FenceObjectProvider _fenceObjectProvider;
        private RectangleShapeProvider _rectangleShapeProvider;
        private EllipseShapeProvider _ellipseShapeProvider;
        private PointProvider _pointProvider;
        private SymbolInfoModel _symbolInfoModel;
        #endregion
    }
}
