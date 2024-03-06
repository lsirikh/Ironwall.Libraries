using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Ironwall.Libraries.Cameras.Models
{
    public class CameraDeviceModel 
        : CameraBaseModel
        , ICameraDeviceModel
    {
        public CameraDeviceModel()
        {

        }

        public CameraDeviceModel(ICameraModel model)
        {
            Id = model.Id;
            HostName = model.HostName;
            IpAddress = model.IpAddress;
            Port = model.Port;
            UserName = model.UserName;
            Password = model.Password;
            FirmwareVersion = model.FirmwareVersion;
            HardwareId = model.HardwareId;
            Manufacturer = model.Manufacturer;
            DeviceModel = model.DeviceModel;
            SerialNumber = model.SerialNumber;
            Profile = model.Profile;
            Uri = model.Uri;
            Type = model.Type;
        }

        #region - Implementation of ICameraModel - 
        //public int Id { get; set; }
        public string HostName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirmwareVersion { get; set; }
        public string HardwareId { get; set; }
        public string Manufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string SerialNumber { get; set; }

        public int Profile { get; set; }
        public string Uri { get; set; }
        public string Type { get; set; }
        #endregion

        #region - Implementation of ICameraDeviceModel - 
        public string Name { get; set; }
        public string RtspUri { get; set; }
        public int RtspPort { get; set; }
        public int TypeDevice { get; set; }
        public string Mac { get; set; }
        public int Mode { get; set; }
        #endregion
        


        public ICameraModel ToCameraModel()
        {
            var model = new CameraModel()
            {
                Id = Id,
                HostName = HostName,
                IpAddress = IpAddress,
                Port = Port,
                UserName = UserName,
                Password = Password,
                FirmwareVersion = FirmwareVersion,
                HardwareId = HardwareId,
                Manufacturer = Manufacturer,
                DeviceModel = DeviceModel,
                SerialNumber = SerialNumber,
                Profile = Profile,
                Uri = Uri,
                Type = Type,
            };
            return model;
        }

        public class CameraModel : ICameraModel
        {
            #region - Implementation of ICameraModel - 
            public int Id { get; set; }
            public string HostName { get; set; }
            public string IpAddress { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }

            public string FirmwareVersion { get; set; }
            public string HardwareId { get; set; }
            public string Manufacturer { get; set; }
            public string DeviceModel { get; set; }
            public string SerialNumber { get; set; }

            public int Profile { get; set; }
            public string Uri { get; set; }
            public string Type { get; set; }
            #endregion
        }
    }
}
