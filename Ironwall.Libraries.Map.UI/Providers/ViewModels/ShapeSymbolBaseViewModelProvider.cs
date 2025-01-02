using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.Common.Helpers;
using Ironwall.Libraries.Map.Common.Providers.Models;
using Ironwall.Libraries.Map.UI.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 2:05:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ShapeSymbolBaseViewModelProvider<T> : SymbolBaseViewModelProvider<IShapeSymbolViewModel> where T : ShapeSymbolViewModel
    {

        #region - Ctors -
        public ShapeSymbolBaseViewModelProvider(SymbolProvider provider)
        {
            ClassName = nameof(ShapeSymbolBaseViewModelProvider<T>);
            _provider = provider;
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
        private Task<bool> Provider_Initialize()
        {
            try
            {
                Clear();

                foreach (var item in _provider.OfType<IShapeSymbolModel>().ToList())
                {
                    if (!(SymbolHelper.IsShapeCategory(item.TypeShape))) continue;

                    var viewModel = MapViewModelFactory.Build<T>(item);
                    //viewModel.ActivateAsync();
                    Add(viewModel);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        private Task<bool> Provider_Insert(ISymbolModel item)
        {
            try
            {
                var viewModel = MapViewModelFactory.Build<T>((IShapeSymbolModel)item);
                Add(viewModel, 0);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        private Task<bool> Provider_Update(ISymbolModel item)
        {
            try
            {
                var viewModel = MapViewModelFactory.Build<T>((IShapeSymbolModel)item);
                //viewModel.ActivateAsync();
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                if (searchedItem != null)
                    searchedItem = viewModel;
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
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                {
                    Remove(searchedItem);
                }

            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        protected SymbolProvider _provider;
        #endregion
    }
}
