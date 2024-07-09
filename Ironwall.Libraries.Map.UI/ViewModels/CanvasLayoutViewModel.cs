using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Map.Common.Helpers;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Libraries.Map.Common.Services;
using Ironwall.Libraries.Map.UI.Models.Messages;
using Ironwall.Libraries.Map.UI.ViewModels.SymbolCollections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System;
using Ironwall.Libraries.Map.UI.ViewModels.Panels;
using System.Linq;
using System.Threading;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Map.UI.Providers.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using Ironwall.Framework.Models.Messages;
using System.Windows.Controls.Primitives;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/8/2023 5:29:32 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CanvasLayoutViewModel 
        : BaseViewModel
        , IHandle<EventMapChangeMessage>
        , IHandle<EditorSetupFromBaseMessage>
        , IHandle<RunSymbolDbLoadMessage>
        , IHandle<RunSymbolDbSaveMessage>
        , IHandle<RunMapSymbolClearMessage>
        , IHandle<RunMapDeleteMessage>
        , IHandle<RunMapDbSaveMessage>
    {

        #region - Ctors -
        public CanvasLayoutViewModel(IEventAggregator eventAggregator
                                    , ILogService log
                                    , MapDbService mapDbService
                                    , MapProvider mapProvider
                                    , MapViewModelProvider mapViewModelProvider
                                    , SymbolProvider symbolProvider
                                    , PointProvider pointProvider

                                    , SymbolCollectionViewModel symbolCollectionViewModel

                                    , CanvasViewModel canvasViewModel
                                    , CanvasOverlayViewModel canvasOverlayViewModel
                                    , MapStatusViewModel mapStatusViewModel
                                    , SymbolPropertyPanelViewModel symbolPropertyPanelViewModel)
            :base(eventAggregator)
        {
            _log = log;

            CanvasViewModel = canvasViewModel;
            CanvasOverlayViewModel = canvasOverlayViewModel;
            MapStatusViewModel = mapStatusViewModel;
            SymbolPropertyPanelViewModel = symbolPropertyPanelViewModel;

            MapViewModelProvider = mapViewModelProvider;
            MapViewModelProvider.Refresh += MapViewModelProvider_Refresh;

            _symbolCollectionViewModel = symbolCollectionViewModel;

            _symbolProvider = symbolProvider;
            _pointProvider = pointProvider;
            _mapDbService = mapDbService;
            _mapProvider = mapProvider;
        }

        private Task<bool> MapViewModelProvider_Refresh()
        {
            return Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100);
                    await ClearMapTask();
                    await SetSelectedMap();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await CanvasViewModel.ActivateAsync();
            CanvasViewModel.DrawingFinished += CanvasViewModel_DrawingFinished;
            await CanvasOverlayViewModel.ActivateAsync();
            await MapStatusViewModel.ActivateAsync();
            await SymbolPropertyPanelViewModel.ActivateAsync();

            _symbolCollectionViewModel.MappedSymbolViewModelProvider.Completed += MappedSymbolViewModelProvider_Completed;
            await base.OnActivateAsync(cancellationToken);
        }

        

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await CanvasViewModel.DeactivateAsync(true);
            CanvasViewModel.DrawingFinished -= CanvasViewModel_DrawingFinished;
            await CanvasOverlayViewModel.DeactivateAsync(true);
            await MapStatusViewModel.DeactivateAsync(true);
            await SymbolPropertyPanelViewModel.DeactivateAsync(true);
            
            _symbolCollectionViewModel.MappedSymbolViewModelProvider.Completed -= MappedSymbolViewModelProvider_Completed;
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void OnClickLoadMap(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button).DataContext is MapViewModel viewModel))
                return;

            SetSelectedMap(viewModel);
        }

        private Task SetSelectedMap(IMapViewModel viewModel = default)
        {

            return Task.Run(() => 
            {
                try
                {
                    if (viewModel == null)
                        viewModel = MapViewModelProvider.OrderBy(entity => entity.MapNumber).FirstOrDefault();
                    if (CanvasViewModel.SelectedMapViewModel == viewModel) return;
                    
                    DispatcherService.Invoke((System.Action)(async () =>
                    {
                        _symbolCollectionViewModel.IsVisible = false;
                        _symbolCollectionViewModel.MappedSymbolViewModelProvider.Clear();


                        ///UI 쓰레드에 의해 점유중인 자원에 대해선 자원에 
                        ///수정을 가할 때 UI 쓰레드의 Dispatcher에 작업을 위임해야 한다. 
                        CanvasViewModel.SelectedMapViewModel = viewModel;
                        if (CanvasViewModel.SelectedMapViewModel != null)
                        {
                            CanvasViewModel.SelectedMapViewModel.IsSelected = true;
                            CanvasViewModel.ContentWidth = CanvasViewModel.SelectedMapViewModel.Width;
                            CanvasViewModel.ContentHeight = CanvasViewModel.SelectedMapViewModel.Height;
                            CanvasViewModel.IsVisible = true;
                        }

                        CanvasOverlayViewModel.OverlayMapViewModel = viewModel;
                        if (CanvasOverlayViewModel.OverlayMapViewModel != null)
                        {
                            CanvasOverlayViewModel.Width = CanvasViewModel.SelectedMapViewModel.Width;
                            CanvasOverlayViewModel.Height = CanvasViewModel.SelectedMapViewModel.Height;
                            CanvasOverlayViewModel.IsVisible = true;
                        }

                        if (!CanvasOverlayViewModel.IsActive)
                            await CanvasOverlayViewModel.ActivateAsync();

                        _symbolCollectionViewModel.MappedSymbolViewModelProvider.SelectedMapNumber = viewModel.MapNumber;
                        await _symbolCollectionViewModel.MappedSymbolViewModelProvider.Provider_Initialize();

                    }));

                    _log.Info($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]{nameof(SetSelectedMap)} was finished!");

                }
                catch (Exception)
                {
                    throw;
                }
            });
            
        }

        private void MappedSymbolViewModelProvider_Completed()
        {
            _symbolCollectionViewModel.IsVisible = true;
        }

        private Task ClearMapTask()
        {
            try
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    foreach (var item in MapViewModelProvider)
                    {
                        item.IsSelected = false;
                    }
                    CanvasViewModel.SelectedMapViewModel = null;

                    if (CanvasOverlayViewModel.IsActive)
                        await CanvasOverlayViewModel.DeactivateAsync(true);

                    CanvasOverlayViewModel.OverlayMapViewModel = null;
                }));
                _log.Info($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]{nameof(ClearMapTask)} was finished!");
            }
            catch (Exception)
            {
                throw;
            }
            return Task.CompletedTask;
        }

      

        /// <summary>
        /// Menu Bar
        /// </summary>
        #region Create Symbol Logic
        private ISymbolModel CreateSymbol(EnumShapeType type)
        {
            try
            {
                var id = 0;
                if (_symbolCollectionViewModel.SymbolProvider.Count > 0)
                    id = _symbolCollectionViewModel.SymbolProvider.Max(item => item.Id) + 1;
                else
                    id = 0;

                ISymbolModel model = null;

                switch (type)
                {
                    case EnumShapeType.NONE:
                        break;
                    case EnumShapeType.TEXT:
                        {
                            model = new SymbolModel()
                            {
                                Id = id,
                                Width = WIDTH,
                                Height = HEIGHT,
                                TypeShape = (int)type,
                                IsShowLable = true,
                                Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                            };
                        }
                        break;
                    case EnumShapeType.LINE:
                    case EnumShapeType.TRIANGLE:
                    case EnumShapeType.RECTANGLE:
                    case EnumShapeType.POLYGON:
                    case EnumShapeType.ELLIPSE:
                    case EnumShapeType.POLYLINE:
                        {
                            model = new ShapeSymbolModel()
                            {
                                Id = id,
                                Width = WIDTH,
                                Height = HEIGHT,
                                TypeShape = (int)type,
                                Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                            };
                        }
                        break;
                    case EnumShapeType.FENCE:
                        {
                            model = new ObjectShapeModel()
                            {
                                Id = id,
                                TypeShape = (int)type,
                                TypeDevice = (int)EnumDeviceType.NONE,

                                ShapeFill = "#00FFFFFF",
                                ShapeStroke = "#FF40FF00",
                                Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                            };
                        }
                        break;
                    case EnumShapeType.CONTROLLER:
                        model = new ObjectShapeModel()
                        {
                            Id = id,
                            Width = WIDTH,
                            Height = HEIGHT,
                            ShapeFill = "#FFBF00",
                            TypeShape = (int)type,
                            TypeDevice = (int)EnumDeviceType.Controller,
                            Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                        };
                        break;
                    case EnumShapeType.MULTI_SNESOR:
                        {
                            model = new ObjectShapeModel()
                            {
                                Id = id,
                                Width = WIDTH,
                                Height = HEIGHT,
                                ShapeFill = "#FFBF00",
                                TypeShape = (int)type,
                                TypeDevice = (int)EnumDeviceType.Multi,
                                Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                            };
                        }
                        break;
                    case EnumShapeType.FENCE_SENSOR:
                        break;
                    case EnumShapeType.UNDERGROUND_SENSOR:
                        break;
                    case EnumShapeType.CONTACT_SWITCH:
                        break;
                    case EnumShapeType.PIR_SENSOR:
                        break;
                    case EnumShapeType.IO_CONTROLLER:
                        break;
                    case EnumShapeType.LASER_SENSOR:
                        break;
                    case EnumShapeType.CABLE:
                        break;
                    case EnumShapeType.IP_CAMERA:
                    case EnumShapeType.FIXED_CAMERA:
                    case EnumShapeType.PTZ_CAMERA:
                    case EnumShapeType.SPEEDDOM_CAMERA:
                        {
                            model = new ObjectShapeModel()
                            {
                                Id = id,
                                Width = WIDTH,
                                Height = HEIGHT,
                                ShapeFill = "#FFBF00",
                                TypeShape = (int)type,
                                TypeDevice = (int)EnumDeviceType.IpCamera,
                                Map = CanvasViewModel.SelectedMapViewModel.MapNumber,
                            };
                        }
                        break;
                    default:
                        break;
                }
                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Menu Logic
        
        #endregion

        #region Upper DropDown Menu
        /// <summary>
        /// 새로운 맵 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMapAdd(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 맵 정보 서버에 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnMapDbSave(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenConfirmPopupMessageModel()
            {
                //Title = Properties.Languages.Language.ProgramExit_Title,
                //Explain = Properties.Languages.Language.ProgramExit_Content,
                Title = "맵 이미지 저장",
                Explain = "맵 이미지 파일을 서버로 전송하시겠습니까?",
                MessageModel = new RunMapDbSaveMessage()
            });

        }

        /// <summary>
        /// 현재 맵의 심볼 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnMapSymbolClear(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenConfirmPopupMessageModel()
            {
                //Title = Properties.Languages.Language.ProgramExit_Title,
                //Explain = Properties.Languages.Language.ProgramExit_Content,
                Title = "맵 심볼 초기화 확인",
                Explain = "현재 맵의 심볼을 초기화 하시겠습니까?",
                MessageModel = new RunMapSymbolClearMessage()
            });
        }

        /// <summary>
        /// 현재 맵 삭제 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnMapDelete(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenConfirmPopupMessageModel()
            {
                //Title = Properties.Languages.Language.ProgramExit_Title,
                //Explain = Properties.Languages.Language.ProgramExit_Content,
                Title = "맵 삭제 확인",
                Explain = "현재의 맵을 삭제하시겠습니까?",
                MessageModel = new RunMapDeleteMessage()
            });
        }

        /// <summary>
        /// 맵 에디터 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnEditorExit(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(false, null));
            await _eventAggregator.PublishOnUIThreadAsync(new EditorSetupFromLocalMessage(false));
        }

        public async void OnSymbolDbSave(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenConfirmPopupMessageModel()
            {
                //Title = Properties.Languages.Language.ProgramExit_Title,
                //Explain = Properties.Languages.Language.ProgramExit_Content,
                Title = "심볼 편집 저장",
                Explain = "심별 편집 내용을 저장하시겠습니까?",
                MessageModel = new RunSymbolDbSaveMessage()
            });
        }


        public async void OnSymbolDbLoad(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnCurrentThreadAsync(new OpenConfirmPopupMessageModel()
            {
                //Title = Properties.Languages.Language.ProgramExit_Title,
                //Explain = Properties.Languages.Language.ProgramExit_Content,
                Title = "심볼 편집 불러오기",
                Explain = "심별 편집 내용을 불러오시겠습니까?",
                MessageModel = new RunSymbolDbLoadMessage()
            });
        }

        private void CanvasViewModel_DrawingFinished(object obj)
        {
            ToggleButtonClear();
        }

        private async void ToggleButtonHandler(string button, bool flag)
        {
            ToggleButtonClear();
            
            if (!flag) return;
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(false, null));
            switch (button)
            {
                case "Controller":
                    _isOnAddController = flag;
                    NotifyOfPropertyChange(() => IsOnAddController);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.CONTROLLER);
                    
                    break;
                case "Multi":
                    _isOnAddMultisensor = flag;
                    NotifyOfPropertyChange(() => IsOnAddMultisensor);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.MULTI_SNESOR);
                    break;
                case "GroupLine":
                    _isOnAddGroupLine = flag;
                    NotifyOfPropertyChange(() => IsOnAddGroupLine);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.FENCE);
                    CanvasViewModel.InsertedPoints = new ObservableCollection<Point>();
                    break;
                case "Body":
                    _isOnAddCamera = flag;
                    NotifyOfPropertyChange(() => IsOnAddCamera);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.FIXED_CAMERA);
                    break;
                case "Ellipse":
                    _isOnAddEllipse = flag;
                    NotifyOfPropertyChange(() => IsOnAddEllipse);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.ELLIPSE);
                    break;
                case "Rectangle":
                    _isOnAddRectangle = flag;
                    NotifyOfPropertyChange(() => IsOnAddRectangle);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.RECTANGLE);
                    break;
                case "Triangle":
                    _isOnAddTriangle = flag;
                    NotifyOfPropertyChange(() => IsOnAddTriangle);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.TRIANGLE);
                    break;
                case "Line":
                    _isOnAddLine = flag;
                    NotifyOfPropertyChange(() => IsOnAddLine);
                    break;
                case "PolyLine":
                    _isOnAddPolyLine = flag;
                    NotifyOfPropertyChange(() => IsOnAddPolyLine);
                    break;
                case "Text":
                    _isOnAddText = flag;
                    NotifyOfPropertyChange(() => IsOnAddText);
                    CanvasViewModel.SelectedSymbol = CreateSymbol(EnumShapeType.TEXT);
                    break;
                default:
                    break;
            }

            CanvasViewModel.scroller_Focuse();
        }

        private void ToggleButtonClear()
        {
            _isOnAddController = false;
            _isOnAddGroupLine = false;
            _isOnAddMultisensor = false;
            _isOnAddCamera = false;
            _isOnAddEllipse = false;
            _isOnAddRectangle = false;
            _isOnAddTriangle = false;
            _isOnAddLine = false;
            _isOnAddPolyLine = false;
            _isOnAddText = false;
            CanvasViewModel.SelectedSymbol = null;
            Refresh();
        }
        #endregion
        #endregion
        #region - IHanldes -
        public Task HandleAsync(EditorSetupFromBaseMessage message, CancellationToken cancellationToken)
        {
            IsOnEditable = message.IsOnEditable;

            return Task.CompletedTask;
        }

        public async Task HandleAsync(RunMapSymbolClearMessage message, CancellationToken cancellationToken)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

            await _symbolCollectionViewModel.SymbolProvider.ClearData();
            await Task.Delay(500);
            await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _cancellationTokenSource.Token);
        }

        public Task HandleAsync(RunMapDeleteMessage message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task HandleAsync(RunMapDbSaveMessage message, CancellationToken cancellationToken)
        {
            try
            {
                //if (_cancellationTokenSource != null 
                //    && !_cancellationTokenSource.IsCancellationRequested)
                //    _cancellationTokenSource.Cancel();

                //_cancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                var maps = new List<MapModel>();

                foreach (var item in _mapProvider.ToList())
                {
                    maps.Add(item as MapModel);
                }
                await _eventAggregator.PublishOnUIThreadAsync(new EditorMapSaveMessage(maps), _cancellationTokenSource.Token);

                //await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);
                //await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception)
            {

            }
            
        }

        public Task HandleAsync(RunSymbolDbLoadMessage message, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            { 
                try
                {
                    await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                    await _eventAggregator.PublishOnUIThreadAsync(new EditorSymbolLoadMessage());

                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception)
                {

                }
            });
        }

        public Task HandleAsync(RunSymbolDbSaveMessage message, CancellationToken cancellationToken)
        {
            return Task.Run(async () => 
            {
                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                await _mapDbService.SaveAllSymbols();

                var points = new List<PointClass>();
                var symbols = new List<SymbolModel>();
                var shapes = new List<ShapeSymbolModel>();
                var objects = new List<ObjectShapeModel>();

                foreach (var item in _pointProvider.ToList())
                {
                    points.Add(item as PointClass);
                }

                foreach (var item in _symbolProvider)
                {
                    if (!SymbolHelper.IsSymbolCategory(item.TypeShape)) continue;

                    symbols.Add(item as SymbolModel);
                }

                foreach (var item in _symbolProvider)
                {
                    if (!SymbolHelper.IsShapeCategory(item.TypeShape)) continue;

                    shapes.Add(item as ShapeSymbolModel);
                }

                foreach (var item in _symbolProvider)
                {
                    if (!SymbolHelper.IsObjectCategory(item.TypeShape)) continue;

                    objects.Add(item as ObjectShapeModel);
                }

                await _eventAggregator.PublishOnUIThreadAsync(new EditorSymbolSaveMessage(points, symbols, shapes, objects));
            });
            
        }

        public Task HandleAsync(EventMapChangeMessage message, CancellationToken cancellationToken)
        {
            var map = MapViewModelProvider.Where(entity => entity.MapNumber == message.MapNumber).FirstOrDefault() as MapViewModel;
            SetSelectedMap(map);
            return Task.CompletedTask;
        }


        #endregion
        #region - Properties -

        public bool IsOnEditable
        {
            get { return _isOnEditable; }
            set
            {
                _isOnEditable = value;
                NotifyOfPropertyChange(() => IsOnEditable);
            }
        }

        private bool _isOnAddController;

        public bool IsOnAddController
        {
            get { return _isOnAddController; }
            set 
            { 
                ToggleButtonHandler("Controller", value);
            }
        }

        

        private bool _isOnAddMultisensor;

        public bool IsOnAddMultisensor
        {
            get { return _isOnAddMultisensor; }
            set
            {
                ToggleButtonHandler("Multi", value);
            }
        }

        private bool _isOnAddGroupLine;

        public bool IsOnAddGroupLine
        {
            get { return _isOnAddGroupLine; }
            set
            {
                ToggleButtonHandler("GroupLine", value);
            }
        }

        private bool _isOnAddCamera;

        public bool IsOnAddCamera
        {
            get { return _isOnAddCamera; }
            set
            {
                ToggleButtonHandler("Body", value);
            }
        }

        private bool _isOnAddEllipse;

        public bool IsOnAddEllipse
        {
            get { return _isOnAddEllipse; }
            set
            {
                
                ToggleButtonHandler("Ellipse", value);
            }
        }

        private bool _isOnAddRectangle;

        public bool IsOnAddRectangle
        {
            get { return _isOnAddRectangle; }
            set
            {
                ToggleButtonHandler("Rectangle", value);
            }
        }

        private bool _isOnAddTriangle;

        public bool IsOnAddTriangle
        {
            get { return _isOnAddTriangle; }
            set
            {
                ToggleButtonHandler("Triangle", value);
            }
        }

        private bool _isOnAddPolyLine;

        public bool IsOnAddPolyLine
        {
            get { return _isOnAddPolyLine; }
            set
            {
                ToggleButtonHandler("PolyLine", value);
            }
        }

        private bool _isOnAddLine;

        public bool IsOnAddLine
        {
            get { return _isOnAddLine; }
            set
            {
                ToggleButtonHandler("Line", value);
            }
        }


        private bool _isOnAddText;

        public bool IsOnAddText
        {
            get { return _isOnAddText; }
            set
            {
                ToggleButtonHandler("Text", value);
            }
        }

        
        public MapStatusViewModel MapStatusViewModel { get; }
        public SymbolPropertyPanelViewModel SymbolPropertyPanelViewModel { get; }
        public MapViewModelProvider MapViewModelProvider { get; }

        public CanvasViewModel CanvasViewModel { get; }
        public CanvasOverlayViewModel CanvasOverlayViewModel { get; }
        #endregion
        #region - Attributes -
        private bool _isOnEditable;
        private SymbolCollectionViewModel _symbolCollectionViewModel;
        private SymbolProvider _symbolProvider;
        private PointProvider _pointProvider;
        private MapDbService _mapDbService;
        private MapProvider _mapProvider;
        private ILogService _log;

        public const double WIDTH = 80;
        public const double HEIGHT = 60;
        #endregion
    }
}
