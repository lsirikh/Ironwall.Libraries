using System;

namespace Ironwall.Framework.Models.Accounts
{
    public interface ILoginBaseModel : IAccountBaseModel
    {
        DateTime TimeCreated { get; set; }
        string UserId { get; set; }
    }
}