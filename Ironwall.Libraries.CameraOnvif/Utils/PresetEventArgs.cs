using Mictlanix.DotNet.Onvif.Common;
using System.Collections.ObjectModel;
using System;

namespace Ironwall.Libraries.CameraOnvif.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 9:55:36 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PresetEventArgs : EventArgs
    {

        #region - Ctors -
        public PresetEventArgs()
        {

        }

        public PresetEventArgs(ObservableCollection<PTZPreset> preset)
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
        public ObservableCollection<PTZPreset> PtzPresets { get; private set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
