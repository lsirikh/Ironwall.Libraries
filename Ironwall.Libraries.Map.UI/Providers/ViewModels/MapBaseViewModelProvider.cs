using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.Events;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Map.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    public abstract class MapBaseViewModelProvider
        : BaseCommonProvider<IMapViewModel> , ILoadable
    {
        #region - Ctors -
        public MapBaseViewModelProvider()
        {
            ClassName = nameof(MapBaseViewModelProvider);
        }
        #endregion
        #region - Implementation of Interface -
        public abstract Task<bool> Initialize(CancellationToken token = default);
        public abstract void Uninitialize();
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        public override async Task<bool> Finished()
        {
            try
            {
                if (Refresh == null)
                    return false;

                bool ret = await Refresh.Invoke();
                return ret;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Finished)}({ClassName}) : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(IMapViewModel item)
        {
            try
            {
                Add(item);

                if (Inserted == null)
                    return false;

                bool ret = await Inserted.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)}({ClassName}) : ", ex.Message);
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(IMapViewModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({ClassName}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(IMapViewModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({ClassName}) : ", ex.Message);
                return false;
            }
            return true;
        }

        
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
