using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:02:18 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvApiViewModel : SurvBaseViewModel<ISurvApiModel>
    {

        #region - Ctors -
        public SurvApiViewModel(ISurvApiModel model) : base(model) 
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
        public string ApiAddress
        {
            get { return _model.ApiAddress; }
            set 
            {
                _model.ApiAddress = value;
                NotifyOfPropertyChange(() => ApiAddress);
            }
        }

        public uint ApiPort
        {
            get { return _model.ApiPort; }
            set
            {
                _model.ApiPort = value;
                NotifyOfPropertyChange(() => ApiPort);
            }
        }

        public string UserName
        {
            get { return _model.UserName; }
            set
            {
                _model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string Password
        {
            get { return _model.Password; }
            set
            {
                _model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
