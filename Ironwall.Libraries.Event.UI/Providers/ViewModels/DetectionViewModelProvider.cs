using Caliburn.Micro;
using Ironwall.Framework.Models.Events;

using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Providers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public sealed class DetectionViewModelProvider
        : EventBaseViewModelProvider
    {
        #region - Ctors -
        public DetectionViewModelProvider(EventProvider eventProvider)
        {
            ClassName = nameof(DetectionViewModelProvider);
            _provider = eventProvider;
        }
        #endregion
        #region - Implementation of Interface -
        public override Task<bool> Initialize(CancellationToken token = default)
        {

            Debug.WriteLine($"{nameof(DetectionViewModelProvider)} is {nameof(Initialize)}");
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
        public Task<bool> Provider_Initialize()
        {
            return Task.Run(() =>
            {
                try
                {
                    Clear();
                    var list = _provider.OfType<IDetectionEventModel>().OrderByDescending(item => item.DateTime).ToList();

                    foreach (var item in list)
                    {
                        var viewModel = ViewModelFactory.Build<DetectionEventViewModel>(item);
                        viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({nameof(DetectionViewModelProvider)}) : {ex.ToString()}");
                    return false;
                }

                return true;
            });
        }
        private Task<bool> Provider_Insert(IMetaEventModel item)
        {
            if ((EnumEventType)item.MessageType != EnumEventType.Intrusion) return Task.FromResult(false);
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<DetectionEventViewModel>(item);
                    Add(viewModel, 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({nameof(DetectionViewModelProvider)}) : {ex.Message}");
                    return false;
                }
                
                return true;

            });
        }
        private Task<bool> Provider_Update(IMetaEventModel item)
        {
            if ((EnumEventType)item.MessageType != EnumEventType.Intrusion) return Task.FromResult(false);
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<DetectionEventViewModel>(item);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = viewModel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({nameof(DetectionViewModelProvider)}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }
        private Task<bool> Provider_Delete(IMetaEventModel item)
        {
            if ((EnumEventType)item.MessageType != EnumEventType.Intrusion) return Task.FromResult(false);
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
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({nameof(DetectionViewModelProvider)}) : {ex.Message}");
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
        private EventProvider _provider;
        #endregion
    }
}
