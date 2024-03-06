namespace Ironwall.Libraries.CameraOnvif.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/30/2023 1:39:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifSetupModel
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
        public int PtzTimeout
        {
            get => _ptzTimeout;
            set
            {
                _ptzTimeout = value;
                Properties.Settings.Default.PtzTimeout = _ptzTimeout;
                Properties.Settings.Default.Save();
            }
        }

        public bool IsPopupTimer
        {
            get => _isPopupTimer;
            set
            {
                _isPopupTimer = value;
                Properties.Settings.Default.IsPopupTimer = _isPopupTimer;
                Properties.Settings.Default.Save();
            }
        }
        public bool IsCameraPopup
        {
            get => _isCameraPopup;
            set
            {
                _isCameraPopup = value;
                Properties.Settings.Default.IsCameraPopup = _isCameraPopup;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        #region - Attributes -
        private bool _isPopupTimer = Properties.Settings.Default.IsPopupTimer;
        private bool _isCameraPopup = Properties.Settings.Default.IsCameraPopup;
        private int _ptzTimeout = Properties.Settings.Default.PtzTimeout;
        #endregion
    }
}
