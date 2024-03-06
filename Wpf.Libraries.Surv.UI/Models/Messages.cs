using System.Collections.Generic;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.UI.ViewModels;

namespace Wpf.Libraries.Surv.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/6/2023 10:27:17 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OpenSurvCameraDialogMessageModel
    {
        public OpenSurvCameraDialogMessageModel(SurvCameraViewModel survCameraViewModel)
        {
            ViewModel = survCameraViewModel;
        }

        public SurvCameraViewModel ViewModel { get; }
    }

    public class RefreshSurvIpAddressMessageModel
    {
        public RefreshSurvIpAddressMessageModel(ISurvCameraModel model)
        {
            Model = model;
        }
        public ISurvCameraModel Model { get; }
    }

    public class OpenSurvSensorMatchingDialogMessageModel
    {

    }

    public class RefreshSurvSensorSetupMessageModel
    {

    }
}
