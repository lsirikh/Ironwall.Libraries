using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public class CameraPresetViewModel
        : CameraBaseViewModel
        , ICameraPresetViewModel
    {
        #region - Ctors -
        public CameraPresetViewModel()
        {
        }
        public CameraPresetViewModel(ICameraPresetModel model)
        {
            Model = model;
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
        public string NameArea
        {
            get => Model.NameArea;
            set
            {
                Model.NameArea = value;
                NotifyOfPropertyChange(() => NameArea);

            }
        }

        public int IdController
        {
            get => Model.IdController;
            set
            {
                Model.IdController = value;
                NotifyOfPropertyChange(() => IdController);
            }
        }

        public int IdSensorBgn
        {
            get => Model.IdSensorBgn;
            set
            {
                Model.IdSensorBgn = value;
                NotifyOfPropertyChange(() => IdSensorBgn);
            }
        }

        public int IdSensorEnd
        {
            get => Model.IdSensorEnd;
            set
            {
                Model.IdSensorEnd = value;
                NotifyOfPropertyChange(() => IdSensorEnd);
            }
        }

        public string CameraFirst
        {
            get => Model.CameraFirst;
            set
            {
                Model.CameraFirst = value;
                NotifyOfPropertyChange(() => CameraFirst);

                if (Notify != null)
                    Notify(this, new PropertyNotifyEventArgs() { Property = "CameraFirst" });
            }
        }

        public int TypeDeviceFirst
        {
            get => Model.TypeDeviceFirst;
            set
            {
                Model.TypeDeviceFirst = value;
                NotifyOfPropertyChange(() => TypeDeviceFirst);
            }
        }

        public string HomePresetFirst
        {
            get => Model.HomePresetFirst;
            set
            {
                Model.HomePresetFirst = value;
                NotifyOfPropertyChange(() => HomePresetFirst);
            }
        }

        public string TargetPresetFirst
        {
            get => Model.TargetPresetFirst;
            set
            {
                Model.TargetPresetFirst = value;
                NotifyOfPropertyChange(() => TargetPresetFirst);
            }
        }

        public string CameraSecond
        {
            get => Model.CameraSecond;
            set
            {
                Model.CameraSecond = value;
                NotifyOfPropertyChange(() => CameraSecond);

                if (Notify != null)
                    Notify(this, new PropertyNotifyEventArgs() { Property = "CameraSecond" });
            }
        }

        public int TypeDeviceSecond
        {
            get => Model.TypeDeviceSecond;
            set
            {
                Model.TypeDeviceSecond = value;
                NotifyOfPropertyChange(() => TypeDeviceSecond);
            }
        }

        public string HomePresetSecond
        {
            get => Model.HomePresetSecond;
            set
            {
                Model.HomePresetSecond = value;
                NotifyOfPropertyChange(() => HomePresetSecond);
            }
        }

        public string TargetPresetSecond
        {
            get => Model.TargetPresetSecond;
            set
            {
                Model.TargetPresetSecond = value;
                NotifyOfPropertyChange(() => TargetPresetSecond);
            }
        }

        public int ControlTime
        {
            get => Model.ControlTime;
            set
            {
                Model.ControlTime = value;
                NotifyOfPropertyChange(() => ControlTime);
            }
        }
        public ICameraPresetModel Model
        {
            get { return _model as ICameraPresetModel; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        #endregion
        #region - Attributes -
        public event EventHandler Notify;
        #endregion
    }
}
