using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public class UserViewModel : UserBaseViewModel, IUserViewModel
    {
        #region - Ctors -
        public UserViewModel()
        {

        }

        public UserViewModel(IUserModel model)
            : base(model)
        {
            EmployeeNumber = model.EmployeeNumber;
            Birth = model.Birth;
            Phone = model.Phone;
            Address = model.Address;
            EMail = model.EMail;
            Image = model.Image;
            Position = model.Position;
            Department = model.Department;
            Company = model.Company;
        }

        public UserViewModel(
            int id,
            string userId,
            string password,
            string name,
            string empnum,
            string birth,
            string phone,
            string address,
            string email,
            string image,
            string position,
            string department,
            string company,
            int level,
            bool used)
            : base(id, userId, password, level, name, used)
        {
            EmployeeNumber = empnum;
            Birth = birth;
            Phone = phone;
            Address = address;
            EMail = email;
            Image = image;
            Position = position;
            Department = department;
            Company = company;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void ClearProperty()
        {
            base.ClearProperty();
            EmployeeNumber = null;
            Birth = null;
            Phone = null;
            EMail=null;
            Image = null;
            Position = null;
            Department = null;
            Company = null;
            IsSelected = false;
        }

        public override string ToString()
        {
            return $"Id:{Id}," +
                $"IdUser:{IdUser}," +
                $"Password:{Password}," +
                $"MappingGroup:{Name}," +
                $"EmployeeNumber:{EmployeeNumber}," +
                $"Birth:{Birth}," +
                $"Phone:{Phone}," +
                $"EMail:{EMail}," +
                $"Address:{Address}," +
                $"Image:{Image}," +
                $"Position:{Position}," +
                $"Department:{Department}," +
                $"Company:{Company}," +
                $"Used:{Used}";
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string EmployeeNumber
        {
            get { return _employeeNumber; }
            set
            {
                _employeeNumber = value;
                NotifyOfPropertyChange(() => EmployeeNumber);
            }
        }

        public string Birth
        {
            get { return _birth; }
            set
            {
                _birth = value;
                NotifyOfPropertyChange(() => Birth);
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyOfPropertyChange(() => Phone);
            }
        }

        public string EMail
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => EMail);
            }
        }

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyOfPropertyChange(() => Address);
            }
        }

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public string Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }

        public string Department
        {
            get { return _department; }
            set
            {
                _department = value;
                NotifyOfPropertyChange(() => Department);
            }
        }

        public string Company
        {
            get { return _company; }
            set
            {
                _company = value;
                NotifyOfPropertyChange(() => Company);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        #endregion
        #region - Attributes -
        private string _employeeNumber;
        private string _birth;
        private string _phone;
        private string _email;
        private string _address;
        private string _image;
        private string _position;
        private string _department;
        private string _company;
        private bool _isSelected;
        #endregion
    }
}
