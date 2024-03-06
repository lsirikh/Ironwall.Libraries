using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Models
{
    public class DiscoveryDeviceModel : IDiscoveryDeviceModel
    {
        #region - Ctors -
        public DiscoveryDeviceModel()
        {

        }
        public DiscoveryDeviceModel(string ipAddress, int port, string userName, 
            string password, string profiles, string types, string mac, string deviceModel, string company, string location)
        {
            IpAddress = ipAddress;
            Port = port;
            UserName = userName;
            Password = password;
            Profiles = profiles;
            Types = types;
            Mac = mac;
            DeviceModel = deviceModel;
            Company = company;
            Location = location;

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
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Profiles { get; set; }
        public string Types { get; set; }
        public string Mac { get; set; }
        public string DeviceModel { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        #endregion
        #region - Attributes -
        #endregion

    }
}
