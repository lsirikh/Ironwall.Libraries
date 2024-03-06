using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;

namespace Wpf.Libraries.Surv.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:01:27 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvCameraSetupViewModel : BaseViewModel
    {

        #region - Ctors -
        public SurvCameraSetupViewModel(IEventAggregator eventAggregator)
                                        : base(eventAggregator)
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
        public SurvCameraViewModel SurvCameraViewModel { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
