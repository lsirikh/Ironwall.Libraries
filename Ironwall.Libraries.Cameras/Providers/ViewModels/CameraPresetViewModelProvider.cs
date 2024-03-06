using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.Providers.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Providers.ViewModels
{
    public class CameraPresetViewModelProvider
        : CameraBaseViewModelProvider
    {
        #region - Ctors -
        public CameraPresetViewModelProvider(CameraPresetProvider provider)
        {
            ClassName = nameof(CameraPresetViewModelProvider);
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

                    foreach (ICameraPresetModel item in _provider
                    .ToList())
                    {
                        Add(new CameraPresetViewModel(item));
                    }
                    await Finished();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;
            });
        }
        private Task<bool> Provider_Insert(Cameras.Models.ICameraBaseModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    Add(new CameraPresetViewModel(item as ICameraPresetModel));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }
        private Task<bool> Provider_Update(Cameras.Models.ICameraBaseModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = new CameraPresetViewModel(item as ICameraPresetModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }
        private Task<bool> Provider_Delete(Cameras.Models.ICameraBaseModel item)
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
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
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
        private CameraPresetProvider _provider;
        #endregion
    }
}
