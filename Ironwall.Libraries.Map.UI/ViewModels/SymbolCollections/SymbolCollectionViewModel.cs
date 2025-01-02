using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Libraries.Map.UI.Models.Messages;
using Ironwall.Libraries.Map.UI.Providers.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels.SymbolCollections
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 2:53:55 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolCollectionViewModel : Screen
                                            , IHandle<EditShapeMessage>
                                            , IHandle<DeleteShapeMessage>
                                            , IHandle<CopyShapeMessage>
    {
        #region - Ctors -
        public SymbolCollectionViewModel(IEventAggregator eventAggregator
                                        , ILogService log
                                        , SymbolProvider provider
                                        , PointProvider pointProvider
                                        , SymbolViewModelProvider symbolViewModelProvider
                                        , MappedSymbolViewModelProvider mappedSymbolViewModelProvider)
        {
            _eventAggregator = eventAggregator;
            _log = log;
            SymbolProvider = provider;
            PointProvider = pointProvider;
            SymbolViewModelProvider = symbolViewModelProvider;
            MappedSymbolViewModelProvider = mappedSymbolViewModelProvider;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator.SubscribeOnUIThread(this);
            return base.OnActivateAsync(cancellationToken);
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public Task HandleAsync(EditShapeMessage message, CancellationToken cancellationToken)
        {
            if (message.ViewModel == null)
            {
                foreach (var item in SymbolViewModelProvider.ToList())
                {
                    item.IsEditable = false;
                }
                _log.Info($"SymbolViewModel의 IsEditable 속성이 false로 설정되었습니다.");
            }
            else
            {
                foreach (var item in SymbolViewModelProvider.ToList())
                {
                    if (item != message.ViewModel)
                    {
                        item.IsEditable = false;
                    }
                }
                _log.Info($"SymbolViewModel의 IsEditable 속성이 true로 설정되었습니다.");
            }

            return Task.CompletedTask;
        }

        public async Task HandleAsync(DeleteShapeMessage message, CancellationToken cancellationToken)
        {
            if (message.symbolModel == null)
                return;

            var model = message.symbolModel;
            await SymbolProvider.DeletedItem(model);
            switch ((EnumShapeType)model.TypeShape)
            {
                case EnumShapeType.NONE:
                    break;
                case EnumShapeType.TEXT:
                    break;

                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                case EnumShapeType.FENCE:
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
                        if (!(model is ShapeSymbolModel shapeSymbol)) break;

                        PointProvider.RemoveAll(entity => entity.PointGroup == shapeSymbol.Id);
                    }
                    break;
                default:
                    break;
            }
        }

        public async Task HandleAsync(CopyShapeMessage message, CancellationToken cancellationToken)
        {
            if (message.symbolModel == null)
                return;

            var id = 0;
            if (SymbolProvider.Count > 0)
                id = SymbolProvider.Max(item => item.Id) + 1;
            else
                id = 0;

            switch ((EnumShapeType)message.symbolModel.TypeShape)
            {
                case EnumShapeType.NONE:
                    break;
                case EnumShapeType.TEXT:
                    {
                        var model = MapModelFactory.Build<SymbolModel>(message.symbolModel);
                        model.Id = id;
                        model.X += model.Width;
                        model.Y += model.Height;

                        await SymbolProvider.InsertedItem(model);
                        await Task.Run(() =>
                        {
                            var viewModel = (ISymbolViewModel)SymbolViewModelProvider.Where(entity => entity.Id == model.Id).FirstOrDefault();
                            viewModel.OnEditable = true;
                        });
                    }
                    break;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                    {
                        var model = MapModelFactory.Build<ShapeSymbolModel>(message.symbolModel as IShapeSymbolModel);
                        model.Id = id;
                        model.X += model.Width;
                        model.Y += model.Height;

                        await SymbolProvider.InsertedItem(model);
                        await Task.Run(() =>
                        {
                            var viewModel = (IShapeSymbolViewModel)SymbolViewModelProvider.Where(entity => entity.Id == model.Id).FirstOrDefault();
                            viewModel.OnEditable = true;
                        });
                    }
                    break;
                case EnumShapeType.FENCE:
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
                        var model = MapModelFactory.Build<ObjectShapeModel>(message.symbolModel as IObjectShapeModel);
                        model.Id = id;
                        model.X += model.Width;
                        model.Y += model.Height;

                        await SymbolProvider.InsertedItem(model);
                        await Task.Run(() =>
                        {
                            var viewModel = (IShapeSymbolViewModel)SymbolViewModelProvider.Where(entity => entity.Id == model.Id).FirstOrDefault();
                            viewModel.OnEditable = true;
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public void GenerateMappedSymbol(MapViewModel mapViewModel)
        {
            MappedCollectionEntity = new ObservableCollection<ISymbolViewModel>(SymbolViewModelProvider.Where(entity => entity.Map == mapViewModel.MapNumber).ToList());
            NotifyOfPropertyChange(() => MappedCollectionEntity);
        }

        public async Task SetMappedSymbols(int selectedMapNumber)
        {
            MappedSymbolViewModelProvider.SelectedMapNumber = selectedMapNumber;
            await Task.Delay(100);
            await MappedSymbolViewModelProvider.Provider_Initialize();
        }
        public void ClearMappedSymbols() => MappedSymbolViewModelProvider.Clear();

        #endregion
        #region - Properties -

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public SymbolProvider SymbolProvider { get; }
        public PointProvider PointProvider { get; }
        public SymbolViewModelProvider SymbolViewModelProvider { get; }
        public MappedSymbolViewModelProvider MappedSymbolViewModelProvider { get; }
        public ObservableCollection<ISymbolViewModel> MappedCollectionEntity { get; set; }
        public MapViewModel SelectedMapViewModel { get; set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        private ILogService _log;
        private bool _isVisible;
        #endregion
    }
}
