namespace Ironwall.Libraries.Onvif.Models
{
    public interface IHostInfoModel
    {
        object Extension { get; set; }
        bool FromDHCP { get; set; }
        string Name { get; set; }
    }
}