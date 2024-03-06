namespace Ironwall.Libraries.Onvif.Models
{
    public interface IMediaUrlModel
    {
        object Any { get; set; }
        bool InvalidAfterConnect { get; set; }
        bool InvalidAfterReboot { get; set; }
        string Timeout { get; set; }
        string Uri { get; set; }
    }
}