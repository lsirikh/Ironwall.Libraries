namespace Ironwall.Libraries.RTSP.Models
{
    public interface ICameraDeviceModel
    {
        public string Name { get; set; }
        public string RtspUri { get; set; }
        public int RtspPort { get; set; }

        public int TypeDevice { get; set; }
        public string Mac { get; set; }
        public int Mode { get; set; }
    }
}