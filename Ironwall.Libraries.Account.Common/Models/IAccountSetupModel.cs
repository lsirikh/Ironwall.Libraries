namespace Ironwall.Libraries.Account.Common.Models
{
    public interface IAccountSetupModel
    {
        string NameDatabase { get; }
        string PathDatabase { get; }
        string TableLogin { get; }
        string TableSession { get; }
        string TableUser { get; }
    }
}