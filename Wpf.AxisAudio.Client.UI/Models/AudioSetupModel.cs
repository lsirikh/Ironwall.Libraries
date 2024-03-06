namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/19/2023 2:57:15 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSetupModel
    {

        #region - Ctors -
        public AudioSetupModel()
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
        public string AudioFile => Properties.Settings.Default.AudioFile;
        public string TableAudio => Properties.Settings.Default.TableAudio;
        public string TableAudioGroup => Properties.Settings.Default.TableAudioGroup;
        public string TableAudioSensor => Properties.Settings.Default.TableAudioSensor;
        public string TableAudioSymbol => Properties.Settings.Default.TableAudioSymbol;
        public string TableAudioMultiGroup => Properties.Settings.Default.TableMultiGroup;
        #endregion
        #region - Attributes -
        #endregion
    }
}
