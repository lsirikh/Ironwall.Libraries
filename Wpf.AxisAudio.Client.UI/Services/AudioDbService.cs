using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.Models;
using Wpf.AxisAudio.Common.Models;
using Dapper;
using System.Data.SQLite;
using System.Collections.Generic;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Caliburn.Micro;
using Wpf.AxisAudio.Client.UI.ViewModels;
using Ironwall.Libraries.Base.Services;
using Wpf.AxisAudio.Common;

namespace Wpf.AxisAudio.Client.UI.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/15/2023 4:49:12 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioDbService
    {

        #region - Ctors -
        public AudioDbService(IDbConnection dbConnection
                            , ILogService log
                            , IEventAggregator eventAggregator
                            , AudioSetupModel setupModel
                            , AudioProvider audioProvider
                            , AudioGroupProvider audioGroupProvider
                            , AudioSensorProvider audioSensorProvider
                            , AudioSymbolProvider audioSymbolProvider
                            , AudioMultiGroupProvider audioMultiGroupProvider
                            , AudioSymbolViewModelProvider audioSymbolViewModelProvider)
        {
            _log = log;
            _dbConnection = dbConnection;
            _audioProvider = audioProvider;
            _audioGroupProvider = audioGroupProvider;
            _audioMultiGroupProvider = audioMultiGroupProvider;
            _audioSensorProvider = audioSensorProvider;
            _audioSymbolProvider = audioSymbolProvider;
            _audioSymbolViewModelProvider = audioSymbolViewModelProvider;
            _eventAggregator = eventAggregator;
            _setupModel = setupModel;
        }
        #endregion
        #region - Implementation of Interface -
        public async Task FetchMultiGroupModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Fetch TableAudioMultiGroup
                ///////////////////////////////////////////////////////////////////
                var sql = $@"SELECT * FROM {_setupModel.TableAudioMultiGroup}";

                _audioMultiGroupProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<AudioMultiGroupModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _audioMultiGroupProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchMultiGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchMultiGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertAudioMultiGroupModel(IAudioModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableAudioMultiGroup;

                

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var item in model.Groups)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();
                        var mapper = new AudioMultiGroupModel() 
                        {
                            AudioId = model.Id,
                            GroupId = item.Id,
                        };
                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (audioid, groupid) 
                            VALUES
                            (@AudioId, @GroupId)", mapper);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])");
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertAudioMultiGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertAudioMultiGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchAudioModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Fetch TableAudio
                ///////////////////////////////////////////////////////////////////
                var taskCompletionSource = new TaskCompletionSource<bool>();
                await FetchMultiGroupModel(token, taskCompletionSource);

                if (!taskCompletionSource.Task.Result) throw new Exception($"{nameof(FetchMultiGroupModel)} was fail to execute!");

                var sql = $@"SELECT * FROM {_setupModel.TableAudio}";


                _audioProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<AudioModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;
                    var groups = _audioGroupProvider
                        .Where(entity => _audioMultiGroupProvider
                        .Where(innerEntity => innerEntity.AudioId == model.Id)
                        .Any(innerEntity => innerEntity.GroupId == entity.Id))
                        .OfType<AudioGroupBaseModel>().ToList();

                    model.Groups = groups;
                    _audioProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchAudioModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchAudioModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertAudioModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableAudio;

                await DeleteRecordOrTable(table);
                await DeleteRecordOrTable(_setupModel.TableAudioMultiGroup);
                using (var transaction = conn.BeginTransaction())
                {
                    await DeleteRecordOrTable(table);

                    foreach (var model in _audioProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();
                        
                        await InsertAudioMultiGroupModel(model);
                        
                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, devicename, username, password, ipaddress, port) 
                            VALUES
                            (@Id, @DeviceName, @UserName, @Password, @IpAddress, @Port)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])");
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertAudioModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertAudioModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public Task CreateAudioGroupModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {

                foreach (var item in _audioGroupProvider)
                {
                    var audioModels = _audioProvider.Where(entity => entity.Groups.Any(innerEntity => innerEntity.GroupNumber == item.GroupNumber));

                    if (audioModels != null)
                    {
                        foreach (var audioModel in audioModels)
                        {
                            item.AudioModels.Add(audioModel);
                        }
                    }

                    var sensorModels = _audioSensorProvider.Where(entity => entity.Group?.GroupNumber == item.GroupNumber);
                    if (sensorModels != null)
                    {
                        foreach (var sensorModel in sensorModels)
                        {
                            item.SensorModels.Add(sensorModel);
                        }
                    }
                }
                tcs?.SetResult(true);
                
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(CreateAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(CreateAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }

            return Task.CompletedTask;
        }

        public async Task FetchAudioGroupModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                  Fetch TableAudioGroup
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableAudioGroup}";

                _audioGroupProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<AudioGroupModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;
                    _audioGroupProvider.Add(model);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertAudioGroupModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableAudioGroup;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _audioGroupProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, groupnumber, groupname) 
                            VALUES
                            (@Id, @GroupNumber, @GroupName)", model);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}]");
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertAudioGroupModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchAudioSensorModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                  Fetch TableAudioSensor
                ///////////////////////////////////////////////////////////////////

                var sql = $@"SELECT * FROM {_setupModel.TableAudioSensor}";

                _audioSensorProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<AudioSensorMapperModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;
                    var groupModel = _audioGroupProvider.Where(entity => entity.GroupNumber ==
                    model.GroupNumber).FirstOrDefault();
                    if (groupModel == null) continue;

                    var sensorModel = new AudioSensorModel()
                    {
                        Id = model.Id,
                        Group = groupModel,
                        DeviceName = model.DeviceName,
                        ControllerId = model.ControllerId,
                        SensorId = model.SensorId,
                        DeviceType = model.DeviceType,
                    };
                    _audioSensorProvider.Add(sensorModel);
                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchAudioSensorModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchAudioSensorModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task InsertAudioSensorModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                int commitResult = 0;
                var conn = _dbConnection as SQLiteConnection;
                var table = _setupModel.TableAudioSensor;

                await DeleteRecordOrTable(table);

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var model in _audioSensorProvider)
                    {
                        if (token.IsCancellationRequested) throw new TaskCanceledException();

                        var mapper = new AudioSensorMapperModel(model);

                        commitResult += await conn.ExecuteAsync(
                            $@"INSERT INTO {table} 
                            (id, groupnumber, devicename, controllerid, sensorid, devicetype) 
                            VALUES
                            (@Id, @GroupNumber, @DeviceName, @ControllerId, @SensorId, @DeviceType)", mapper);
                    }
                    transaction.Commit();
                }

                _log.Info($"({commitResult}) rows was inserted in DB[{table}])");
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(InsertAudioSensorModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(InsertAudioSensorModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }

        public async Task FetchAudioSymbolModel(CancellationToken token = default, TaskCompletionSource<bool> tcs = default)
        {
            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                ///////////////////////////////////////////////////////////////////
                ///                   Fetch TableAudioSymbol
                ///////////////////////////////////////////////////////////////////

                var sql = @$"SELECT * FROM {_setupModel.TableAudioSymbol} WHERE used = '1'";

                _audioSymbolProvider.Clear();
                foreach (var model in (await _dbConnection
                    .QueryAsync<AudioSymbolModel>(sql)).ToList())
                {
                    if (token.IsCancellationRequested) break;

                    _audioSymbolProvider.Add(model);
                    var viewModel = new AudioSymbolViewModel(model, _eventAggregator);
                    await viewModel.ActivateAsync();
                    _audioSymbolViewModelProvider.Add(viewModel);

                }
                tcs?.SetResult(true);
            }
            catch (TaskCanceledException ex)
            {
                _log.Error($"Task was cancelled in {nameof(FetchAudioSymbolModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchAudioSymbolModel)}: " + ex.Message);
                tcs?.SetException(ex);
            }
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task DeleteRecordOrTable(string table, int? id = default, string refer = "id")
        {
            try
            {
                if (id != null)
                {
                    DeleteRecordFromId(table, id, refer);
                }
                else
                {
                    DeleteTable(table);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(DeleteRecordOrTable)}: " + ex.Message);
            }

            return Task.CompletedTask;

        }

        private async void DeleteTable(string table)
        {
            var commitResult = 0;
            var conn = _dbConnection as SQLiteConnection;

            var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                                    FROM {table}"));

            if (count > 0)
            {
                commitResult = await conn.ExecuteAsync($@"DELETE FROM {table}");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Info($"No data from {table}");
            }
        }

        private async void DeleteRecordFromId(string table, int? id, string refer)
        {
            var commitResult = 0;
            var conn = _dbConnection as SQLiteConnection;

            var count = Convert.ToInt32(await conn.ExecuteScalarAsync($@"SELECT COUNT(*) 
                                                                        FROM {table}
                                                                        WHERE {refer} = '{id}'"));

            if (count > 0)
            {
                commitResult = await conn.ExecuteAsync($@"DELETE FROM {table} 
                                                          WHERE {refer} = '{id}'");

                if (!(commitResult > 0)) throw new Exception($"Raised exception during deleting Table({table}).");
            }
            else
            {
                _log.Info($"Not Exist for Matched ID({id})");
            }
        }

        private ILogService _log;
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IDbConnection _dbConnection;
        private AudioProvider _audioProvider;
        private AudioGroupProvider _audioGroupProvider;
        private AudioMultiGroupProvider _audioMultiGroupProvider;
        private AudioSensorProvider _audioSensorProvider;
        private AudioSymbolProvider _audioSymbolProvider;
        private AudioSymbolViewModelProvider _audioSymbolViewModelProvider;
        private IEventAggregator _eventAggregator;
        private AudioSetupModel _setupModel;
        #endregion
    }
}
