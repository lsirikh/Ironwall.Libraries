using Ironwall.Framework.Helpers;
using System;

namespace Ironwall.Libraries.OpenCvRtsp.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/25/2023 9:43:47 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class LogEventArgs : EventArgs
    {

        #region - Ctors -
        public LogEventArgs(string log, DateTime dateTime = default)
        {
            Log = log;
            DateTime = dateTime != null ? DateTimeHelper.GetInputTimeWithoutMS(dateTime) : DateTimeHelper.GetCurrentTimeWithoutMS();
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
        public string Log { get; }
        public DateTime DateTime { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
