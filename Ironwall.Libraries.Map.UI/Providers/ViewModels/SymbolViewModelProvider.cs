﻿using Caliburn.Micro;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Map.UI.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ironwall.Framework.Services;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.Common.Providers.Models;
using System.Threading;
using System.Collections.ObjectModel;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 1:57:11 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolViewModelProvider : SymbolBaseViewModelProvider<ISymbolViewModel>
    {

        #region - Ctors -
        public SymbolViewModelProvider(SymbolProvider provider)
        {
            ClassName = nameof(SymbolViewModelProvider);
            _provider = provider;
            _lock = new object();
        }
        #endregion
        #region - Implementation of Interface -
        public override Task<bool> Initialize(CancellationToken token = default)
        {
            _provider.Refresh += Provider_Initialize;
            _provider.Inserted += Provider_Insert;
            _provider.Updated += Provider_Update;
            _provider.Deleted += Provider_Delete;

            return Provider_Initialize();
        }

        public override void Uninitialize()
        {
            _provider.Refresh -= Provider_Initialize;
            _provider.Inserted -= Provider_Insert;
            _provider.Updated -= Provider_Update;
            _provider.Deleted -= Provider_Delete;
            _provider = null;

            Clear();

            GC.Collect();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task<bool> Provider_Initialize()
        {
            try
            {
                Clear();
                foreach (var item in _provider.OfType<ISymbolModel>().ToList())
                {
                    await AddProcess(item);
                }
                await Finished();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return false;
            }

            return true;
        }
        private async Task<bool> Provider_Insert(ISymbolModel item)
        {
            try
            {
                await AddProcess(item);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return false;
            }
            return true;
        }

        private Task<bool> Provider_Update(ISymbolModel item)
        {
            bool ret = false;
            try
            {
                ret = UpdateProcess(item);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }


        private Task<bool> Provider_Delete(ISymbolModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        //Remove(searchedItem);
                        ret = await DeletedItem(searchedItem);
                    }

                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }

        private async Task AddProcess(ISymbolModel item, bool isUseIndex = false)
        {
            ISymbolViewModel viewModel = null;
            try
            {
                switch ((EnumShapeType)item.TypeShape)
                {
                    case EnumShapeType.NONE:
                        break;
                    case EnumShapeType.TEXT:
                        {
                            viewModel = MapViewModelFactory.Build<TextSymbolViewModel>(item);
                            await viewModel.ActivateAsync();

                        }
                        break;
                    case EnumShapeType.LINE:
                        break;
                    case EnumShapeType.TRIANGLE:
                        {
                            if (!(item is IShapeSymbolModel model)) return;

                            viewModel = MapViewModelFactory.Build<TriangleShapeViewModel>(model);
                            await viewModel.ActivateAsync();

                        }
                        break;
                    case EnumShapeType.RECTANGLE:
                        {
                            if (!(item is IShapeSymbolModel model)) return;

                            viewModel = MapViewModelFactory.Build<RectangleShapeViewModel>(model);
                            await viewModel.ActivateAsync();
                        }
                        break;
                    case EnumShapeType.POLYGON:
                        break;
                    case EnumShapeType.POLYLINE:
                        break;
                    case EnumShapeType.FENCE:
                        {
                            if (!(item is IObjectShapeModel model)) return;

                            viewModel = MapViewModelFactory.Build<FenceObjectViewModel>(model);
                            await viewModel.ActivateAsync();
                        }
                        break;
                    case EnumShapeType.ELLIPSE:
                        {
                            if (!(item is IShapeSymbolModel model)) return;

                            viewModel = MapViewModelFactory.Build<EllipseShapeViewModel>(model);
                            await viewModel.ActivateAsync();
                        }
                        break;
                    case EnumShapeType.CONTROLLER:
                        {
                            if (!(item is IObjectShapeModel model)) return;

                            viewModel = MapViewModelFactory.Build<ControllerObjectViewModel>(model);
                            await viewModel.ActivateAsync();
                        }
                        break;
                    case EnumShapeType.MULTI_SNESOR:
                        {
                            if (!(item is IObjectShapeModel model)) return;

                            viewModel = MapViewModelFactory.Build<MultiSensorObjectViewModel>(model);
                            await viewModel.ActivateAsync();
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
                        break;
                    case EnumShapeType.FIXED_CAMERA:
                        {
                            if (!(item is IObjectShapeModel model)) return;

                            viewModel = MapViewModelFactory.Build<FixedCameraObjectViewModel>(model);
                            await viewModel.ActivateAsync();
                        }
                        break;
                    case EnumShapeType.PTZ_CAMERA:
                        break;
                    case EnumShapeType.SPEEDDOM_CAMERA:
                        break;
                    default:
                        break;
                }

                if (viewModel == null)
                    throw new InvalidOperationException($"Failed to create ViewModel for TypeShape {item.TypeShape}.");

                if (!isUseIndex)
                    await InsertedItem(viewModel);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(AddProcess)}({ClassName}) : {ex.Message}");
            }

        }

        private bool UpdateProcess(ISymbolModel item)
        {
            try
            {
                ISymbolViewModel searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                if (searchedItem == null)
                    throw new NullReferenceException(message: $"SymbolModel({item.Id}) was not Searched in SymbolCollection");

                switch ((EnumShapeType)item.TypeShape)
                {
                    case EnumShapeType.NONE:
                        break;
                    case EnumShapeType.TEXT:
                        {
                            var viewModel = MapViewModelFactory.Build<TextSymbolViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.LINE:
                        break;
                    case EnumShapeType.TRIANGLE:
                        {
                            var viewModel = MapViewModelFactory.Build<TriangleShapeViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.RECTANGLE:
                        {
                            var viewModel = MapViewModelFactory.Build<RectangleShapeViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.POLYGON:
                        break;
                    case EnumShapeType.ELLIPSE:
                        {
                            var viewModel = MapViewModelFactory.Build<EllipseShapeViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.POLYLINE:
                        break;
                    case EnumShapeType.FENCE:
                        {
                            var viewModel = MapViewModelFactory.Build<FenceObjectViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.CONTROLLER:
                        {
                            var viewModel = MapViewModelFactory.Build<ControllerObjectViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.MULTI_SNESOR:
                        {
                            var viewModel = MapViewModelFactory.Build<MultiSensorObjectViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
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
                        break;
                    case EnumShapeType.FIXED_CAMERA:
                        {
                            var viewModel = MapViewModelFactory.Build<FixedCameraObjectViewModel>(item);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumShapeType.PTZ_CAMERA:
                        break;
                    case EnumShapeType.SPEEDDOM_CAMERA:
                        break;
                    default:
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(AddProcess)}({ClassName}) : {ex.Message}");
                return false;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private SymbolProvider _provider;
        private object _lock;
        #endregion
    }
}
