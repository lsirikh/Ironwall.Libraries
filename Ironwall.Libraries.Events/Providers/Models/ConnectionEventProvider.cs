using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    public sealed class ConnectionEventProvider
    : EventBaseProvider
    {
        #region - ctors -
        public ConnectionEventProvider(
            EventProvider metaEventProvider)
        {
            ClassName = nameof(ConnectionEventProvider);
            _eventProvider = metaEventProvider;
            _eventProvider.Refresh += EventProvider_Initialize;
            _eventProvider.Updated += EventProvider_Update;
            _eventProvider.Inserted += EventProvider_Insert;
            _eventProvider.Deleted += EventProvider_Delete;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task<bool> EventProvider_Initialize()
        {
            return Task.Run(async () =>
            {
                try
                {
                    Clear();

                    foreach (MalfunctionEventModel item in _eventProvider
                    .Where(entity => entity.MessageType == EnumEventType.Connection)
                    .ToList())
                    {
                        Add(item);
                    }

                    await Finished();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Initialize)}({nameof(ConnectionEventProvider)} : {ex.Message}");
                    return false;
                }
                return true;
            });
        }
        private Task<bool> EventProvider_Insert(IMetaEventModel item)
        {
            bool ret = false;
            return Task.Run(async () =>
            {
                try
                {
                    if (item.MessageType == EnumEventType.Connection)
                        ret = await InsertedItem(item);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Insert)}({nameof(ConnectionEventProvider)} : {ex.Message}");
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
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Update)}({nameof(ConnectionEventProvider)} : {ex.Message}");
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
                        ret = await DeletedItem(item);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(EventProvider_Update)}({nameof(ConnectionEventProvider)} : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }
        #endregion

        private EventProvider _eventProvider;
    }
}
