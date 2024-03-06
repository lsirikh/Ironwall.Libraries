using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ironwall.Libraries.Device.UI.ViewModels;
using Caliburn.Micro;
using System.Threading;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 9:50:03 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class DeviceViewModelProvider : DeviceBaseViewModelProvider<IDeviceViewModel>
    {
        #region - Ctors -
        public DeviceViewModelProvider(DeviceProvider provider)
        {
            ClassName = nameof(DeviceViewModelProvider);
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

                foreach (IBaseDeviceModel item in _provider.ToList())
                {
                    AddProcess(item);
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
        }


        private Task<bool> Provider_Insert(IBaseDeviceModel item)
        {
            try
            {
                AddProcess(item, true);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
        }


        private Task<bool> Provider_Update(IBaseDeviceModel item)
        {
            try
            {
                bool ret = false;
                ret = UpdateProcess(item);
                return Task.FromResult(ret);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
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
                return Task.FromResult(false);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                return Task.FromResult(false);
            }
        }


        private void AddProcess(IBaseDeviceModel item, bool isUseIndex = false)
        {
            try
            {
                DeviceViewModel viewModel = null;

                switch ((EnumDeviceType)item.DeviceType)
                {
                    case EnumDeviceType.NONE:
                        break;
                    case EnumDeviceType.Controller:
                        {
                            viewModel = ViewModelFactory.Build<ControllerDeviceViewModel>(item as IControllerDeviceModel);
                        }
                        break;
                    case EnumDeviceType.Multi:
                    case EnumDeviceType.Fence:
                    case EnumDeviceType.Underground:
                    case EnumDeviceType.Contact:
                    case EnumDeviceType.PIR:
                    case EnumDeviceType.IoController:
                    case EnumDeviceType.Laser:
                        {
                            viewModel = ViewModelFactory.Build<SensorDeviceViewModel>(item as ISensorDeviceModel);
                        }
                        break;
                    case EnumDeviceType.Cable:
                        break;
                    case EnumDeviceType.IpCamera:
                        {
                            viewModel = ViewModelFactory.Build<CameraDeviceViewModel>(item as ICameraDeviceModel);
                        }
                        break;
                    default:
                        break;
                }

                viewModel.ActivateAsync();
                if (!isUseIndex)
                    Add(viewModel);
                else
                    Add(viewModel, 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(AddProcess)}({ClassName}) : {ex.Message}");
            }
        }


        private bool UpdateProcess(IBaseDeviceModel item)
        {
            try
            {
                IDeviceViewModel searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                if (searchedItem == null)
                    throw new NullReferenceException(message: $"SymbolModel({item.Id}) was not Searched in SymbolCollection");

                switch ((EnumDeviceType)item.DeviceType)
                {
                    case EnumDeviceType.NONE:
                        break;
                    case EnumDeviceType.Controller:
                        {
                            var viewModel = ViewModelFactory.Build<ControllerDeviceViewModel>(item as IControllerDeviceModel);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumDeviceType.Multi:
                    case EnumDeviceType.Fence:
                    case EnumDeviceType.Underground:
                    case EnumDeviceType.Contact:
                    case EnumDeviceType.PIR:
                    case EnumDeviceType.IoController:
                    case EnumDeviceType.Laser:
                        {
                            var viewModel = ViewModelFactory.Build<SensorDeviceViewModel>(item as ISensorDeviceModel);
                            searchedItem = viewModel;
                            return true;
                        }
                    case EnumDeviceType.Cable:
                        break;
                    case EnumDeviceType.IpCamera:
                        break;
                    default:
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(AddProcess)}({ClassName}) : {ex.Message}");
                return false;
            }
        }

        
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        protected DeviceProvider _provider;
        #endregion
    }
}
