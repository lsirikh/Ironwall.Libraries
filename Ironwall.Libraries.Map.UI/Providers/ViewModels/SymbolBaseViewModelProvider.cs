using Ironwall.Framework.DataProviders;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ironwall.Framework.Services;
using System.Threading;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 10:55:53 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class SymbolBaseViewModelProvider<T>
        : BaseCommonProvider<T>, ILoadable where T :ISymbolViewModel
    {

        #region - Ctors -
        public SymbolBaseViewModelProvider()
        {
            ClassName = nameof(SymbolBaseViewModelProvider<T>);
        }
        #endregion
        #region - Implementation of Interface -
        public abstract Task<bool> Initialize(CancellationToken token = default);
        public abstract void Uninitialize();
        #endregion
        #region - Overrides -
        public override async Task<bool> Finished()
        {
            try
            {
                if (Refresh == null)
                    return false;

                bool ret = await Refresh?.Invoke();
                return ret;

            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Finished)}({ClassName}) : {ex.Message}");
                return false;
            }
        }

        public override async Task<bool> InsertedItem(T item)
        {
            try
            {
                Add(item);

                if (Inserted == null)
                    return false;

                bool ret = await Inserted?.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(InsertedItem)}({ClassName}) : {ex.Message}");
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(T item)
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

                _log.Error($"Raised Exception in {nameof(UpdatedItem)}({ClassName}) : {ex.Message}");
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(T item)
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

                _log.Error($"Raised Exception in {nameof(DeletedItem)}({ClassName}) : {ex.Message}");
                return false;
            }
            return true;
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
