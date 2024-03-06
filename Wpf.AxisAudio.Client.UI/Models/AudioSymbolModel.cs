using Ironwall.Framework.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/8/2023 10:58:19 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSymbolModel : IEntityModel
    {

        #region - Ctors -
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
        public int Id { get; set; }
        public string NameArea { get; set; }
        public int TypeDevice { get; set; }
        public string NameDevice { get; set; }
        public int IdController { get; set; }
        public int IdSensor { get; set; }
        public int TypeShape { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Angle { get; set; }
        public int Map { get; set; }
        public bool Used { get; set; }
        public bool Visibility { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
