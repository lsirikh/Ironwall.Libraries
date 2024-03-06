﻿using Newtonsoft.Json;
using StackExchange.Redis;

namespace Ironwall.Framework.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/28/2023 3:23:50 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class BaseModel : IBaseModel
    {

        public BaseModel()
        {

        }

        public BaseModel(string id)
        {
            Id = id;
        }

        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }
    }
}
