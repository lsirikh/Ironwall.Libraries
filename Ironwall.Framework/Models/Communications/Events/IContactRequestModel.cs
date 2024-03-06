namespace Ironwall.Framework.Models.Communications.Events
{
    public interface IContactRequestModel : IBaseEventMessageModel
    {
        ContactDetailModel Detail { get; set; }
    }
}