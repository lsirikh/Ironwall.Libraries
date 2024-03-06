using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Events;

using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Event.UI.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public sealed class ActionViewModelProvider
        : ActionBaseViewModelProvider
    {
        public ActionViewModelProvider(ActionEventProvider eventProvider)
        {
            ClassName = nameof(ActionViewModelProvider);
            _eventProvider = eventProvider;
            
        }

        public override Task<bool> Initialize(CancellationToken token = default)
        {
            _eventProvider.Refresh += Provider_Initialize;
            _eventProvider.Updated += Provider_Update;
            _eventProvider.Inserted += Provider_Insert;
            _eventProvider.Deleted += Provider_Delete;
            return Provider_Initialize();
        }

        public override void Uninitialize()
        {
            _eventProvider.Refresh -= Provider_Initialize;
            _eventProvider.Updated -= Provider_Update;
            _eventProvider.Inserted -= Provider_Insert;
            _eventProvider.Deleted -= Provider_Delete;
            _eventProvider = null;

            Clear();

            GC.Collect();
        }

        public Task<bool> Provider_Initialize()
        {
            return Task.Run(() =>
            {
                try
                {
                    Clear();
                    //Debug.WriteLine($"{nameof(Provider_Initialize)}({nameof(DetectionViewModelProvider)}) was executed!!!");
                    foreach (IActionEventModel item in _eventProvider.OrderByDescending(item => item.DateTime).ToList())
                    {
                        var viewModel = ViewModelFactory.Build<ActionEventViewModel>(item);
                        Add(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;
            });
        }
        private Task<bool> Provider_Insert(IBaseEventModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<ActionEventViewModel>(item as IActionEventModel);
                    Add(viewModel, 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }
       
        private Task<bool> Provider_Update(IBaseEventModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<ActionEventViewModel>(item as IActionEventModel);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = viewModel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return false;
                }
                return true;
            });
        }
        private Task<bool> Provider_Delete(IBaseEventModel item)
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

        

        #region - Overrides -
        #endregion

        private ActionEventProvider _eventProvider;
    }
}
