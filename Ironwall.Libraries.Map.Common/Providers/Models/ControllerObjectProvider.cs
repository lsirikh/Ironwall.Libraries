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
        Created On   : 4/21/2023 1:49:17 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class ControllerObjectProvider : SymbolCommonProvider
    {

        #region - Ctors -
        public ControllerObjectProvider(SymbolProvider provider) : base(provider)
        {
            ClassName = nameof(ControllerObjectProvider);
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
        private Task<bool> Provider_Initialize()
        {
            return Task.Run(async () =>
            {
                var isValid = false;
                try
                {
                    Clear();
                    foreach (SymbolModel item in _provider
                    .Where(entity => entity.TypeShape == (int)EnumShapeType.CONTROLLER)
                    .ToList())
                    {
                        isValid = true;
                        Add(item as IObjectShapeModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

                if (isValid)
                    return await Finished();
                else
                    return false;
            });
        }

        private Task<bool> Proivder_Insert(ISymbolModel item)
        {
            bool ret = false;
            return Task.Run(() =>
            {
                try
                {
                    if (item.TypeShape == (int)EnumShapeType.CONTROLLER)
                    {
                        Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                        Add(item as IObjectShapeModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Proivder_Insert)}({ClassName}) : {ex.Message}");
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
        #endregion
    }
}
