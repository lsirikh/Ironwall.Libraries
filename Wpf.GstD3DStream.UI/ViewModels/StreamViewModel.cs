using Caliburn.Micro;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Wpf.GstD3DStream.UI.Controls;
using Wpf.GstD3DStream.UI.Utils;
using Wpf.GstD3DStream.UI.Views;

namespace Wpf.GstD3DStream.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/19/2024 4:26:22 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class StreamViewModel : Screen, IDisposable
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    try
                    {
                        // Dispose 로직을 여기에 구현
                        DispatcherService.Invoke(() =>
                        {
                            _streamView.Dispose();
                            _streamView = null;
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }
        #endregion
        #region - Overrides -
        protected override void OnViewAttached(object view, object context)
        {
            var viewData = view as StreamView;
            _streamView = viewData.D3DStreamView;
            base.OnViewAttached(view, context);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Title
        {
            get => _title;
            set { _title = value; NotifyOfPropertyChange(() => Uri); }
        }

        public string Uri
        {
            get => _uri;
            set { _uri = value; NotifyOfPropertyChange(() => Uri); }
        }

        #endregion
        #region - Attributes -
        private string _title;
        private string _uri;
        private IEventAggregator _eventAggregator;
        private D3DStreamView _streamView;
        private readonly object _disposeLock = new object();
        private bool _disposed;
        #endregion
    }
}
