using Caliburn.Micro;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Tcp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Tcp.Common.ViewModels
{
    public class TcpUserViewModel : Screen, ITcpUserViewModel
    {
        #region - Ctors -
        public TcpUserViewModel()
        {

        }

        public TcpUserViewModel(int id, ITcpModel tcpModel, IUserModel userModel)
        {
            Id = id;
            TcpModel = tcpModel;
            UserModel = userModel;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public override void Refresh()
        {
            base.Refresh();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }


        public string IpAddress
        {
            get { return TcpModel.IpAddress; }
            set
            {
                TcpModel.IpAddress = value;
                NotifyOfPropertyChange(nameof(IdUser));
            }
        }
        public int Port
        {
            get { return TcpModel.Port; }
            set
            {
                TcpModel.Port = value;
                NotifyOfPropertyChange(nameof(IdUser));
            }
        }

        public string IdUser
        {
            get { return UserModel.IdUser; }
            set
            {
                UserModel.IdUser = value;
                NotifyOfPropertyChange(nameof(IdUser));
            }
        }
        public string Password
        {
            get { return UserModel.Password; }
            set
            {
                UserModel.Password = value;
                NotifyOfPropertyChange(nameof(Password));
            }
        }
        public string Name
        {
            get { return UserModel.Name; }
            set
            {
                UserModel.Name = value;
                NotifyOfPropertyChange(nameof(Name));
            }
        }
        public string EmployeeNumber
        {
            get { return UserModel.EmployeeNumber; }
            set
            {
                UserModel.EmployeeNumber = value;
                NotifyOfPropertyChange(nameof(EmployeeNumber));
            }
        }
        public string Birth
        {
            get { return UserModel.Birth; }
            set
            {
                UserModel.Birth = value;
                NotifyOfPropertyChange(nameof(Birth));
            }
        }
        public string Phone
        {
            get { return UserModel.Phone; }
            set
            {
                UserModel.Phone = value;
                NotifyOfPropertyChange(nameof(Phone));
            }
        }
        public string Address
        {
            get { return UserModel.Address; }
            set
            {
                UserModel.Address = value;
                NotifyOfPropertyChange(nameof(Address));
            }
        }
        public string EMail
        {
            get { return UserModel.EMail; }
            set
            {
                UserModel.EMail = value;
                NotifyOfPropertyChange(nameof(EMail));
            }
        }
        public string Image
        {
            get { return UserModel.Image; }
            set
            {
                UserModel.Image = value;
                NotifyOfPropertyChange(nameof(Image));
            }
        }
        public string Position
        {
            get { return UserModel.Position; }
            set
            {
                UserModel.Position = value;
                NotifyOfPropertyChange(nameof(Position));
            }
        }
        public string Department
        {
            get { return UserModel.Department; }
            set
            {
                UserModel.Department = value;
                NotifyOfPropertyChange(nameof(Department));
            }
        }
        public string Company
        {
            get { return UserModel.Company; }
            set
            {
                UserModel.Company = value;
                NotifyOfPropertyChange(nameof(Company));
            }
        }
        public int Level
        {
            get { return UserModel.Level; }
            set
            {
                UserModel.Level = value;
                NotifyOfPropertyChange(nameof(Level));
            }
        }
        public bool Used
        {
            get { return UserModel.Used; }
            set
            {
                UserModel.Used = value;
                NotifyOfPropertyChange(nameof(Used));
            }
        }

        public ITcpModel TcpModel { get; set; }
        public IUserModel UserModel { get; set; }
        #endregion
        #region - Attributes -
        private int _id;
        #endregion
    }
}
