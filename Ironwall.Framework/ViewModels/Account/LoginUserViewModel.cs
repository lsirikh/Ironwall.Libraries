using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public class LoginUserViewModel
        : LoginBaseViewModel, ILoginUserViewModel
    {
        public LoginUserViewModel()
        {

        }

        public LoginUserViewModel(ILoginUserModel model)
            : base(model)
        {
            UserLevel = model.UserLevel;
            ClientId = model.ClientId;
            Mode = model.Mode;
        }

        public LoginUserViewModel(string userId, int userLevel, int clientId, int mode, string timeCreated)
            : base(userId, timeCreated)
        {
            UserLevel = userLevel;
            ClientId = clientId;
            Mode = mode;
        }

        private int _userLevel;

        public int UserLevel
        {
            get { return _userLevel; }
            set
            {
                _userLevel = value;
                NotifyOfPropertyChange(() => UserLevel);
            }
        }

        private int _clientId;

        public int ClientId
        {
            get { return _clientId; }
            set
            {
                _clientId = value;
                NotifyOfPropertyChange(() => ClientId);
            }
        }

        private int _mode;

        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }



    }
}
