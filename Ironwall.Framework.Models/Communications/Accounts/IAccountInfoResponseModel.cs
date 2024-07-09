namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IAccountInfoResponseModel : IResponseModel
    {
        AccountDetailModel Details { get; set; }
    }
}