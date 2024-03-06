namespace Ironwall.Libraries.Sounds.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/10/2023 11:01:52 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoundModel : ISoundModel
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
        public string Name { get; set; }
        public string File { get; set; }

        public string Category { get; set; }
        public bool IsPlaying { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
