namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IAccountIdCheckRequest : IBaseMessageModel
    {
        string IdUser { get; set; }
    }
}