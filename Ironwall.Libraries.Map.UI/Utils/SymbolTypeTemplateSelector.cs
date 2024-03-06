using System.Windows.Controls;
using System.Windows;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Map.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/28/2023 9:17:51 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolTypeTemplateSelector : DataTemplateSelector
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ISymbolViewModel symbol))
                return null;

            switch ((EnumShapeType)symbol.TypeShape)
            {
                case EnumShapeType.NONE:
                    return null;
                case EnumShapeType.TEXT:
                    return SymbolTemplate;
                case EnumShapeType.LINE:
                case EnumShapeType.TRIANGLE:
                case EnumShapeType.RECTANGLE:
                case EnumShapeType.POLYGON:
                case EnumShapeType.ELLIPSE:
                case EnumShapeType.POLYLINE:
                    return ShapeTemplate;
                case EnumShapeType.FENCE:
                case EnumShapeType.CONTROLLER:
                case EnumShapeType.MULTI_SNESOR:
                case EnumShapeType.FENCE_SENSOR:
                case EnumShapeType.UNDERGROUND_SENSOR:
                case EnumShapeType.CONTACT_SWITCH:
                case EnumShapeType.PIR_SENSOR:
                case EnumShapeType.IO_CONTROLLER:
                case EnumShapeType.LASER_SENSOR:
                case EnumShapeType.CABLE:
                case EnumShapeType.IP_CAMERA:
                case EnumShapeType.FIXED_CAMERA:
                case EnumShapeType.PTZ_CAMERA:
                case EnumShapeType.SPEEDDOM_CAMERA:
                    return ObjectTemplate;
                default:
                    return null;
            }
           
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public DataTemplate SymbolTemplate { get; set; }
        public DataTemplate ShapeTemplate { get; set; }
        public DataTemplate ObjectTemplate { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
