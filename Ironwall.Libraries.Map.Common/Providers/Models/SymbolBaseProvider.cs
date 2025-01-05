using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 10:06:42 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class SymbolBaseProvider : BaseCommonProvider<ISymbolModel>
    {

        #region - Ctors -
        public SymbolBaseProvider()
        {
            ClassName = nameof(SymbolBaseProvider);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override async Task<bool> Finished()
        {
            try
            {
                _log.Info($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Finished)}({ClassName}) was executed!!!");

                if (Refresh == null) return false;

                bool ret = await Refresh?.Invoke();
                return ret;

            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(DeletedItem)} in {nameof(Finished)} : {ex.Message}");
                return false;
            }
        }

        public override async Task<bool> InsertedItem(ISymbolModel item)
        {
            try
            {
                Add(item);
                _log.Info($"[{item.Id}]nameof(InsertedItem)}}({{ClassName}}) was executed({CollectionEntity.Count()})!!!");

                if (Inserted == null) return false;

                bool ret = await Inserted?.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {

                _log.Error($"Raised {nameof(DeletedItem)} in {nameof(InsertedItem)} : {ex.Message}");
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(ISymbolModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null) return false;

                bool ret = await Updated?.Invoke(item);
            }
            catch (Exception ex)
            {

                _log.Error($"Raised {nameof(DeletedItem)} in {nameof(UpdatedItem)}({nameof(ISymbolModel)})  : {ex.Message}");
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(ISymbolModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null) Remove(searchedItem);

                if (Deleted == null) return false;

                bool ret = await Deleted?.Invoke(item);
            }
            catch (Exception ex)
            {

                _log.Error($"Raised {nameof(DeletedItem)} in {nameof(DeletedItem)}({nameof(ISymbolModel)})  : {ex.Message}");
                return false;
            }
            return true;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
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
