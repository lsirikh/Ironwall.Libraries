using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 11:02:51 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvMappingViewModel : SurvBaseViewModel<ISurvMappingModel>
    {

        #region - Ctors -
        public SurvMappingViewModel(ISurvMappingModel model, ISurvEventModel eventModel) : base(model)
        {
            EventModel = eventModel;
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
        public int GroupNumber
        {
            get { return _model.GroupNumber; }
            set
            {
                _model.GroupNumber = value;
                NotifyOfPropertyChange(() => GroupNumber);
            }
        }

        public string GroupName
        {
            get { return _model.GroupName; }
            set 
            { 
                _model.GroupName = value;
                NotifyOfPropertyChange(() =>  GroupName);
            }
        }

        public ISurvEventModel EventModel
        {
            get { return _eventModel; }
            set 
            { 
                _eventModel = value; 
                NotifyOfPropertyChange(() => EventModel);
                if(value != null)
                    _model.EventId = value.Id;
            }
        }

        #endregion
        #region - Attributes -
        private ISurvEventModel _eventModel;
        #endregion
    }
}
