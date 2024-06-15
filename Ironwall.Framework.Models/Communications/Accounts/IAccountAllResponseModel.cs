using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IAccountAllResponseModel : IResponseModel
    {
        List<AccountDetailModel> Details { get; set; }
    }
}