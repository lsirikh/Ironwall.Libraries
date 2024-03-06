using Wpf.AxisAudio.Common;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 1:34:30 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSensorViewModel : AudioBaseViewModel<IAudioSensorModel>
    {

        #region - Ctors -
        public AudioSensorViewModel(IAudioSensorModel model) : base(model)
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
            get { return Group?.GroupName; }
        }

        public AudioGroupBaseModel Group
        {
            get { return _model.Group; }
            set
            {
                _model.Group = value;
                NotifyOfPropertyChange(() => Group);
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
