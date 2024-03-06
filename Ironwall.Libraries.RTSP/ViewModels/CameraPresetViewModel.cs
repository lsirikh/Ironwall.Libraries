using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.RTSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.ViewModels
{
    public class CameraPresetViewModel
        : Screen
        , ICameraPresetModel
    {
        #region - Ctors -
        public CameraPresetViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        public CameraPresetViewModel(CameraPresetModel model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            Model = model;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            base.OnActivateAsync(cancellationToken);
            return Task.CompletedTask;
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            base.OnDeactivateAsync(close, cancellationToken);
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Id
        {
            get => Model.Id;
            set
            {
                Model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

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
                
                if(Notify != null)
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

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public CameraPresetModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        #endregion
        #region - Attributes -
        private CameraPresetModel _model;
        private IEventAggregator _eventAggregator;
        public event EventHandler Notify;
        private bool _isSelected;
        #endregion
    }
}
