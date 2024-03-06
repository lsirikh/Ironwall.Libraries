using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/24/2023 4:22:10 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class RectangleShapeViewModel : ShapeSymbolViewModel
    {

        #region - Ctors -
        public RectangleShapeViewModel()
        {
            TypeShape = (int)EnumShapeType.RECTANGLE;
            Width = 60d;
            Height = 60d;
        }
        public RectangleShapeViewModel(IShapeSymbolModel model)
            : base(model)
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
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
