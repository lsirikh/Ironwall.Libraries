using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public sealed class DetectionEventProvider
        : EventBaseProvider
    {

        public DetectionEventProvider(
            EventProvider metaEventProvider)
        {
            ClassName = nameof(DetectionEventProvider);
            _eventProvider = metaEventProvider;
            _eventProvider.Refresh += EventProvider_Initialize;
            _eventProvider.Updated += EventProvider_Update;
            _eventProvider.Inserted += EventProvider_Insert;
            _eventProvider.Deleted += EventProvider_Delete;
        }

        private Task<bool> EventProvider_Initialize()
        {
            return Task.Run(async () =>
            {
                try
                {
                    Clear();

                    foreach (DetectionEventModel item in _eventProvider
                    .Where(entity => entity.MessageType == (int)EnumEventType.Intrusion)
                    .ToList())
                    {
                        Debug.WriteLine($"DetectionEventModel == >{item.Id}");
                        Add(item);
                    }
                    await Finished();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Initialize)}({nameof(DetectionEventProvider)} : {ex.Message}");
                    return false;
                }

                return true;
            });
        }
        private Task<bool> EventProvider_Insert(IMetaEventModel item)
        {
            bool ret = false;
            return Task.Run(async() =>
            {
                try
                {
                    if (item.MessageType == (int)EnumEventType.Intrusion)
                    {
                        Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                        ret = await InsertedItem(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Insert)}({nameof(DetectionEventProvider)} : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }
        private Task<bool> EventProvider_Update(IMetaEventModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        searchedItem = item;
                        ret = await UpdatedItem(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Update)}({nameof(DetectionEventProvider)} : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }

        private Task<bool> EventProvider_Delete(IMetaEventModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        //Remove(searchedItem);
                        ret = await DeletedItem(item);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Update)}({nameof(DetectionEventProvider)} : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }

        private EventProvider _eventProvider;
    }
    
}
