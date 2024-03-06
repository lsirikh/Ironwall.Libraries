using System;

namespace Ironwall.Libraries.OpenCvRtsp.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/18/2023 2:08:23 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SizeEventArgs : EventArgs
    {

        #region - Ctors -
        public SizeEventArgs(double width, double height)
        {
            Width = width;
            Height = height;
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
        public double Width { get; set; }
        public double Height { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
