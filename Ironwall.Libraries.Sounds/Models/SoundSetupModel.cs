namespace Ironwall.Libraries.Sounds.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/10/2023 10:40:35 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoundSetupModel
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

        public int DiscardTime
        {
            get => _dicardTime;
            set 
            {
                _dicardTime = value;
                Properties.Settings.Default.DiscardTime = value;
                Properties.Settings.Default.Save();
            }
        }


        public bool IsSound
        {
            get => _isSound;
            set 
            { 
                _isSound = value;
                Properties.Settings.Default.IsSound = value;
                Properties.Settings.Default.Save();
            }
        } 


        #endregion
        #region - Attributes -
        private int _dicardTime = Properties.Settings.Default.DiscardTime;
        private bool _isSound = Properties.Settings.Default.IsSound;
        #endregion
    }
}
