using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Map.UI.Models.Messages;
using System.Windows;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/26/2023 8:33:03 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class FixedCameraObjectViewModel : ObjectShapeViewModel
    {
        #region - Ctors -
        public FixedCameraObjectViewModel()
        {
            TypeShape = (int)EnumShapeType.FIXED_CAMERA;
            TypeDevice = (int)EnumDeviceType.IpCamera;
            Width = 60d;
            Height = 60d;
        }
        public FixedCameraObjectViewModel(IObjectShapeModel model)
            : base(model)
        {
            //TypeShape = (int)EnumShapeType.FIXED_CAMERA;
            //TypeDevice = (int)EnumDeviceType.IpCamera;
            //Width = 60d;
            //Height = 60d;
        }
        #endregion
        #region - Implementation of Interface -
        public async void OnClickStreaming(object sender, RoutedEventArgs args)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new RequestCameraStreaming(NameDevice));
        }
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
