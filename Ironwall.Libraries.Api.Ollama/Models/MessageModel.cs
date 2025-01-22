using System;

namespace Ironwall.Libraries.Api.Ollama.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 1/16/2025 10:17:43 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class MessageModel
    {
        #region - Ctors -
        public MessageModel()
        {
            
        }
        public MessageModel(string user, string content)
        {
            User = user;
            Content = content;
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
        public string User { get; set; }
        public string Content { get; set; } 
        #endregion
        #region - Attributes -
        #endregion
    }
}