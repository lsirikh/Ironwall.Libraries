using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using System;
using System.Data;

namespace Ironwall.Libraries.VMS.Common.Services
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/11/2024 2:10:59 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsDbService
    {
        #region - Ctors -
        public VmsDbService(ILogService log
                            , IDbConnection dbConnection
                            , VmsApiProvider vmsApiProvider
                            , VmsEventProvider vmsEventProvider
                            , VmsMappingProvider vmsMappingProvider)
        {
            _dbConnection = dbConnection;

        }
        #endregion
        #region - Implementation of Interface -
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
        #endregion
    }
}