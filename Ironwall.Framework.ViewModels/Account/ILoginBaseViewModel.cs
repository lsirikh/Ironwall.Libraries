using System;

namespace Ironwall.Framework.ViewModels.Account
{
    public interface ILoginBaseViewModel : IAccountBaseViewModel
    {
        DateTime TimeCreated { get; set; }
        string UserId { get; set; }
    }
}