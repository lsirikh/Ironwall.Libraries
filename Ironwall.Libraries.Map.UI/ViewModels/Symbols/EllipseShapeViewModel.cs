using Ironwall.Framework.Services;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using Ironwall.Libraries.Enums;
using System.Threading.Tasks;
using Ironwall.Framework.Models.Maps.Symbols;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/25/2023 3:54:12 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class EllipseShapeViewModel : ShapeSymbolViewModel
    {

        #region - Ctors -
        public EllipseShapeViewModel()
        {
            TypeShape = (int)EnumShapeType.ELLIPSE;
            Width = 60d;
            Height = 60d;
        }
        public EllipseShapeViewModel(IShapeSymbolModel model)
            : base(model)
        {
            
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        
        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
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
