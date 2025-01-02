using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Libraries.Map.Common.Services;
using Ironwall.Libraries.Map.UI.Providers.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.SymbolCollections;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Framework.Services;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Ironwall.Framework.Models.Communications.Symbols;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Map.Common.Helpers;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using Ironwall.Libraries.Map.UI.ViewModels.Panels;
using Ironwall.Libraries.Map.UI.Models.Messages;
using System.IO;
using Ironwall.Libraries.Map.UI.Views;
using System.Windows.Shapes;
using Ironwall.Libraries.Map.UI.Utils;
using Ironwall.Libraries.Map.UI.ViewModels.DesignComponents;
using Ironwall.Libraries.Map.UI.Models;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols.Components;
using System.Windows.Media.Imaging;
using ZoomAndPan;
using System.Runtime.Remoting.Contexts;
using System.Security.RightsManagement;
using System.Runtime.Remoting.Messaging;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/5/2023 9:32:43 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CanvasViewModel : BaseViewModel
    {

        #region - Ctors -
        public CanvasViewModel(IEventAggregator eventAggregator
                                , ILogService log
                                , SymbolCollectionViewModel symbolCollectionViewModel
                                ) : base(eventAggregator, log)
        {
            SymbolCollectionViewModel = symbolCollectionViewModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void OnViewAttached(object view, object context)
        {
            ZoomAndPanControl = (view as CanvasView).overview;
            _canvas = (view as CanvasView).canvas;
            _scroller = (view as CanvasView).scroller;
            base.OnViewAttached(view, context);
        }
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            ContentScale = 100;
            Coordinate = true;
            IsVisible = true;
            TempPolyLineViewModel = new TempPolyLineViewModel(_eventAggregator);
            await SymbolCollectionViewModel.ActivateAsync();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task<ISymbolViewModel> SymbolViewModelSelector(ISymbolModel selectedSymbol, Point position)
        {
            ISymbolViewModel viewModel = null;
            switch ((EnumShapeType)SelectedSymbol.TypeShape)
            {
                case EnumShapeType.NONE:
                    break;
                case EnumShapeType.TEXT:
                    {
                        await SymbolCollectionViewModel.SymbolProvider.InsertedItem(SelectedSymbol);
                        await Task.Run(() =>
                        {
                            viewModel = SymbolCollectionViewModel
                            .SymbolViewModelProvider
                            .Where(entity => entity.Id == SelectedSymbol.Id)
                            .FirstOrDefault();

                            viewModel.OnEditable = true;
                            viewModel.X = position.X - viewModel.Width / 2;
                            viewModel.Y = position.Y - viewModel.Height / 2;

                            SelectedSymbol = null;
                        });
                        DrawingFinished?.Invoke(this);
                    }
                    break;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                    {
                        await SymbolCollectionViewModel.SymbolProvider.InsertedItem(SelectedSymbol);
                        await Task.Run(() =>
                        {
                            viewModel = (IShapeSymbolViewModel)SymbolCollectionViewModel.SymbolViewModelProvider.Where(entity => entity.Id == SelectedSymbol.Id).FirstOrDefault();
                            viewModel.OnEditable = true;
                            viewModel.X = position.X - viewModel.Width / 2;
                            viewModel.Y = position.Y - viewModel.Height / 2;

                            SelectedSymbol = null;
                        });
                        DrawingFinished?.Invoke(this);
                    }
                    break;
                case EnumShapeType.POLYLINE:
                    break;
                case EnumShapeType.FENCE:
                    {
                        if (!(SelectedSymbol is IObjectShapeModel fenceViewModel))
                            return null;

                        InsertedPoints.Add(position);

                        if (InsertedPoints.Count > 1)
                        {
                            double? x, y;
                            UpdateCoordinate(InsertedPoints, out x, out y);
                            fenceViewModel.X = (double)x;
                            fenceViewModel.Y = (double)y;

                            fenceViewModel.Points = RegerateRPoint((double)x, (double)y);
                            _polyCount++;
                        }
                        else
                        {
                            _polyCount = 0;
                            if (fenceViewModel.Points == null)
                                fenceViewModel.Points = new PointCollection();

                            fenceViewModel.Points.Add(new Point(0, 0));
                            fenceViewModel.X = position.X;
                            fenceViewModel.Y = position.Y;
                        }

                        AddDesignSymbole(position);

                        fenceViewModel.Width = (double)GetWidth();
                        fenceViewModel.Height = (double)GetHeight();
                    }
                    break;
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    {
                        await SymbolCollectionViewModel.SymbolProvider.InsertedItem(SelectedSymbol);
                        
                        await Task.Run(async () =>
                        {
                            var searchCount = 0;
                            while ((IObjectShapeViewModel)SymbolCollectionViewModel
                             .SymbolViewModelProvider.Where(entity => entity.Id == SelectedSymbol.Id)
                             .FirstOrDefault() == null)
                            {
                                if (searchCount > 10)
                                    throw new Exception("SymbolViewModelProvider에서 데이터를 찾지 못했다.");

                                _log.Info($"Selected ViewModel을 찾지 못했다.");
                                await Task.Delay(100);
                                searchCount++;
                            }
                            viewModel = (IObjectShapeViewModel)SymbolCollectionViewModel.SymbolViewModelProvider
                            .Where(entity => entity.Id == SelectedSymbol.Id)
                            .FirstOrDefault();
                            viewModel.OnEditable = true;
                            viewModel.X = position.X - viewModel.Width / 2;
                            viewModel.Y = position.Y - viewModel.Height / 2;

                            SelectedSymbol = null;
                        });
                        DrawingFinished?.Invoke(this);
                    }
                    break;
                default:
                    break;
            }
            return viewModel;
        }

        private void AddDesignSymbole(Point position)
        {
            var ePoint = new EllipseViewModel(_polyCount);
            ePoint.X = position.X - ePoint.EllipseWidth / 2;
            ePoint.Y = position.Y - ePoint.EllipseHeight / 2;
            TempPolyLineViewModel.AddEllipse(ePoint);
            TempPolyLineViewModel.CreateLines();
        }

        private PointCollection RegerateRPoint(double originX, double originY)
        {
            return new PointCollection(InsertedPoints.Select(p => new Point(p.X - originX, p.Y - originY)).ToList());
        }

        private void UpdateCoordinate(ObservableCollection<Point> insertedPoints, out double? x, out double? y)
        {
            double? tempX = null;
            double? tempY = null;
            foreach (var item in insertedPoints)
            {
                if (!tempX.HasValue || !tempY.HasValue)
                {
                    tempX = item.X; tempY = item.Y;
                    continue;
                }

                if (tempX > item.X)
                {
                    tempX = item.X;
                }

                if (tempY > item.Y)
                {
                    tempY = item.Y;
                }
            }

            x = tempX;
            y = tempY;
        }

        private double? GetWidth()
        {
            if (!(InsertedPoints.Count > 0))
                return null;

            double? left = null;
            double? right = null;

            foreach (var point in InsertedPoints)
            {
                if (!left.HasValue || !right.HasValue)
                {
                    left = point.X;
                    right = point.X;

                    continue;
                }

                if (left >= point.X)
                {
                    left = point.X;
                }
                else if (right <= point.X)
                {
                    right = point.X;
                }
            }

            return right - left;
        }

        private double? GetHeight()
        {
            if (!(InsertedPoints.Count > 0))
                return null;

            double? top = null;
            double? bottom = null;

            foreach (var point in InsertedPoints)
            {
                if (!top.HasValue || !bottom.HasValue)
                {
                    top = point.Y;
                    bottom = point.Y;

                    continue;
                }
                else
                {
                    if (bottom <= point.Y)
                    {
                        bottom = point.Y;
                    }
                    else if (top >= point.Y)
                    {
                        top = point.Y;
                    }
                }
            }

            return bottom - top;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void scroller_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as ScrollViewer).Content is ZoomAndPanControl zoomAndPanControl)) return;

            _scroller.Focus();
            Keyboard.Focus(_scroller);
            //_canvas.Focus();
            //Keyboard.Focus(_canvas);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(this.ZoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(_canvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Middle)
            {
                // Just a plain old left-down initiates panning mode.
                mouseHandlingMode = MouseHandlingMode.Panning;
            }
            else if (mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                if (SelectedSymbol != null)
                    mouseHandlingMode = MouseHandlingMode.AddSymbol;
                else
                    CurrentPoint = origContentMouseDownPoint;

                _log.Info($"Current Point => ({CurrentPoint.X}, {CurrentPoint.Y})");
            }
            //else if (mouseButtonDown == MouseButton.Right)
            //{
            //    // Just a plain old left-down initiates panning mode.
            //    mouseHandlingMode = MouseHandlingMode.FinishDrawing;

            //}
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                this.ZoomAndPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        public async void scroller_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (!((sender as ScrollViewer).Content is ZoomAndPanControl zoomAndPanControl)) return;
            _scroller.Focus();
            Keyboard.Focus(_scroller);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(this.ZoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(_canvas);

            _log.Info($"Click Point => ({origContentMouseDownPoint.X}, {origContentMouseDownPoint.Y})");
            //ZoomAndPanControl.AnimatedZoomTo(2.0d, new Rect(origZoomAndPanControlMouseDownPoint.X, origZoomAndPanControlMouseDownPoint.Y, 600, 600));
            await GoToEventLocation(origContentMouseDownPoint);

        }

        public Task GoToEventLocation(Point point)
        {
            return Task.Run(() =>
            {
                DispatcherService.Invoke((System.Action)(() =>
                {
                    if (!IsEventZoom)
                    {
                        ZoomAndPanControl.ZoomAboutPoint(1.0d, point);
                        ZoomAndPanControl.AnimatedZoomTo(new Rect(point.X - 300, point.Y - 300, 600, 600));
                        IsEventZoom = true;
                    }
                    else
                    {
                        ZoomAndPanControl.AnimatedSnapTo(point);
                    }
                }));
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void scroller_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (mouseHandlingMode)
            {
                case MouseHandlingMode.None:
                    return;
                case MouseHandlingMode.DraggingRectangles:
                    break;
                case MouseHandlingMode.Panning:
                    break;
                case MouseHandlingMode.Zooming:
                    {
                        if (mouseButtonDown == MouseButton.Left)
                        {
                            // Shift + left-click zooms in on the content.
                            ZoomIn(origContentMouseDownPoint);
                        }
                        else if (mouseButtonDown == MouseButton.Right)
                        {
                            // Shift + left-click zooms out from the content.
                            ZoomOut(origContentMouseDownPoint);
                        }
                    }
                    break;
                case MouseHandlingMode.AddSymbol:
                    {
                        await SymbolViewModelSelector(SelectedSymbol, origContentMouseDownPoint);
                        //mouseHandlingMode != MouseHandlingMode.None
                    }
                    break;
                default:
                    break;
            }

            ZoomAndPanControl.ReleaseMouseCapture();
            mouseHandlingMode = MouseHandlingMode.None;
            e.Handled = true;
        }

        public void scroller_Focuse()
        {
            _scroller.Focus();
            Keyboard.Focus(_scroller);
            this.ZoomAndPanControl.CaptureMouse();
            _log.Info("scroller_Focuse was executed!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>         
        public void scroller_MouseMove(object sender, MouseEventArgs e)
        {
            if (!((sender as ScrollViewer).Content is ZoomAndPanControl zoomAndPanControl)) return;


            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(_canvas);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                ContentOffsetX -= dragOffset.X;
                ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void scroller_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                Point curContentMousePoint = e.GetPosition(_canvas);
                ZoomIn(curContentMousePoint);
            }
            else if (e.Delta < 0)
            {
                Point curContentMousePoint = e.GetPosition(_canvas);
                ZoomOut(curContentMousePoint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void scroller_KeyDown(object sender, KeyEventArgs e)
        {
            if (!((sender as ScrollViewer).Content is ZoomAndPanControl zoomAndPanControl)) return;

            _scroller.Focus();
            Keyboard.Focus(_scroller);
            //_canvas.Focus();
            //Keyboard.Focus(_canvas);

            if (e.Key == Key.Escape)
            {
                if ((SelectedSymbol is IObjectShapeModel model
                    && (model.TypeShape == (int)EnumShapeType.FENCE)))
                    keyboardHandlingModel = KeyboardHandlingMode.FinishDrawing;
                else
                    keyboardHandlingModel = KeyboardHandlingMode.EscapeSelection;
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                this.ZoomAndPanControl.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void scroller_KeyUp(object sender, KeyEventArgs e)
        {
            switch (keyboardHandlingModel)
            {
                case KeyboardHandlingMode.None:
                    return;
                case KeyboardHandlingMode.FinishDrawing:
                    await CloseDrawSymbolTask();
                    break;
                case KeyboardHandlingMode.EscapeSelection:
                    foreach (var item in SymbolCollectionViewModel.SymbolViewModelProvider)
                    {
                        item.IsEditable = false;
                    }
                    await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(false, null));
                    break;
                default:
                    break;
            }

            ZoomAndPanControl.ReleaseMouseCapture();
            keyboardHandlingModel = KeyboardHandlingMode.None;
            e.Handled = true;
        }

        /// <summary>
        /// PolyLine 계통의 Drawing을 종료 시 동작
        /// </summary>
        /// <returns></returns>
        private Task CloseDrawSymbolTask()
        {
            _log.Info($"{nameof(CloseDrawSymbolTask)} was executed!");
            return Task.Run(() =>
            {
                if ((!(SelectedSymbol is IObjectShapeModel model) || (model.TypeShape != (int)EnumShapeType.FENCE))) return;

                DispatcherService.Invoke((System.Action)(async () =>
                {
                    ISymbolViewModel viewModel = null;
                    var ret = await SymbolCollectionViewModel.SymbolProvider.InsertedItem(SelectedSymbol);

                    if (ret)
                    {
                        try
                        {
                            var searchCount = 0;
                            while ((IObjectShapeViewModel)SymbolCollectionViewModel
                             .SymbolViewModelProvider.Where(entity => entity.Id == SelectedSymbol.Id)
                             .FirstOrDefault() == null)
                            {
                                if (searchCount > 10) 
                                    throw new Exception("SymbolViewModelProvider에서 데이터를 찾지 못했다.");

                                _log.Info($"Selected ViewModel을 찾지 못했다.");
                                await Task.Delay(100);
                                searchCount++;
                            }
                            viewModel = (IObjectShapeViewModel)SymbolCollectionViewModel
                                        .SymbolViewModelProvider.Where(entity => entity.Id == SelectedSymbol.Id)
                                        .FirstOrDefault();

                            viewModel.OnEditable = true;
                            (viewModel as FenceObjectViewModel).originWidth = viewModel.Width;
                            (viewModel as FenceObjectViewModel).originHeight = viewModel.Height;
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"Rasied Exception of {nameof(CloseDrawSymbolTask)} in {nameof(CanvasViewModel)} : {ex.Message}");
                        }
                    }

                    SelectedSymbol = null;
                    InsertedPoints = null;

                    TempPolyLineViewModel.Clear();
                    mouseHandlingMode = MouseHandlingMode.None;
                    DrawingFinished?.Invoke(this);
                }));
            });
        }

        /// <summary>
        /// Zoom the viewport out by a small increment.
        /// </summary>
        public void ZoomOut(Point point = default)
        {
            try
            {
                if (ContentScale < 50) throw new IndexOutOfRangeException();
                if (point != null)
                    ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale - 0.1d, point);
                else
                    ZoomAndPanControl.ContentScale -= 0.1d;
            }
            catch (IndexOutOfRangeException)
            {
            }
            finally
            {
                NotifyOfPropertyChange(() => ContentScale);
                _log.Info($"ZoomOut : {ContentScale}%");
            }
        }

        /// <summary>
        /// Zoom the viewport in by a small increment.
        /// </summary>
        public void ZoomIn(Point point = default)
        {
            try
            {
                if (ContentScale > 300) throw new IndexOutOfRangeException();
                if (point != null)
                    ZoomAndPanControl.ZoomAboutPoint(ZoomAndPanControl.ContentScale + 0.1d, point);
                else
                    ZoomAndPanControl.ContentScale += 0.1d;
            }
            catch (IndexOutOfRangeException)
            {
            }
            finally
            {
                NotifyOfPropertyChange(() => ContentScale);
                _log.Info($"ZoomIn : {ContentScale}%");
            }
        }

        public void OnClickZoomIn()
        {
            ZoomIn();
        }

        public void OnClickZoomOut()
        {
            ZoomOut();
        }

        public Task OnClickSetZoom100()
        {
            return Task.Run((System.Action)(() =>
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    ZoomAndPanControl.AnimatedZoomTo(1.0);
                    await Task.Delay(500);
                    IsEventZoom = false;
                    NotifyOfPropertyChange(() => ContentScale);
                }));
            }));
        }

        public Task OnClickSetFill()
        {
            return Task.Run((System.Action)(() =>
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    ZoomAndPanControl.AnimatedScaleToFit();
                    await Task.Delay(500);
                    IsEventZoom = false;
                    NotifyOfPropertyChange(() => ContentScale);
                }));

            }));
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public double X
        {
            get => _x;
            set
            {
                _x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        public bool Coordinate
        {
            get { return _coordinate; }
            set
            {
                _coordinate = value;
                NotifyOfPropertyChange(() => Coordinate);
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public bool IsOnEditable
        {
            get { return _isOnEditable; }
            set
            {
                _isOnEditable = value;
                NotifyOfPropertyChange(() => IsOnEditable);
            }
        }

        public double ContentScale
        {
            get { return ZoomAndPanControl.ContentScale * 100; }
            set
            {

                _scale = value;
                NotifyOfPropertyChange(() => ContentScale);

                if (_scale != 0d)
                    ZoomAndPanControl.ContentScale = _scale / 100;
            }
        }

        public double ContentWidth
        {
            get { return _contentWidth; }
            set
            {
                _contentWidth = value;
                NotifyOfPropertyChange(() => ContentWidth);
            }
        }

        public double ContentHeight
        {
            get { return _contentHeight; }
            set
            {
                _contentHeight = value;
                NotifyOfPropertyChange(() => ContentHeight);
            }
        }

        public double ContentOffsetX
        {
            get { return ZoomAndPanControl.ContentOffsetX; }
            set
            {
                ZoomAndPanControl.ContentOffsetX = value;
                NotifyOfPropertyChange(() => ContentOffsetX);
            }

        }
        public double ContentOffsetY
        {
            get { return ZoomAndPanControl.ContentOffsetY; }
            set
            {
                ZoomAndPanControl.ContentOffsetY = value;
                NotifyOfPropertyChange(() => ContentOffsetY);
            }
        }

        public double ContentViewportWidth
        {
            get { return _contentViewportWidth; }
            set
            {
                _contentViewportWidth = value;
                NotifyOfPropertyChange(() => ContentViewportWidth);
            }
        }

        public double ContentViewportHeight
        {
            get { return _contentViewportHeight; }
            set
            {
                _contentViewportHeight = value;
                NotifyOfPropertyChange(() => ContentViewportHeight);
            }
        }

        public IMapViewModel SelectedMapViewModel
        {
            get { return _selectedMapViewModel; }
            set
            {
                _selectedMapViewModel = value;
                NotifyOfPropertyChange(() => SelectedMapViewModel);
            }
        }

        public TempPolyLineViewModel TempPolyLineViewModel
        {
            get { return _tempPolyLineViewModel; }
            set
            {
                _tempPolyLineViewModel = value;
                NotifyOfPropertyChange(() => TempPolyLineViewModel);
            }
        }

        private Point _currentPoint;

        public Point CurrentPoint
        {
            get { return _currentPoint; }
            set
            {
                _currentPoint = value;
                NotifyOfPropertyChange(() => CurrentPoint);
            }
        }

        public SymbolCollectionViewModel SymbolCollectionViewModel { get; }
        public ISymbolModel SelectedSymbol { get; set; }
        public ObservableCollection<Point> InsertedPoints { get; set; }
        public ZoomAndPanControl ZoomAndPanControl { get; private set; }
        public bool IsEventZoom { get; set; }
        #endregion
        #region - Attributes -
        private Canvas _canvas;
        private ScrollViewer _scroller;
        private double _scale;
        private double _contentWidth;
        private double _contentHeight;
        private double _contentViewportWidth;
        private double _contentViewportHeight;

        private double _x;
        private double _y;
        private bool _isVisible;
        private bool _isOnEditable;
        private bool _coordinate;
        private IMapViewModel _selectedMapViewModel;
        private TempPolyLineViewModel _tempPolyLineViewModel;
        private MouseButton mouseButtonDown;
        private Point origZoomAndPanControlMouseDownPoint;
        private Point origContentMouseDownPoint;
        private MouseHandlingMode mouseHandlingMode;
        private KeyboardHandlingMode keyboardHandlingModel;

        public event Action<object> DrawingFinished;

        private int _polyCount;
        #endregion
    }
}
