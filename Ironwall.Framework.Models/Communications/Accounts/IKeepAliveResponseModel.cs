using System;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IKeepAliveResponseModel : IResponseModel
    {
        DateTime TimeExpired { get; set; }
    }
}