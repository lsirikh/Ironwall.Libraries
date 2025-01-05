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
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Framework.ViewModels
{
    public abstract class BaseShellViewModel: Conductor<IScreen>
    {
        public BaseShellViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _log = IoC.Get<ILogService>();
            _class = this.GetType();
            _eventAggregator.SubscribeOnPublishedThread(this);
        }
        protected BaseShellViewModel(IEventAggregator eventAggregator, ILogService log)
        {
            _eventAggregator = eventAggregator;
            _log = log;
            _class = this.GetType();
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
                _log.Error(ex.Message);
            }
        }
        #endregion
        #region - Procedures -
        private void MaximizeWindow(Window window)
        {
            try
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
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }

        }
        #endregion
        #region - Properties -
        protected IEventAggregator _eventAggregator;
        protected ILogService _log;
        protected Type _class;
        #endregion
        #region - Attributes -
        protected bool isFullScreen = default;
        #endregion
    }
}
