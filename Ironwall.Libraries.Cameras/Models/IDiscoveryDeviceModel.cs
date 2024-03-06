namespace Ironwall.Libraries.Cameras.Models
{
    public interface IDiscoveryDeviceModel
    {
        ///EndpointDiscoveryMetadata->ListenUris[0]
        ///Host
        ///Port
        ///Id
        ///Password

        ///EndpointDiscoveryMetadata->Scopes[i]->LocalPath
        ///Profile
        ///type
        ///MAC
        ///hardware : Model
        ///name : Brand
        ///location

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


    }
}