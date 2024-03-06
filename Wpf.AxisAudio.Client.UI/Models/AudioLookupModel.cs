using System.Collections.Generic;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/7/2023 2:20:28 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioLookupModel
    {

        #region - Ctors -
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
        public AudioGroupModel GroupModel { get; set; }
        public IEnumerable<AudioModel> AudioModels { get; set; }
        public IEnumerable<AudioSensorModel> SensorModels { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
