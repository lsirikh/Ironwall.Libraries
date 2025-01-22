using System;

namespace Ironwall.Framework.Models.Ais
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 1/14/2025 7:10:19 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class NetworkSettingModel : BaseModel, INetworkSettingModel
    {
        #region - Ctors -
        public NetworkSettingModel()
        {

        }

        public NetworkSettingModel(int id, bool isAvailable, string name, string ipAddress, int port) : base(id)
        {
            IsAvailable = isAvailable;
            Name = name;
            IpAddress = ipAddress;
            Port = port;
        }

        public NetworkSettingModel(INetworkSettingModel model): base(model) 
        {
            IsAvailable = model.IsAvailable;
            Name = model.Name;
            IpAddress = model.IpAddress;
            Port = model.Port;
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
        public bool IsAvailable { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}