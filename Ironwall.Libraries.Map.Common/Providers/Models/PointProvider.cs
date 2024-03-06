using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using System;
using System.Linq;

namespace Ironwall.Libraries.Map.Common.Providers.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/26/2023 10:09:50 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PointProvider : EntityCollectionProvider<IPointClass>
    {

        #region - Ctors -
        public PointProvider()
        {

        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void RemoveAll(Func<IPointClass, bool> value)
        {
            foreach (var item in CollectionEntity.Where(value).ToList())
            {
                CollectionEntity.Remove(item);
            }
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
