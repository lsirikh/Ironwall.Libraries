using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Libraries.Map.UI.ViewModels;
using Ironwall.Libraries.Map.UI.Models;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Map.Common.Providers.Models;
using System.Threading;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    public class MapViewModelProvider : MapBaseViewModelProvider
    {
        #region - Ctors -
        public MapViewModelProvider(MapProvider provider)
        {
            ClassName = nameof(MapViewModelProvider);
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
            return Task.Run(async () =>
            {
                try
                {
                    Clear();
                    foreach (var item in _provider.OrderBy(entity => entity.Id).ToList())
                    {
                        var viewModel = MapViewModelFactory.Build<MapViewModel>(item);
                        Add(viewModel);
                    }
                    await Finished();

                    return true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }
            });
        }

        private Task<bool> Provider_Insert(IMapModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = MapViewModelFactory.Build<MapViewModel>(item);
                    Add(viewModel, 0);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }

        private Task<bool> Provider_Update(IMapModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = MapViewModelFactory.Build<MapViewModel>(item);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = viewModel;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return false;
                }
                return true;
            });
        }

        private Task<bool> Provider_Delete(IMapModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                        Remove(searchedItem);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                    return false;
                }
                return true;
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private MapProvider _provider;
        #endregion
    }
}
