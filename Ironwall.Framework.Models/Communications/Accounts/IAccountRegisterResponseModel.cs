namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IAccountRegisterResponseModel : IResponseModel
    {
        AccountDetailModel Details { get; set; }
    }
}