using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Device.UI.ViewModels.Panels;
using Ironwall.Libraries.Device.UI.Views.Panels;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ironwall.Libraries.Device.UI.ViewModels.Dashboards
{
    public class DeviceDashBoardViewModel : ConductorOneViewModel
    {
        #region - Ctors -
        public DeviceDashBoardViewModel(IEventAggregator eventAggregator
                                        , DeviceProvider deviceProvider
                                        
            ) : base(eventAggregator)
        {
            _deviceProvider = deviceProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            ControllerPanelViewModel = IoC.Get<ControllerPanelViewModel>();
            SensorPanelViewModel = IoC.Get<SensorPanelViewModel>();
            CameraPanelViewModel = IoC.Get<CameraPanelViewModel>();

            NotifyOfPropertyChange(() => ControllerPanelViewModel);
            NotifyOfPropertyChange(() => SensorPanelViewModel);
            NotifyOfPropertyChange(() => CameraPanelViewModel);

            await base.OnActivateAsync(cancellationToken);

            await DataInitialize(cancellationToken);
            
            await GetDeviceType(cancellationToken);

            await ControllerPanelViewModel.ActivateAsync();

        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async void OnActiveTab(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                if (!(args.Source is TabControl)) return;

                if (!(args.AddedItems[0] is TabItem tapItem)) return;

                var contentControl = (tapItem as ContentControl)?.Content as ContentControl;

                if (selectedViewModel != null)
                    await selectedViewModel.DeactivateAsync(true);

                if (contentControl?.Content is ControllerPanelView)
                {
                    selectedViewModel = ControllerPanelViewModel;
                }
                else if (contentControl?.Content is SensorPanelView)
                {
                    selectedViewModel = SensorPanelViewModel;
                }
                else if (contentControl?.Content is CameraPanelView)
                {
                    selectedViewModel = CameraPanelViewModel;
                }

                await selectedViewModel.ActivateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(OnActiveTab)} : {ex.Message}");
            }

        }

        private Task DataInitialize(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(500);
                DeviceProvider = IoC.Get<DeviceViewModelProvider>();
                NotifyOfPropertyChange(() => DeviceProvider);
            });
        }

        private void ClearData()
        {
            Controller = 0;
            MultiSensor = 0;
            FenseSensor = 0;
            ContactSensor = 0;
            UndergroundSensor = 0;
            PIRSensor = 0;
            IOController = 0;
            LaserSensor = 0;
            IPCamera = 0;
        }

        private Task GetDeviceType(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                Controller = DeviceProvider.OfType<IControllerDeviceViewModel>().Count();
                await Task.Delay(200, cancellationToken);
                MultiSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.Multi).Count();
                await Task.Delay(200, cancellationToken);
                FenseSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.Fence).Count();
                await Task.Delay(200, cancellationToken);
                ContactSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.Contact).Count();
                UndergroundSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.Underground).Count();
                PIRSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.PIR).Count();
                IOController = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.IoController).Count();
                LaserSensor = DeviceProvider.OfType<ISensorDeviceViewModel>().Where(t => t.DeviceType == (int)EnumDeviceType.Laser).Count();
                await Task.Delay(200, cancellationToken);
                IPCamera = DeviceProvider.OfType<ICameraDeviceViewModel>().Count();
            });
        }

        public DeviceViewModelProvider DeviceProvider { get; private set; }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;
                NotifyOfPropertyChange(() => Controller);
            }
        }

        public int MultiSensor
        {
            get { return _multiSensor; }
            set
            {
                _multiSensor = value;
                NotifyOfPropertyChange(() => MultiSensor);
            }
        }
        public int FenseSensor
        {
            get { return _fenseSensor; }
            set
            {
                _fenseSensor = value;
                NotifyOfPropertyChange(() => FenseSensor);
            }
        }

        public int UndergroundSensor
        {
            get { return _undergroundSensor; }
            set
            {
                _undergroundSensor = value;
                NotifyOfPropertyChange(() => UndergroundSensor);
            }
        }

        public int ContactSensor
        {
            get { return _contactSensor; }
            set
            {
                _contactSensor = value;
                NotifyOfPropertyChange(() => ContactSensor);
            }
        }
        public int PIRSensor
        {
            get { return _pirSensor; }
            set
            {
                _pirSensor = value;
                NotifyOfPropertyChange(() => PIRSensor);
            }
        }
        public int IOController
        {
            get { return _ioController; }
            set
            {
                _ioController = value;
                NotifyOfPropertyChange(() => IOController);
            }
        }
        public int LaserSensor
        {
            get { return _laserSensor; }
            set
            {
                _laserSensor = value;
                NotifyOfPropertyChange(() => LaserSensor);
            }
        }
        public int IPCamera
        {
            get { return _ipCamera; }
            set
            {
                _ipCamera = value;
                NotifyOfPropertyChange(() => IPCamera);
            }
        }

        public ControllerPanelViewModel ControllerPanelViewModel { get; private set; }
        public SensorPanelViewModel SensorPanelViewModel { get; private set; }
        public CameraPanelViewModel CameraPanelViewModel { get; private set; }

        #endregion
        #region - Attributes -
        private int _controller;
        private int _multiSensor;
        private int _fenseSensor;
        private int _undergroundSensor;
        private int _contactSensor;
        private int _pirSensor;
        private int _ioController;
        private int _laserSensor;
        private int _ipCamera;
        private DeviceProvider _deviceProvider;
        BaseViewModel selectedViewModel;
        #endregion
    }
}
