﻿namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchActionRequestModel : IUserSessionBaseRequestModel
    {
        string EndDateTime { get; set; }
        string StartDateTime { get; set; }
    }
}