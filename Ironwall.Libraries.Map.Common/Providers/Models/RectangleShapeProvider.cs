﻿using Ironwall.Framework.Models.Maps.Symbols;
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
        Created On   : 4/24/2023 4:31:07 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class RectangleShapeProvider : SymbolCommonProvider
    {

        #region - Ctors -
        public RectangleShapeProvider(SymbolProvider provider) :base(provider)
        {
            ClassName = nameof(RectangleShapeProvider);
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
                                        .OfType<IShapeSymbolModel>() // 타입 필터링
                                        .Where(entity => entity.TypeShape == (int)EnumShapeType.RECTANGLE)
                                        .ToList())
                    {
                        isValid = true;
                        Add(item);
                    }
                    _log.Info($"{nameof(SymbolModel)}s of {nameof(EnumShapeType.RECTANGLE)} were inserted to {nameof(RectangleShapeProvider)}");
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
                    if (item.TypeShape == (int)EnumShapeType.RECTANGLE)
                    {
                        Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                        Add(item);
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
