using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.UI.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/12/2023 1:50:45 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MappedSymbolViewModelProvider : SymbolBaseViewModelProvider<ISymbolViewModel>
    {

        #region - Ctors -
        public MappedSymbolViewModelProvider(SymbolViewModelProvider provider)
        {
            ClassName = nameof(SymbolViewModelProvider);
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

            SelectedMapNumber = 1;
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
        public Task<bool> Provider_Initialize()
        {
            try
            {
                Clear();
                foreach (var item in _provider.OfType<ISymbolViewModel>().ToList())
                {
                    if (item.Map != SelectedMapNumber) continue;
                    Add(item);
                }
                Completed?.Invoke();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private Task<bool> Provider_Insert(ISymbolViewModel item)
        {
            try
            {
                if (item.Map != SelectedMapNumber) return Task.FromResult(false);
                Add(item);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private Task<bool> Provider_Update(ISymbolViewModel item)
        {
            try
            {
                if (item.Map != SelectedMapNumber) return Task.FromResult(false);

                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                if (searchedItem != null)
                    searchedItem = item;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        private Task<bool> Provider_Delete(ISymbolViewModel item)
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
                _log.Error($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        //public MapViewModel MapViewModel { get; set; }
        public int SelectedMapNumber { get; set; }
        #endregion
        #region - Attributes -
        private SymbolViewModelProvider _provider;
        public event Action Completed;
        #endregion
    }
}
