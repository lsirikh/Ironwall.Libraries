using Ironwall.Framework.Models.Devices;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Ironwall.Framework.Models.Mappers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/3/2023 2:56:36 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MappingTableMapper : IMappingTableMapper
    {

        #region - Ctors -
        public MappingTableMapper()
        {

        }

        public MappingTableMapper(ICameraMappingModel model)
        {
            MapperId = model.Id;
            GroupId = model.Group;
            Sensor = model.Sensor.Id;
            FirstPreset = model?.FirstPreset?.Id;
            SecondPreset = model?.SecondPreset?.Id;
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
        public string MapperId { get; set; }
        public string GroupId { get; set; }
        public string Sensor { get; set; }
        public string FirstPreset { get; set; }
        public string SecondPreset { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
