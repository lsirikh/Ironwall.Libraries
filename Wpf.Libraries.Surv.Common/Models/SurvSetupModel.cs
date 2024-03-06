namespace Wpf.Libraries.Surv.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/1/2023 2:43:59 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvSetupModel
    {

        public string TableSurvApi => Properties.Settings.Default.TableSurvApi;
        public string TableSurvEvent => Properties.Settings.Default.TableSurvEvent;
        public string TableSurvMapping => Properties.Settings.Default.TableSurvMapping;
        public string TableSurvCamera => Properties.Settings.Default.TableSurvCamera;
        public string TableSurvSensor => Properties.Settings.Default.TableSurvSensor;
    }
}
