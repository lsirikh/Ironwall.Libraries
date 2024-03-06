namespace Ironwall.Framework.Models.Accounts
{
    public class LoginUserModel 
        : LoginBaseModel
        , ILoginUserModel
    {
        public LoginUserModel()
        {

        }

        public LoginUserModel(ILoginUserModel model)
            : base(model)
        {
            UserLevel = model.UserLevel;
            ClientId = model.ClientId;
            Mode = model.Mode;
        }

        public LoginUserModel(string userId, int userLevel, int clientId, int mode, string timeCreated)
            : base(userId, timeCreated)
        {
            UserLevel = userLevel;
            ClientId = clientId;
            Mode = mode;
        }

        public int UserLevel { get; set; }
        public int ClientId { get; set; }
        public int Mode { get; set; }
        public void Insert(string userId, int userLevel, int clientId, int mode, string timeCreated)
        {
            UserId = userId;
            UserLevel = userLevel;
            ClientId = clientId;
            Mode = mode;
            TimeCreated = timeCreated;
        }

        public override string ToString()
        {
            return $"UserId : {UserId}, UserLevel : {UserLevel}, ClientId : {ClientId}, Mode : {Mode}, TimeCreated : {TimeCreated}";
        }
    }
}
