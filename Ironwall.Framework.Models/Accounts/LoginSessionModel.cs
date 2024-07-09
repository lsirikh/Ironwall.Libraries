using Ironwall.Framework.Models.Communications.Accounts;
using System;

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
        public DateTime TimeExpired { get; set; }

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
            DateTime timeCreated,
            DateTime timeExpired)
            : base(id, userId, timeCreated)
        {
            UserPass = userPass;
            Token = token;
            TimeExpired = timeExpired;
        }
    }
}
