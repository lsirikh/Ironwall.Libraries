using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Accounts
{
    public abstract class LoginBaseModel
        : AccountBaseModel
        , ILoginBaseModel
    {
        public LoginBaseModel()
        {

        }
        public LoginBaseModel(string userId, string timeCreated)
        {
            UserId = userId;
            TimeCreated = timeCreated;
        }

        public LoginBaseModel(ILoginBaseModel model)
            : base(model.Id)
        {
            UserId = model.UserId;
            TimeCreated = model.TimeCreated;
        }

        public LoginBaseModel(int id, string userId, string timeCreated)
            : base(id)
        {
            UserId = userId;
            TimeCreated = timeCreated;
        }


        public string UserId { get; set; }
        public string TimeCreated { get; set; }
    }
}
