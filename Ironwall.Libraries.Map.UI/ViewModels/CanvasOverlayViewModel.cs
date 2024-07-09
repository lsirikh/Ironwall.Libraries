using Caliburn.Micro;
using System.Windows.Controls;
using System.Windows;
using Ironwall.Libraries.Map.UI.Views;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Map.UI.Providers.ViewModels;
using System;
using System.Linq;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Map.UI.ViewModels.SymbolCollections;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using ZoomAndPan;
using Ironwall.Libraries.Base.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/8/2023 9:38:06 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CanvasOverlayViewModel : Screen
    {

        #region - Ctors -
        public CanvasOverlayViewModel(ILogService log
                                    , IEventAggregator eventAggregator
                                    , CanvasViewModel canvasViewModel) 
        {
            _log = log;
            _eventAggregator = eventAggregator;
            CanvasViewModel = canvasViewModel;
            ClassName = nameof(CanvasOverlayViewModel);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator?.SubscribeOnUIThread(this);
            _cancellationTokenSource = new CancellationTokenSource();
            _ = DataInitialize(_cancellationTokenSource.Token);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator?.Unsubscribe(this);
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            return base.OnDeactivateAsync(close, cancellationToken);
        }

        protected override void OnViewAttached(object view, object context)
        {
            _zoomAndPanControl = (view as CanvasOverlayView).overview;
            _thumb = (view as CanvasOverlayView).overviewZoomRectThumb;
            base.OnViewAttached(view, context);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void overlay_SizeChanged(object sender, RoutedEventArgs e)
        {
            _zoomAndPanControl.ScaleToFit();
            _log.Info($"{nameof(overlay_SizeChanged)}");
        }

        public void overlay_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var locX = Canvas.GetLeft(_thumb);
            var locY = Canvas.GetTop(_thumb);
            //Debug.WriteLine($"{nameof(overlay_DragDelta)} : {locX}, {locY} ==> {e.HorizontalChange}, {e.VerticalChange}");
            double newContentOffsetX = Math.Min(Math.Max(0.0, Canvas.GetLeft(_thumb) + e.HorizontalChange), CanvasViewModel.ContentWidth - CanvasViewModel.ContentViewportWidth);
            Canvas.SetLeft(_thumb, newContentOffsetX);

            double newContentOffsetY = Math.Min(Math.Max(0.0, Canvas.GetTop(_thumb) + e.VerticalChange), CanvasViewModel.ContentHeight - CanvasViewModel.ContentViewportHeight);
            Canvas.SetTop(_thumb, newContentOffsetY);
            //Debug.WriteLine($"{nameof(overlay_DragDelta)} : {newContentOffsetX}, {newContentOffsetY}");
        }

        private object DataInitialize(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    IsVisible = false;
                    //ViewModelProvider Setting
                    if (cancellationToken.IsCancellationRequested) new TaskCanceledException("Task was cancelled!");

                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        SymbolCollectionViewModel = IoC.Get<SymbolCollectionViewModel>();
                    }));
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)} in {ClassName}) : {ex.Message}");
                }

            }, cancellationToken);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IMapViewModel OverlayMapViewModel
        {
            get { return _overlayMapViewModel; }
            set 
            { 
                _overlayMapViewModel = value;
                NotifyOfPropertyChange(() => OverlayMapViewModel);
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set 
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public SymbolCollectionViewModel SymbolCollectionViewModel
        {
            get { return _symbolCollectionViewModel; }
            set { _symbolCollectionViewModel = value;  NotifyOfPropertyChange(() => SymbolCollectionViewModel); }
        }

        public CanvasViewModel CanvasViewModel { get; }
        public string ClassName { get; }
        #endregion
        #region - Attributes -
        private ILogService _log;
        private IEventAggregator _eventAggregator;
        private ZoomAndPanControl _zoomAndPanControl;
        private Thumb _thumb;

        private IMapViewModel _overlayMapViewModel;
        private SymbolCollectionViewModel _symbolCollectionViewModel;

        private double _width;
        private double _height;
        private CancellationTokenSource _cancellationTokenSource;
        private const int ACTION_TOKEN_TIMEOUT = 5000;
        #endregion
    }
}
