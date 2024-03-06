using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.VlcRTSP.Models
{
    public class VlcSetupModel
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
        #endregion
    }
}
