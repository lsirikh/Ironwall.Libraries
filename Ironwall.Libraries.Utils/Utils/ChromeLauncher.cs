using Microsoft.Win32;

namespace Ironwall.Libraries.Utils
{
    public static class WebBrowser
    {
        #region - Static Procedures -
        public static string WebBrowserChrome
        {
            get
            {
                return (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ??
                                    Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null));
            }
        }
        #endregion

        #region - Attributes = 
        private const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
        #endregion
    }
}
