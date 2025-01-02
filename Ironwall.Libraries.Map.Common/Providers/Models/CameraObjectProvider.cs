using Ironwall.Framework.Models.Devices;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using System.Linq;
using Ironwall.Libraries.Enums;
using static Dapper.SqlMapper;
using Ironwall.Framework.Models.Maps.Symbols;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 10:27:50 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class CameraObjectProvider : SymbolCommonProvider
    {

        #region - Ctors -
        public CameraObjectProvider(SymbolProvider provider) : base(provider)
        {
            ClassName = nameof(CameraObjectProvider);
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
                    foreach (var item in _provider
                                        .OfType<IObjectShapeModel>() // 타입 필터링
                                        .Where(entity => entity.TypeShape == (int)EnumShapeType.IP_CAMERA
                                        || entity.TypeShape == (int)EnumShapeType.FIXED_CAMERA
                                        || entity.TypeShape == (int)EnumShapeType.PTZ_CAMERA
                                        || entity.TypeShape == (int)EnumShapeType.SPEEDDOM_CAMERA)
                    .ToList())
                    {
                        isValid = true;
                        Add(item);
                    }
                    _log.Info($"{nameof(SymbolModel)}s of ({nameof(EnumShapeType.IP_CAMERA)}, {nameof(EnumShapeType.FIXED_CAMERA)}, {nameof(EnumShapeType.PTZ_CAMERA)}, {nameof(EnumShapeType.SPEEDDOM_CAMERA)})  were inserted to {nameof(CameraObjectProvider)}");
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
                    if (item.TypeShape == (int)EnumShapeType.IP_CAMERA
                    || item.TypeShape == (int)EnumShapeType.FIXED_CAMERA
                    || item.TypeShape == (int)EnumShapeType.PTZ_CAMERA
                    || item.TypeShape == (int)EnumShapeType.SPEEDDOM_CAMERA)
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
