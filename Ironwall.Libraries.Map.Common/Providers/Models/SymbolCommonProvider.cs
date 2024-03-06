using System.Threading.Tasks;
using System;
using System.Linq;
using System.Diagnostics;
using Ironwall.Framework.Models.Maps.Symbols;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/25/2023 10:55:27 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class SymbolCommonProvider : SymbolBaseProvider
    {

        #region - Ctors -
        public SymbolCommonProvider(SymbolProvider provider)
        {
            ClassName = nameof(ControllerObjectProvider);
            _provider = provider;
            //_provider.Refresh += Provider_Initialize;
            //_provider.Inserted += Proivder_Insert;
            //_provider.Updated += Provider_Update;
            _provider.Deleted += Proivder_Delete;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        //private Task<bool> Provider_Update(ISymbolModel item)
        //{
        //    bool ret = false;
        //    return Task.Run(async () =>
        //    {
        //        try
        //        {
        //            var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
        //            if (searchedItem != null)
        //            {
        //                searchedItem = item;
        //                ret = await UpdatedItem(item);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
        //            return ret;
        //        }
        //        return ret;
        //    });
        //}

        private Task<bool> Proivder_Delete(ISymbolModel item)
        {
            bool ret = false;
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity?.Where(t => t?.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        Debug.WriteLine($"{nameof(Proivder_Delete)}({ClassName}) was exceuted.");
                        Remove(searchedItem);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Proivder_Delete)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        protected SymbolProvider _provider;
        #endregion
    }
}
