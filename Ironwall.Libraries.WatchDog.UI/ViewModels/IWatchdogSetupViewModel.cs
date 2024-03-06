namespace Ironwall.Libraries.WatchDog.UI.ViewModels
{
    public interface IWatchdogSetupViewModel
    {
        bool CanActivateWatchdog { get; }
        bool CanDeactivateWatchdog { get; }
        bool IsEnabledToggleButtonWatchdog { get; set; }
        bool IsWatchdogActive { get; set; }
        string WatchdogStatus { get; set; }

        void ActivateWatchdog();
        void DeactivateWatchdog();

        void Initialize();
    }
}