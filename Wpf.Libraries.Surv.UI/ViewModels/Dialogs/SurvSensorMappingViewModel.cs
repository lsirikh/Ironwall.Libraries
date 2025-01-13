using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System;
using Wpf.Libraries.Surv.UI.Providers.ViewModels;
using System.Linq;

namespace Wpf.Libraries.Surv.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/6/2023 2:30:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvSensorMappingViewModel : BaseViewModel
    {

        #region - Ctors -
        public SurvSensorMappingViewModel(ILogService log
                                        , IEventAggregator eventAggregator)
                                        : base(eventAggregator, log)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            _ = DataInitialize(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {

            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task InsertSensorCollection(IEnumerable<string> sensors)
        {
            return Task.Run(() =>
            {
                SensorProvider = new ObservableCollection<string>();
                foreach (var sensor in sensors)
                {
                    SensorProvider.Add(sensor);
                }
            });

        }
        private Task DataInitialize(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {

                Maximum = 100;
                TickFrequency = 1;
                ItemCount = 0;

                SelectedGroupItem = null;
                SelectedSensorItem = null;
                LastSensor = null;
                SelectedSensors = null;

                await Task.Delay(500);
                //ViewModelProvider Setting
                GroupProvider = IoC.Get<SurvMappingViewModelProvider>();
                NotifyOfPropertyChange(() => GroupProvider);
            });
        }

        public void OnChangeValue(object sendor, EventArgs e)
        {
            if (SelectedSensorItem == null) return;

            var startIndex = SensorProvider.IndexOf(SelectedSensorItem);

            try
            {
                LastSensor = SensorProvider[startIndex + ItemCount - 1];
                SelectedSensors = SensorProvider.Skip(startIndex).Take(ItemCount).ToList();
            }
            catch
            {
            }

        }

        public void OnGroupComboChanged(object sendor, EventArgs e)
        {

            //if (SelectedGroupItem == null) return;

            ItemCount = 0;

        }

        public void OnSensorComboChanged(object sendor, EventArgs e)
        {

            //if (SelectedSensorItem == null) return;

            ItemCount = 0;

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        public int TickFrequency
        {
            get { return _tickFrequency; }
            set
            {
                _tickFrequency = value;
                NotifyOfPropertyChange(() => TickFrequency);
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }
            set
            {
                _itemCount = value;
                NotifyOfPropertyChange(() => ItemCount);
            }
        }


        public SurvMappingViewModel SelectedGroupItem
        {
            get { return _selectedGroupItem; }
            set
            {
                _selectedGroupItem = value;
                NotifyOfPropertyChange(() => SelectedGroupItem);
            }
        }

        public string SelectedSensorItem
        {
            get { return _selectedSensorItem; }
            set
            {
                _selectedSensorItem = value;
                NotifyOfPropertyChange(() => SelectedSensorItem);
            }
        }

        public SurvMappingViewModelProvider GroupProvider { get; private set; }
        public ObservableCollection<string> SensorProvider { get; private set; }

        public string LastSensor
        {
            get { return _lastSensor; }
            set
            {
                _lastSensor = value;
                NotifyOfPropertyChange(() => LastSensor);
            }
        }

        public List<string> SelectedSensors { get; private set; }
        #endregion
        #region - Attributes -
        private int _maximum;
        private int _tickFrequency;
        private int _itemCount;
        private SurvMappingViewModel _selectedGroupItem;
        private string _selectedSensorItem;
        private string _lastSensor;
        #endregion
    }
}
