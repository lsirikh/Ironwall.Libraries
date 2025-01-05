using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/26/2023 11:18:04 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class FenceObjectProvider : SymbolCommonProvider
    {

        #region - Ctors -
        public FenceObjectProvider(SymbolProvider provider) : base(provider)
        {
            ClassName = nameof(FenceObjectProvider);
            _provider.Refresh += Provider_Initialize;
            _provider.Inserted += Proivder_Insert;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task<bool> Provider_Initialize()
        {
            var isValid = false;
            try
            {
                Clear();
                foreach (SymbolModel item in _provider
                                    .OfType<IObjectShapeModel>() // 타입 필터링
                                    .Where(entity => entity.TypeShape == (int)EnumShapeType.FENCE)
                                    .ToList())
                {
                    isValid = true;
                    Add(item as IObjectShapeModel);
                }

                _log.Info($"{nameof(SymbolModel)}s of {nameof(EnumShapeType.FENCE)} were inserted to {nameof(FenceObjectProvider)}");
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                return false;
            }

            if (isValid)
                return await Finished();
            else
                return false;
        }

        private Task<bool> Proivder_Insert(ISymbolModel item)
        {
            bool ret = false;
            try
            {
                if (item.TypeShape == (int)EnumShapeType.FENCE)
                {
                    _log.Info($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                    Add(item);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(Proivder_Insert)}({ClassName}) : {ex.Message}");
            }
            return Task.FromResult(ret);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
