namespace Ironwall.Framework.Models.Communications.Events
{
    public interface ISearchMalfunctionRequestModel : IUserSessionBaseRequestModel
    {
        string EndDateTime { get; set; }
        string StartDateTime { get; set; }
    }
}