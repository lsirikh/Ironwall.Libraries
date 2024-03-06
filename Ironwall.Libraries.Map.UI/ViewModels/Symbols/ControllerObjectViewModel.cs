using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 1:31:06 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ControllerObjectViewModel : ObjectShapeViewModel
    {
        #region - Ctors -
        public ControllerObjectViewModel()
        {
            TypeShape = (int)EnumShapeType.CONTROLLER;
            TypeDevice = (int)EnumDeviceType.Controller;
            Width = 60d;
            Height = 60d;
        }
        public ControllerObjectViewModel(IObjectShapeModel model) 
            : base(model)
        {
            //TypeShape = (int)EnumShapeType.CONTROLLER;
            //TypeDevice = (int)EnumDeviceType.Controller;
            //Width = 60d;
            //Height = 60d;

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
