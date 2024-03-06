namespace Ironwall.Framework.Models.Accounts
{
    public interface IUserBaseModel : IAccountBaseModel
    {
        string IdUser { get; set; }
        string Name { get; set; }
        string Password { get; set; }
        int Level { get; set; }
        bool Used { get; set; }
    }
}