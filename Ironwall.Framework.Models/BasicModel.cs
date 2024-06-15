using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 4:20:33 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class BasicModel : IBasicModel
    {

        public BasicModel()
        {

        }

        public BasicModel(int id)
        {
            Id = id;
        }

        [JsonProperty("id", Order = 1)]
        public int Id { get; set; }
    }
}