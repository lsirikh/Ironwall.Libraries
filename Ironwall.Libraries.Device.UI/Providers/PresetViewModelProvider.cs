using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Devices.Providers.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Framework.Models.Devices;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/26/2023 4:41:47 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PresetViewModelProvider : OptionViewModelProvider<ICameraPresetViewModel>
    {

        #region - Ctors -
        public PresetViewModelProvider(CameraPresetProvider provider)
        {
            ClassName = nameof(PresetViewModelProvider);
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
        private async Task<bool> Provider_Initialize()
        {
            try
            {
                Clear();

                foreach (ICameraPresetModel item in _provider.ToList())
                {
                    var viewModel = ViewModelFactory.Build<CameraPresetViewModel>(item);
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
        private async Task<bool> Provider_Insert(ICameraPresetModel item)
        {
            try
            {
                var viewModel = ViewModelFactory.Build<CameraPresetViewModel>(item);
                await viewModel.ActivateAsync();
                Add(viewModel);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return false;
            }
        }

        private async Task<bool> Provider_Update(ICameraPresetModel item)
        {
            try
            {
                var viewModel = ViewModelFactory.Build<CameraPresetViewModel>(item);
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


        private Task<bool> Provider_Delete(ICameraPresetModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

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
        #endregion
        #region - Attributes -
        private CameraPresetProvider _provider;
        #endregion
    }
}
