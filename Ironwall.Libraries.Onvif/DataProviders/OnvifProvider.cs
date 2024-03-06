using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.Providers.Models;
using Ironwall.Libraries.Onvif.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.DataProviders
{
    public class OnvifProvider : OnvifBaseProvider
    {
        #region - Ctors -
        public OnvifProvider(CameraDeviceProvider provider)
        {
            ClassName = nameof(OnvifProvider);
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

                    foreach (ICameraDeviceModel item in _provider
                    .ToList())
                    {
                        Add(new OnvifModel(item));
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
                    Add(new OnvifModel(item as ICameraDeviceModel));
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
                    var searchedItem = CollectionEntity.Where(t => t == item).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = new OnvifModel(item as ICameraDeviceModel);
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
                    var searchedItem = CollectionEntity.Where(t => t == item).FirstOrDefault();
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
        private CameraDeviceProvider _provider;
        #endregion
    }
}
