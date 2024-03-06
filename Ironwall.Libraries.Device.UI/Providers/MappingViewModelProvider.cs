using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Devices.Providers.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/6/2023 9:08:01 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MappingViewModelProvider : EntityCollectionProvider<ICameraMappingViewModel>, ILoadable
    {

        #region - Ctors -
        public MappingViewModelProvider(CameraMappingProvider provider)
        {
            ClassName = nameof(PresetViewModelProvider);
            _provider = provider;
        }

        #endregion
        #region - Implementation of Interface -
        public Task<bool> Initialize(CancellationToken token = default)
        {
            _provider.Refresh += Provider_Initialize;
            _provider.Inserted += Provider_Insert;
            _provider.Updated += Provider_Update;
            _provider.Deleted += Provider_Delete;

            return Provider_Initialize();
        }

        

        public void Uninitialize()
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

                foreach (var item in _provider.ToList())
                {
                    var viewModel = ViewModelFactory.Build<CameraMappingViewModel>(item);
                    await viewModel.ActivateAsync();
                    Add(viewModel);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return false;
            }
            
        }

        private Task<bool> Provider_Insert(ICameraMappingModel item)
        {
                try
                {
                    var viewModel = ViewModelFactory.Build<CameraMappingViewModel>(item);
                    Add(viewModel);
                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return Task.FromResult(false);
                }
        }

        private async Task<bool> Provider_Update(ICameraMappingModel item)
        {
                try
                {
                    var viewModel = ViewModelFactory.Build<CameraMappingViewModel>(item);
                    await viewModel.ActivateAsync();
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                        searchedItem = viewModel;
                    
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return false;
                }
        }

        private Task<bool> Provider_Delete(ICameraMappingModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                {
                    Remove(searchedItem);
                }
                
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string ClassName { get; }
        #endregion
        #region - Attributes -
        private CameraMappingProvider _provider;

        #endregion
    }
}
