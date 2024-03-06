using Wpf.Libraries.Surv.Common.Models;

namespace Wpf.Libraries.Surv.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 9:18:56 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvSensorViewModel : SurvBaseViewModel<ISurvSensorModel>
    {

        #region - Ctors -
        public SurvSensorViewModel(ISurvSensorModel model) : base(model)
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
        public string GroupName
        {
            get { return _model.GroupName; }
            set
            {
                _model.GroupName = value;
                NotifyOfPropertyChange(() => GroupName);
            }
        }

        public string DeviceName
        {
            get { return _model.DeviceName; }
            set
            {
                _model.DeviceName = value;
                NotifyOfPropertyChange(() => DeviceName);
            }
        }

        public int ControllerId
        {
            get { return _model.ControllerId; }
            set
            {
                _model.ControllerId = value;
                NotifyOfPropertyChange(() => ControllerId);
            }
        }

        public int SensorId
        {
            get { return _model.SensorId; }
            set
            {
                _model.SensorId = value;
                NotifyOfPropertyChange(() => SensorId);
            }
        }

        public int DeviceType
        {
            get { return _model.DeviceType; }
            set
            {
                _model.DeviceType = value;
                NotifyOfPropertyChange(() => DeviceType);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
