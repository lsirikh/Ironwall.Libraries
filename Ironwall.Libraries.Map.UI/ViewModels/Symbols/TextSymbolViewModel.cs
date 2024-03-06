using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/28/2023 4:49:02 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class TextSymbolViewModel : SymbolViewModel
    {

        #region - Ctors -
        public TextSymbolViewModel()
        {
            TypeShape = (int)EnumShapeType.TEXT;
            Width = 60d;
            Height = 60d;
            IsShowLable = true;
        }

        public TextSymbolViewModel(ISymbolModel model)
            : base(model)
        {
            //TypeShape = (int)EnumShapeType.TEXT;
            //Width = 60d;
            //Height = 60d;
            //IsShowLable = true;
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
