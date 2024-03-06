using Ironwall.Framework.Models.Devices;
using System.Runtime.InteropServices;
using System;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/26/2023 4:05:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetViewModel : CameraOptionViewModel, ICameraPresetViewModel
    {

        #region - Ctors -
        public CameraPresetViewModel()
        {
            _model = new CameraPresetModel();
        }

        public CameraPresetViewModel(ICameraPresetModel model) : base(model)
        {
            _model = model;
        }

        #endregion
        #region - Implementation of Interface -
        public override void Dispose()
        {
            _model = new CameraPresetModel();
            GC.Collect();
        }

        public void UpdateModel(ICameraPresetModel model)
        {
            _model = model;
            Refresh();
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

        public string PresetName
        {
            get { return (_model as ICameraPresetModel).PresetName; }
            set
            {
                (_model as ICameraPresetModel).PresetName = value;
                NotifyOfPropertyChange(() => PresetName);
            }
        }

        public bool IsHome
        {
            get { return (_model as ICameraPresetModel).IsHome; }
            set
            {
                (_model as ICameraPresetModel).IsHome = value;
                NotifyOfPropertyChange(() => IsHome);
            }
        }

        public double Pan
        {
            get { return (_model as ICameraPresetModel).Pan; }
            set
            {
                (_model as ICameraPresetModel).Pan = value;
                NotifyOfPropertyChange(() => Pan);
            }
        }

        public double Tilt
        {
            get { return (_model as ICameraPresetModel).Tilt; }
            set
            {
                (_model as ICameraPresetModel).Tilt = value;
                NotifyOfPropertyChange(() => Tilt);
            }
        }

        public double Zoom
        {
            get { return (_model as ICameraPresetModel).Zoom; }
            set
            {
                (_model as ICameraPresetModel).Zoom = value;
                NotifyOfPropertyChange(() => Zoom);
            }
        }

        public int Delay
        {
            get { return (_model as ICameraPresetModel).Delay; }
            set
            {
                (_model as ICameraPresetModel).Delay = value;
                NotifyOfPropertyChange(() => Delay);
            }
        }

        #endregion
        #region - Attributes -
        #endregion
    }
}
