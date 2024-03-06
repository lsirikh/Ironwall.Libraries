using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Accounts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public class LoginSessionViewModel
        : LoginBaseViewModel, ILoginSessionViewModel
    {
        public LoginSessionViewModel()
        {
        }

        public LoginSessionViewModel(ILoginSessionModel model) : base(model)
        {
            UserPass = model.UserPass;
            Token = model.Token;
            TimeExpired = model.TimeExpired;
        }

        public LoginSessionViewModel(
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

        public void UpdatedModel(ILoginSessionModel model)
        {
            Id = model.Id;
            UserId = model.UserId; 
            UserPass = model.UserPass;
            Token = model.Token;
            TimeCreated = model.TimeCreated;
            TimeExpired = model.TimeExpired;
        }


        public string UserPass
        {
            get { return _userPass; }
            set
            {
                _userPass = value;
                NotifyOfPropertyChange(() => UserPass);
            }
        }


        public string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                NotifyOfPropertyChange(() => Token);
            }
        }


        public string TimeExpired
        {
            get { return _timeExpired; }
            set
            {
                _timeExpired = value;
                NotifyOfPropertyChange(() => TimeExpired);
            }
        }


        private string _userPass;
        private string _token;
        private string _timeExpired;

        
    }
}
