using System;

namespace Ironwall.Framework.ViewModels.Account
{
    public interface ILoginSessionViewModel : ILoginBaseViewModel
    {
        DateTime TimeExpired { get; set; }
        string Token { get; set; }
        string UserPass { get; set; }
    }
}