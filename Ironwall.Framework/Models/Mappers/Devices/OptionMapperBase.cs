using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Mappers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/28/2023 3:39:28 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class OptionMapperBase : IOptionMapperBase
    {

        #region - Ctors -
        public OptionMapperBase()
        {

        }

        public OptionMapperBase(IOptionBaseModel model)
        {
            OptionId = model.Id;
            ReferenceId = model.ReferenceId;
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
        public string OptionId { get; set; }
        public string ReferenceId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
