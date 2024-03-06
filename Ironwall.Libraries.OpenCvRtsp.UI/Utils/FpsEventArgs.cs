using System;

namespace Ironwall.Libraries.OpenCvRtsp.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/25/2023 5:32:39 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class FpsEventArgs : EventArgs
    {

        #region - Ctors -
        public FpsEventArgs(double fps)
        {
            Fps = fps;
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
        public double Fps { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
