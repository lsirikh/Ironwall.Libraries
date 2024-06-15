namespace Ironwall.Framework.Models.Accounts
{
    public interface ILoginSessionModel : ILoginBaseModel
    {
        //string TimeCreated { get; set; }
        string TimeExpired { get; set; }
        string Token { get; set; }
        //string UserId { get; set; }
        string UserPass { get; set; }
    }
}