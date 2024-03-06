using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Ironwall.Libraries.WatchDog.UI.Models;
using System;

namespace Ironwall.Libraries.WatchDog.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 2:24:21 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class WatchdogSetupViewModel : BaseViewModel, IWatchdogSetupViewModel
    {

        #region - Ctors -
        public WatchdogSetupViewModel(IEventAggregator eventAggregator
                                     , WatchdogSetupModel setupModel)
                                    : base(eventAggregator)
        {
            _setupModel = setupModel;
        }
        #endregion
        #region - Implementation of Interface -
        public void Initialize()
        {
            ProcessControl.Instance.SetTarget(_setupModel.WatchdogProcess);
            if (_setupModel.IsAutoWatchdog)
                ActivateWatchdog();

        }
        #endregion
        #region - Overrides -
        protected async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await CheckWatchdogTaskAsync(cancellationToken);
            await base.OnActivateAsync(cancellationToken);
        }

        private Task<bool> CheckWatchdogTaskAsync(CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                Process[] wprocs = Process.GetProcessesByName(_setupModel.WatchdogProcess);
                if (wprocs.Length > 0)
                {
                    IsWatchdogActive = true;
                    WatchdogStatus = "Activated";
                    return true;
                }
                else
                {
                    IsWatchdogActive = false;
                    WatchdogStatus = "Deactivated";
                    return false;
                }
            }, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public bool CanActivateWatchdog => true;
        public void ActivateWatchdog()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (IsWatchdogActive)
                        return;

                    ///프로그램 시작
                    var ret = ProcessControl.Instance.StartProcess();

                    if (ret)
                        await CheckWatchdogTaskAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}({ex.ToString()})");
                }

            });
        }
        public bool CanDeactivateWatchdog => true;
        public void DeactivateWatchdog()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (!IsWatchdogActive)
                        return;

                    var ret = ProcessControl.Instance.Terminate();
                    if (ret)
                        await CheckWatchdogTaskAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}({ex.ToString()})");
                }

            });
        }

        
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsEnabledToggleButtonWatchdog
        {
            get => _setupModel.IsAutoWatchdog;
            set
            {
                _setupModel.IsAutoWatchdog = value;
                NotifyOfPropertyChange(() => IsEnabledToggleButtonWatchdog);
            }
        }

        public string WatchdogStatus
        {
            get { return _watchdogStatus; }
            set
            {
                _watchdogStatus = value;
                NotifyOfPropertyChange(() => WatchdogStatus);
            }
        }

        public bool IsWatchdogActive { get; set; }
        #endregion
        #region - Attributes -
        private string _watchdogStatus;
        private WatchdogSetupModel _setupModel;
        #endregion
    }
}
