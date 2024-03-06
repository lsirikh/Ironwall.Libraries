using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Cameras.Providers.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Base.Services;
using System.Threading;

namespace Ironwall.Libraries.Cameras.Providers.ViewModels
{
    public class CameraDeviceViewModelProvider
        : CameraBaseViewModelProvider
    {
        
        #region - Ctors -
        public CameraDeviceViewModelProvider(CameraDeviceProvider provider)
        {
            ClassName = nameof(CameraDeviceViewModelProvider);
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
                    foreach (ICameraDeviceModel item in _provider.ToList())
                    {
                        Add(new CameraDeviceViewModel(item));
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
                    Add(new CameraDeviceViewModel(item as ICameraDeviceModel));
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
                        searchedItem = new CameraDeviceViewModel(item as ICameraDeviceModel);
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
        //public override event RefreshItems Refresh;
        //public override event Insert Inserted;
        //public override event Update Updated;
        //public override event Delete Deleted;
        private CameraDeviceProvider _provider;
        #endregion
    }
}
