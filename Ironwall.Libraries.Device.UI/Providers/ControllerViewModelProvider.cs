using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Devices.Providers;
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
        Created On   : 6/9/2023 10:06:03 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class ControllerViewModelProvider : DeviceBaseViewModelProvider<IControllerDeviceViewModel>
    {

        #region - Ctors -
        public ControllerViewModelProvider(DeviceProvider provider)
        {
            ClassName = nameof(ControllerViewModelProvider);
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

                foreach (IBaseDeviceModel item in _provider.ToList())
                {
                    if (!(item is IControllerDeviceModel model)) continue;

                    var viewModel = ViewModelFactory.Build<ControllerDeviceViewModel>(model);
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

        private async Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            try
            {
                if (!(item is IControllerDeviceModel model)) return false;

                var viewModel = ViewModelFactory.Build<ControllerDeviceViewModel>(model);
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

        private async Task<bool> Provider_Update(IBaseDeviceModel item)
        {
            try
            {
                if (!(item is IControllerDeviceModel model)) return false;

                var viewModel = ViewModelFactory.Build<ControllerDeviceViewModel>(model);
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

        private Task<bool> Provider_Delete(IBaseDeviceModel item)
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
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private DeviceProvider _provider;
        #endregion
    }
}
