using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.Models
{
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
        public int PtzTimeOut
        {
            get => _ptzTimeOut;
            set
            {

                _ptzTimeOut = value;
                Properties.Settings.Default.PtzTimeOut = _ptzTimeOut;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
        #region - Attributes -
        private int _ptzTimeOut = Properties.Settings.Default.PtzTimeOut;
        #endregion
    }
}
