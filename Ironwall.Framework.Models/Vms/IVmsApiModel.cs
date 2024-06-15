namespace Ironwall.Framework.Models.Vms
{
    public interface IVmsApiModel : IBasicModel
    {
        string ApiAddress { get; set; }
        uint ApiPort { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }
}