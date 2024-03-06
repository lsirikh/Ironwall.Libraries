namespace Ironwall.Libraries.WatchDog.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 2:25:39 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WatchdogSetupModel
    {

        public bool IsAutoWatchdog
        {
            get => _isAutoWatchdog;
            set
            {
                _isAutoWatchdog = value;
                Properties.Settings.Default.IsAutoWatchdog = _isAutoWatchdog;
                Properties.Settings.Default.Save();
            }
        }

        public string WatchdogProcess => Properties.Settings.Default.WatchdogProcess;

        private bool _isAutoWatchdog = Properties.Settings.Default.IsAutoWatchdog;
    }
}
