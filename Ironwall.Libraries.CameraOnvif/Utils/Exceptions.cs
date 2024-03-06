using System;

namespace Ironwall.Libraries.CameraOnvif.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 9:54:52 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class NullValueException : Exception
    {
        public NullValueException(string message) : base(message)
        {

        }
    }
}
