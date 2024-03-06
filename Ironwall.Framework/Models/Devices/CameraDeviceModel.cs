using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Devices
{
    public class CameraDeviceModel
        : BaseDeviceModel, ICameraDeviceModel
    {
        public CameraDeviceModel()
        {
            Profiles = new List<CameraProfileModel>();
            Presets = new List<CameraPresetModel>();
            DeviceType = (int)EnumDeviceType.IpCamera;
        }
        public CameraDeviceModel(string id) : this()
        {
            Id = id;
        }

        public CameraDeviceModel(ICameraTableMapper model, List<CameraPresetModel> presets, List<CameraProfileModel> profiles)
            : base(model)
        {
            IpAddress = model.IpAddress;
            Port = model.Port;
            UserName = model.UserName;
            Password = model.Password;
            Presets = presets;
            Profiles = profiles;
            Category = model.Category;
            DeviceModel = model.DeviceModel;
            RtspUri = model.RtspUri;
            RtspPort = model.RtspPort;
            Mode = model.Mode;
        }


        [JsonProperty("ip_address", Order = 6)]
        public string IpAddress { get; set; }

        [JsonProperty("ip_port", Order = 7)]
        public int Port { get; set; }
        
        [JsonProperty("category", Order = 8)]
        public int Category { get; set; }

        [JsonProperty("user_name", Order = 9)]
        public string UserName { get; set; }

        [JsonProperty("user_pass", Order = 10)]
        public string Password { get; set; }

        [JsonProperty("presets", Order = 11)]
        public List<CameraPresetModel> Presets { get; set; }

        [JsonProperty("profiles", Order = 12)]
        public List<CameraProfileModel> Profiles { get; set; }

        [JsonProperty("device_model", Order = 13)]
        public string DeviceModel { get; set; }

        [JsonProperty("rtsp_uri", Order = 14)]
        public string RtspUri { get; set; }

        [JsonProperty("rtsp_port", Order = 15)]
        public int RtspPort { get; set; }

        [JsonProperty("mode", Order = 16)]
        public int Mode { get; set; }

    }
}
