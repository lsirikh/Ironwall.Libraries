using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/26/2024 3:44:17 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public interface IBaseEventViewModel<T> : IBaseCustomViewModel<T> where T : IBaseEventModel
    {
        DateTime DateTime { get; set; }
    }
}