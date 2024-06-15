using Ironwall.Framework.Models.Communications.Accounts;

namespace Ironwall.Framework.Models.Accounts
{
    public class LoginSessionModel 
        : LoginBaseModel
        , ILoginSessionModel
    {
        //public string UserId { get; set; }
        public string UserPass { get; set; }
        public string Token { get; set; }
        //public string TimeCreated { get; set; }
        public string TimeExpired { get; set; }

        public LoginSessionModel()
        {

        }

        public LoginSessionModel(ILoginResultModel model)
            : base(model.Details.Id, model.Details.IdUser, model.TimeCreated)
        {
            UserPass = model.Details.Password;
            Token = model.Token;
            TimeExpired = model.TimeExpired;
        }


        public LoginSessionModel(
            int id,
            string userId, 
            string userPass, 
            string token, 
            string timeCreated, 
            string timeExpired)
            : base(id, userId, timeCreated)
        {
            UserPass = userPass;
            Token = token;
            TimeExpired = timeExpired;
        }

        //public void Insert(string userId, string userPass, string token, string timeCreated, string timeExpired)
        //{
        //    UserId = userId;
        //    UserPass = userPass;
        //    Token = token;
        //    TimeCreated = timeCreated;
        //    TimeExpired = timeExpired;
        //}

        //public void Insert(ILoginSessionModel model)
        //{
        //    UserId = model.UserId;
        //    UserPass = model.UserPass;
        //    Token = model.Token;
        //    TimeCreated = model.TimeCreated;
        //    TimeExpired = model.TimeExpired;
        //}
    }
}
