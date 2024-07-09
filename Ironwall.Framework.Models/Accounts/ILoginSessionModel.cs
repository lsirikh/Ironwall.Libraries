using System;

namespace Ironwall.Framework.Models.Accounts
{
    public interface ILoginSessionModel : ILoginBaseModel
    {
        DateTime TimeExpired { get; set; }
        string Token { get; set; }
        string UserPass { get; set; }
    }
}