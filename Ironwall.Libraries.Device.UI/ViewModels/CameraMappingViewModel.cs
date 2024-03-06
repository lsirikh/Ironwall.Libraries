using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Device.UI.Providers;
using System;
using System.Linq;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/6/2023 9:11:56 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingViewModel : BaseCustomViewModel<ICameraMappingModel>, ICameraMappingViewModel
    {

        #region - Ctors -
        public CameraMappingViewModel()
        {

        }

        public CameraMappingViewModel(ICameraMappingModel model) : base(model)
        {
            UpdateModel(model);
        }

        public override void Dispose()
        {
            _model = new CameraMappingModel();
            GC.Collect();
        }
        #endregion
        #region - Implementation of Interface -
        public override void UpdateModel(ICameraMappingModel model)
        {
            base.UpdateModel(model);
            var sensorPorvider = IoC.Get<SensorViewModelProvider>();
            Sensor = sensorPorvider.OfType<SensorDeviceViewModel>().Where(entity => entity?.Id == model.Sensor.Id).FirstOrDefault();
         
            var presetProvider = IoC.Get<PresetViewModelProvider>();
            FirstPreset = presetProvider.OfType<CameraPresetViewModel>().Where(entity => entity?.Id == model.FirstPreset.Id).FirstOrDefault();
            SecondPreset = presetProvider.OfType<CameraPresetViewModel>().Where(entity => entity?.Id == model.SecondPreset.Id).FirstOrDefault();
        }
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
        public string Group
        {
            get { return _model.Group; }
            set
            {
                _model.Group = value;
                NotifyOfPropertyChange(() => Group);
            }
        }
        public SensorDeviceViewModel Sensor
        {
            get { return _sensor; }
            set
            {
                _sensor = value;
                NotifyOfPropertyChange(() => Sensor);
                _model.Sensor = value?.Model as SensorDeviceModel;
            }
        }

        public CameraPresetViewModel FirstPreset
        {
            get { return _firstPreset; }
            set
            {
                _firstPreset = value;
                NotifyOfPropertyChange(() => FirstPreset);
                _model.FirstPreset = value?.Model as CameraPresetModel;
            }
        }

        public CameraPresetViewModel SecondPreset
        {
            get { return _secondPreset; }
            set
            {
                _secondPreset = value;
                NotifyOfPropertyChange(() => SecondPreset);
                _model.SecondPreset = value?.Model as CameraPresetModel;
            }
        }
        #endregion
        #region - Attributes -
        private SensorDeviceViewModel _sensor;
        private CameraPresetViewModel _firstPreset;
        private CameraPresetViewModel _secondPreset;
        #endregion
    }
}
