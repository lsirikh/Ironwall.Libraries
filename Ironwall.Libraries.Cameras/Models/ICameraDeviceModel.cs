namespace Ironwall.Libraries.Cameras.Models
{
    public interface ICameraDeviceModel : ICameraModel
    {
        public string Name { get; set; }
        public string RtspUri { get; set; }
        public int RtspPort { get; set; }

        public int TypeDevice { get; set; }
        public string Mac { get; set; }
        public int Mode { get; set; }
    }
}