using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Base.DataProviders;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ironwall.Framework.ViewModels.ConductorViewModels
{
    public abstract class UserBasePanelViewModel
        : BaseViewModel
    {
        #region - Ctors -
        public UserBasePanelViewModel(IEventAggregator eventAggregator, ILogService log)
            : base(eventAggregator, log)
        {
            #region - Settings -
            ClassCategory = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

        }

        public UserBasePanelViewModel(
            IEventAggregator eventAggregator
            , ILogService log
            , IUserModel model
            , ILoginSessionModel loginSessionModel
            ) : base(eventAggregator, log)
        {
            #region - Settings -
            ClassCategory = CategoryEnum.PANEL_SHELL_VM_ITEM;
            #endregion - Settings -

            Model = model;
            SessionModel = loginSessionModel;
        }
        #endregion
        #region - Implementation of Interface -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Clear();
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Overrides -
        //UserControl로 삽입된 Componenet는 Name으로 Command연결이 안된다.
        //Caliburn의 Message.Attach로 연결해주어야 작동함.
        //public abstract void OnClickResetPassword(object sender, EventArgs e);
        public abstract void OnClickPictureAdd(object sender, EventArgs e);
        public void OnPreviewTextInputNumeric(object sender, EventArgs ea)
        {
            if (!(ea is TextCompositionEventArgs e))
                return;

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public virtual void Insert(IUserModel model)
        {
            Model = model;
            Refresh();
        }

        public virtual void Insert(IUserModel model, ILoginSessionModel loginSessionModel)
        {
            Model = model;
            SessionModel = new LoginSessionModel(
                0
                , loginSessionModel.UserId
                , model.Password
                , loginSessionModel.Token
                , loginSessionModel.TimeCreated
                , loginSessionModel.TimeExpired);
            Refresh();
        }

        public virtual void Clear()
        {
            Model = InstanceFactory.Build<UserModel>();
            SessionModel = InstanceFactory.Build<LoginSessionModel>();
            Refresh();
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
            get { return Model.IdUser; }
            set
            {
                Model.IdUser = value;
                NotifyOfPropertyChange(() => IdUser);
            }
        }

        public string Password
        {
            get { return Model.Password; }
            set
            {
                Model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public int Level
        {
            get { return Model.Level; }
            set
            {
                Model.Level = value;
                NotifyOfPropertyChange(() => Level);
            }
        }

        public string UserName
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                //NotifyOfPropertyChange(() => MappingGroup);
                Refresh();
            }
        }

        public bool Used
        {
            get { return Model.Used; }
            set
            {
                Model.Used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }

        public string EmployeeNumber
        {
            get { return Model.EmployeeNumber; }
            set
            {
                Model.EmployeeNumber = value;
                NotifyOfPropertyChange(() => EmployeeNumber);
            }
        }

        public string Birth
        {
            get { return Model.Birth; }
            set
            {
                Model.Birth = value;
                NotifyOfPropertyChange(() => Birth);
            }
        }

        public string Phone
        {
            get { return Model.Phone; }
            set
            {
                Model.Phone = value;
                NotifyOfPropertyChange(() => Phone);
            }
        }

        public string Address
        {
            get { return Model.Address; }
            set
            {
                Model.Address = value;
                NotifyOfPropertyChange(() => Address);
            }
        }

        public string EMail
        {
            get { return Model.EMail; }
            set
            {
                Model.EMail = value;
                NotifyOfPropertyChange(() => EMail);
            }
        }

        public string Image
        {
            get { return Model.Image; }
            set
            {
                Model.Image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public string Position
        {
            get { return Model.Position; }
            set
            {
                Model.Position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }

        public string Department
        {
            get { return Model.Department; }
            set
            {
                Model.Department = value;
                NotifyOfPropertyChange(() => Department);
            }
        }

        public string Company
        {
            get { return Model.Company; }
            set
            {
                Model.Company = value;
                NotifyOfPropertyChange(() => Company);
            }
        }

        public IUserModel Model { get; set; }
        public ILoginSessionModel SessionModel { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
