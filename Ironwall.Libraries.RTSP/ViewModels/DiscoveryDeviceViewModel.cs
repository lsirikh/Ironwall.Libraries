using Caliburn.Micro;
using Ironwall.Libraries.RTSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.ViewModels
{
    public class DiscoveryDeviceViewModel
        : Screen
        , IDiscoveryDeviceModel

    {

        #region - Ctors -
        public DiscoveryDeviceViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        public DiscoveryDeviceViewModel(DiscoveryDeviceModel model)
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

        public string IpAddress
        {
            get => Model.IpAddress;
            set
            {
                Model.IpAddress = value;
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get => Model.Port;
            set
            {
                Model.Port = value;
                NotifyOfPropertyChange(() => Port);
            }
        }

        public string UserName
        {
            get => Model.UserName;
            set
            {
                Model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }


        public string Password
        {
            get => Model.Password;
            set
            {
                Model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public string Profiles
        {
            get => Model.Profiles;
            set
            {
                Model.Profiles = value;
                NotifyOfPropertyChange(() => Profiles);
            }
        }


        public string Types
        {
            get => Model.Types;
            set
            {
                Model.Types = value;
                NotifyOfPropertyChange(() => Types);
            }
        }

        public string Mac
        {
            get => Model.Mac;
            set
            {
                Model.Mac = value;
                NotifyOfPropertyChange(() => Mac);
            }
        }

        public string DeviceModel
        {
            get => Model.DeviceModel;
            set
            {
                Model.DeviceModel = value;
                NotifyOfPropertyChange(() => DeviceModel);
            }
        }

        public string Company
        {
            get => Model.Company;
            set
            {
                Model.Company = value;
                NotifyOfPropertyChange(() => Company);
            }
        }

        public string Location
        {
            get => Model.Location;
            set
            {
                Model.Location = value;
                NotifyOfPropertyChange(() => Location);
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

        public DiscoveryDeviceModel Model
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
        private DiscoveryDeviceModel _model;
        private IEventAggregator _eventAggregator;

        private bool _isSelected;
        #endregion

    }
}
