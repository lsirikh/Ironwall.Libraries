﻿namespace Ironwall.Framework.Models.Communications.Accounts
{
    public interface IKeepAliveRequestModel : IBaseMessageModel
    {
        string Token { get; set; }
    }
}