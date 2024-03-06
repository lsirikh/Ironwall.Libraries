using Ironwall.Framework.Models;
using System.Collections.Generic;

namespace Wpf.Libraries.Surv.Common.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/1/2023 5:23:37 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvMappingModel : ISurvMappingModel
    {

        #region - Ctors -
        public SurvMappingModel()
        {

        }

        public SurvMappingModel(int id, int groupNumber, string groupName, int eventId)
        {
            Id = id;
            GroupNumber = groupNumber;
            GroupName = groupName;
            EventId = eventId;
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
        public int Id { get; set; }
        public int GroupNumber { get; set; }
        public string GroupName { get; set; }
        public int EventId { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
