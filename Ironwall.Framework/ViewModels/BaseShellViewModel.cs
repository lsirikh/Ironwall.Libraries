using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ironwall.Libraries.Utils;

namespace Ironwall.Framework.ViewModels
{
    public abstract class BaseShellViewModel
        : Conductor<IScreen>
    {
        public BaseShellViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _eventAggregator.SubscribeOnPublishedThread(this);
        }

        ~BaseShellViewModel()
        {
            _eventAggregator.Unsubscribe(this);
        }

        #region - Event Handlers - 
        public void OnKeyDownToggleMaximize(object sender, KeyEventArgs e)
        {
            try
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    return;

                if (e.Key != Key.F11)
                    return;

                var window = sender as Window;
                /// 수정해야되는 부분!!!
                var task = window.Dispatcher.BeginInvoke(new Action<Window>(MaximizeWindow), window);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region - Procedures -
        private void MaximizeWindow(Window window)
        {
            var styleWindow = SourceChord.FluentWPF.AcrylicWindowStyle.None;

            switch (isFullScreen ^= true)
            {
                case true:
                    window.MaximizeToCurrentMonitor();
                    break;

                case false:
                default:
                    styleWindow = SourceChord.FluentWPF.AcrylicWindowStyle.Normal;
                    window.WindowState = WindowState.Normal;
                    break;
            }

            window.SetValue(SourceChord.FluentWPF.AcrylicWindow.AcrylicWindowStyleProperty, styleWindow);
        }
        #endregion

        #region - Properties -
        protected IEventAggregator _eventAggregator;
        #endregion

        #region - Attributes -
        protected bool isFullScreen = default;
        #endregion
    }
}
