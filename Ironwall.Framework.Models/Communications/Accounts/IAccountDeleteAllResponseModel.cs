using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IAccountDeleteAllResponseModel : IResponseModel
    {
        List<string> DeletedAccounts { get; set; }
    }
}