﻿using Newtonsoft.Json;


namespace Ironwall.Framework.Models.Devices
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/12/2023 10:48:48 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OptionBaseModel : BaseModel,  IOptionBaseModel
    {

        #region - Ctors -
        public OptionBaseModel()
        {

        }

        public OptionBaseModel(int id, int referId) : base(id)
        {
            ReferenceId = referId;
        }

        public OptionBaseModel(IOptionBaseModel model) : base(model)
        {
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
        [JsonProperty("reference_id", Order = 2)]
        public int ReferenceId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
