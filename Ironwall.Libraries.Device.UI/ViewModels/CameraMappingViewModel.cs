using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Device.UI.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            _log = IoC.Get<ILogService>();
            _model = new CameraMappingModel();
        }

        public CameraMappingViewModel(ICameraMappingModel model) : base(model)
        {
            _log = IoC.Get<ILogService>();
            UpdateModel(model);
        }

        public override void Dispose()
        {
            _model = new CameraMappingModel();
            GC.Collect();
        }
        #endregion
        #region - Implementation of Interface -
        public override async void UpdateModel(ICameraMappingModel model)
        {
            base.UpdateModel(model);
            

            await FetchPrerequisits(model);
         
            var presetProvider = IoC.Get<PresetViewModelProvider>();
            if(model.FirstPreset != null) 
                FirstPreset = presetProvider.OfType<CameraPresetViewModel>().Where(entity => entity?.Id == model.FirstPreset.Id).FirstOrDefault();
            if(model.SecondPreset != null) 
                SecondPreset = presetProvider.OfType<CameraPresetViewModel>().Where(entity => entity?.Id == model.SecondPreset.Id).FirstOrDefault();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task FetchPrerequisits(ICameraMappingModel model)
        {
            var _checkCount = 1;
            while (true)
            {
                try
                {
                    if (_checkCount > 5) break;
                    var sensorPorvider = IoC.Get<SensorViewModelProvider>();
                    if (sensorPorvider.Count > 0)
                    {
                        Sensor = sensorPorvider.OfType<SensorDeviceViewModel>().Where(entity => entity?.Id == model.Sensor.Id).FirstOrDefault();
                        break;
                    }
                    _log.Info($"{nameof(UpdateModel)} of {nameof(CameraMappingViewModel)} was executed({_checkCount}) without {nameof(SensorViewModelProvider)}!");
                    await Task.Delay(1000);
                    _checkCount++;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string MappingGroup
        {
            get { return _model.MappingGroup; }
            set
            {
                _model.MappingGroup = value;
                NotifyOfPropertyChange(() => MappingGroup);
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
        private ILogService _log;
        #endregion
    }
}
