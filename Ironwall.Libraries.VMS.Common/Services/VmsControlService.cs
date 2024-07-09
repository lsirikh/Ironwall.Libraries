using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Models.Providers;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Sensorway.Accounts.Base.Models;
using Sensorway.Events.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ironwall.Libraries.VMS.Common.Services
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 12:52:04 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsControlService
    {
        
        #region - Ctors -
        public VmsControlService(ILogService log
                                , IVmsApiService vmsApiService
                                , IVmsDbService vmsDbService
                                , VmsApiProvider vmsApiProvider
                                , VmsEventProvider vmsEventProvider
                                , VmsMappingProvider vmsMappingProvider
                                , VmsSensorProvider vmsSensorProvider
                                , LoginSessionModel loginSession
                                )
        {
            _log = log;
            _vmsApiService = vmsApiService;
            _dbService = vmsDbService;
            _apiProvider = vmsApiProvider;
            _eventProvider = vmsEventProvider;
            _mappingProvider = vmsMappingProvider;
            _SensorProvider = vmsSensorProvider;
            _loginSession = loginSession;

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task<List<IEventModel>> GetEventList()
        {
            var list = _eventProvider.ToList();

            if (list == null || !(list.Count() > 0))
            {
                await _vmsApiService.ApiGetEventListProcess();
                list = _eventProvider.ToList();
            }

            return list;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private IVmsApiService _vmsApiService;
        private IVmsDbService _dbService;
        private VmsApiProvider _apiProvider;
        private VmsEventProvider _eventProvider;
        private VmsMappingProvider _mappingProvider;
        private VmsSensorProvider _SensorProvider;
        private LoginSessionModel _loginSession;
        #endregion
    }
}