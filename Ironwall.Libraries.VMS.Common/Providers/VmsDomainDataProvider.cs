using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Models;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.VMS.Common.Providers
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 3:07:43 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsDomainDataProvider : TaskService
    {
        #region - Ctors -
        public VmsDomainDataProvider(ILogService log
                                    , IDbConnection dbConnection
                                    , VmsSetupModel setupModel
                                    )
        {
            _log = log;
            _dbConnection = dbConnection;
            _setupModel = setupModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                await Task.Run(() => { BuildSchemeAsync(); });
                //await CreateEntity();
                await FetchAsync();
            });
        }

        protected override Task RunTask(CancellationToken token = default)
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
                if (_dbConnection.State != ConnectionState.Open)
                    await (_dbConnection as DbConnection).OpenAsync();

                var cmd = _dbConnection.CreateCommand();

                //Create TableController Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableVmsApiSetting} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            apiaddress TEXT UNIQUE NOT NULL ,
                                            apiport INTEGER,
                                            username TEXT,
                                            password TEXT
                                           )";
                cmd.ExecuteNonQuery();

               

                //Create TableDeviceInfo Device DB Table
                cmd.CommandText = $@"CREATE TABLE IF NOT EXISTS {_setupModel.TableVmsApiMapping} (
                                            id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
                                            groupnumber INTEGER,
                                            eventid INTEGER
                                           )";
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private Task FetchAsync()
        {
            try
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();


            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception for Task to insert DB data in {nameof(FetchAsync)}: " + ex.Message);
            }
            
            return Task.CompletedTask;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private IDbConnection _dbConnection;
        private VmsSetupModel _setupModel;
        #endregion

    }
}