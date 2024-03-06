using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Controls;
using Wpf.AxisAudio.Common;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/4/2024 4:00:54 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioMapperModel : AudioBaseModel, IAudioMapperModel
    {

        #region - Ctors -
        public AudioMapperModel()
        {

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
        public int ReferGroupId { get; set; }

        public string DeviceName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
