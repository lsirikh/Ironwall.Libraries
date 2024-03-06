using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Ironwall.Libraries.Utils
{
    public static class ScreenManager
    {
        public static void MaximizeToCurrentMonitor(this Window window)
        {
            window.WindowState = WindowState.Maximized;
        }

        public static void MaximizeToFirstMonitor(this Window window)
        {
            var screen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

            if (screen != null)
            {
                MaximizeWindow(window, screen);
            }
        }

        public static void MaximizeToSecondMonitor(this Window window)
        {
            var screen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (screen != null)
            {
                MaximizeWindow(window, screen);
            }        
        }

        public static void MaximizeDualMonitor(this Window window, bool isTopmost = false)
        {
            window.Left = 0;
            window.Top = 0;
            window.Width = SystemParameters.VirtualScreenWidth;
            window.Height = SystemParameters.VirtualScreenHeight;
            window.Topmost = isTopmost;
        }

        private static void MaximizeWindow(Window window, Screen secondaryScreen)
        {
            if (!window.IsLoaded)
                window.WindowStartupLocation = WindowStartupLocation.Manual;

            var workingArea = secondaryScreen.WorkingArea;
            window.Left = workingArea.Left;
            window.Top = workingArea.Top;
            window.Width = workingArea.Width;
            window.Height = workingArea.Height;

            if (window.IsLoaded)
                window.WindowState = WindowState.Maximized;
        }
    }
}
