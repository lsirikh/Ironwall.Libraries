using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Map.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    public abstract class MapBaseProvider
        : BaseCommonProvider<IMapModel>
    {
        #region - Ctors -
        public MapBaseProvider()
        {
            ClassName = nameof(MapBaseProvider);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override async Task<bool> Finished()
        {
            try
            {
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Finished)}({ClassName}) was executed!!!");
                if (Refresh == null)
                    return false;

                bool ret = await Refresh?.Invoke();
                return ret;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Finished)} : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(IMapModel item)
        {
            try
            {
                Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                Add(item);

                if (Inserted == null)
                    return false;

                bool ret = await Inserted?.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)} : ", ex.Message);
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(IMapModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated?.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({nameof(IMapModel)}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(IMapModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted?.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({nameof(IMapModel)}) : ", ex.Message);
                return false;
            }
            return true;
        }

        public Task<bool> ClearData()
        {
            return Task.Run(async () =>
            {
                try
                {
                    Clear();
                    await Finished();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            });
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        public override event RefreshItems Refresh;
        public override event Insert Inserted;
        public override event Update Updated;
        public override event Delete Deleted;
        #endregion
    }
}
