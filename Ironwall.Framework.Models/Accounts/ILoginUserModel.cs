namespace Ironwall.Framework.Models.Accounts
{
    public interface ILoginUserModel : ILoginBaseModel
    {
        int ClientId { get; set; }
        int Mode { get; set; }
        //string TimeCreated { get; set; }
        //string UserId { get; set; }
        int UserLevel { get; set; }

        string ToString();
    }
}