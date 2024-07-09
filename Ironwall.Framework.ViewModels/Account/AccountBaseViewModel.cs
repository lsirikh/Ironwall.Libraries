using Caliburn.Micro;
using Ironwall.Framework.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Account
{
    public abstract class AccountBaseViewModel
        : Screen, IAccountBaseViewModel
    {
        #region - Ctors -
        public AccountBaseViewModel()
        {

        }
        public AccountBaseViewModel(int id)
        {
            Id = id;
        }

        public AccountBaseViewModel(IAccountBaseModel model)
        {
            Id = model.Id;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public virtual void ClearProperty()
        {
            Id = 0;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        //public abstract void UpdatedModel(IAccountBaseModel model);
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
        #endregion
        #region - Attributes -
        private int _id;
        #endregion
    }
}
