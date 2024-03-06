using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ironwall.Framework.Services
{
    public static class DispatcherService
    {
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.Invoke(action);
        }

        public static async Task BeginInvoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current != null ? Application.Current.Dispatcher : null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                await dispatchObject.BeginInvoke(action);
        }
    }
}
