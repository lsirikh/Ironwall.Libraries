using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public class UserBaseViewModel : AccountBaseViewModel, IUserBaseViewModel
    {

        #region - Ctors -
        public UserBaseViewModel()
        {

        }

        public UserBaseViewModel(IUserBaseModel model)
            : base(model.Id)
        {
            IdUser = model.IdUser;
            Password = model.Password;
            Level = model.Level;
            Name = model.Name;
            Used = model.Used;
        }

        public UserBaseViewModel(int id, string idUser, string pass, int level, string name, bool used)
            : base(id)
        {
            IdUser = idUser;
            Password = pass;
            Level = level;
            Name = name;
            Used = used;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void ClearProperty()
        {
            base.ClearProperty();
            IdUser = null;
            Password = null;
            Level = 2;
            Name = null;
            Used = false;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string IdUser
        {
            get { return _idUser; }
            set
            {
                _idUser = value;
                NotifyOfPropertyChange(() => IdUser);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                NotifyOfPropertyChange(() => Level);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public bool Used
        {
            get { return _used; }
            set
            {
                _used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }
        #endregion
        #region - Attributes -
        private string _idUser;
        private string _password;
        private int _level = 2;
        private string _name;
        private bool _used;
        #endregion


    }
}
