using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public abstract class LoginBaseViewModel
        : AccountBaseViewModel, ILoginBaseViewModel
    {
        public LoginBaseViewModel()
        {

        }

        public LoginBaseViewModel(string userId, string timeCreated)
        {
            UserId = userId;
            TimeCreated = timeCreated;
        }

        public LoginBaseViewModel(ILoginBaseModel model)
            : base(model)
        {
            UserId = model.UserId;
            TimeCreated = model.TimeCreated;
        }

        public LoginBaseViewModel(int id, string userId, string timeCreated)
            : base(id)
        {
            UserId = userId;
            TimeCreated = timeCreated;
        }


        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                NotifyOfPropertyChange(() => UserId);
            }
        }


        public string TimeCreated
        {
            get { return _timeCreated; }
            set
            {
                _timeCreated = value;
                NotifyOfPropertyChange(() => TimeCreated);
            }
        }


        private string _userId;
        private string _timeCreated;
    }
}
