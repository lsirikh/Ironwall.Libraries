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
        public CanvasOverlayViewModel(SymbolCollectionViewModel symbolCollectionViewModel
                                      , CanvasViewModel canvasViewModel)
        {
            SymbolCollectionViewModel = symbolCollectionViewModel;
            CanvasViewModel = canvasViewModel;
        }

        //private Task<bool> MapViewModelProvider_Refresh()
        //{
        //    try
        //    {
        //        OverlayMapViewModel = _mapViewModelProvider.OrderBy(entity => entity.MapNumber).FirstOrDefault() as MapViewModel;

        //        Width = OverlayMapViewModel.Width;
        //        Height = OverlayMapViewModel.Height;

        //        return Task.FromResult(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Task.FromResult(false);
        //    }
        //}

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void OnViewAttached(object view, object context)
        {
            _zoomAndPanControl = (view as CanvasOverlayView).overview;
            _thumb = (view as CanvasOverlayView).overviewZoomRectThumb;
            base.OnViewAttached(view, context);
        }
        
        public void overlay_SizeChanged(object sender, RoutedEventArgs e)
        {
            _zoomAndPanControl.ScaleToFit();
            Debug.WriteLine($"{nameof(overlay_SizeChanged)}");
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
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
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

        public SymbolCollectionViewModel SymbolCollectionViewModel { get; }
        public CanvasViewModel CanvasViewModel { get; }

        #endregion
        #region - Attributes -
        private ZoomAndPanControl _zoomAndPanControl;
        private Thumb _thumb;

        private IMapViewModel _overlayMapViewModel;

        private double _width;
        private double _height;
        #endregion
    }
}
