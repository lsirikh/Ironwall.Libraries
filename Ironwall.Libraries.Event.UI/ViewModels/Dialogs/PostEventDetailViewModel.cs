using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.ViewModels.ConductorViewModels;

using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dialogs
{
    public class PostEventDetailViewModel
        : Screen
    {
        #region - ctors -
        public PostEventDetailViewModel()
        {
        }
        #endregion

        #region - Binding Methods -
        
        #endregion

        #region - Properties - 
        //ActionReport Id
        public int Id
        {
            get => _eventViewModel.Id;
            set 
            {
                _eventViewModel.Id = value;
                NotifyOfPropertyChange();
            }

        }
        public string Content
        {
            get => _eventViewModel.Content;
            set
            {
                _eventViewModel.Content = value;
                NotifyOfPropertyChange();
            }
        }

        public string User
        {
            get => _eventViewModel.User;
            set
            {
                _eventViewModel.User = value;
                NotifyOfPropertyChange();
            }
        }

        public IMetaEventModel FromEventModel
        {
            get => _eventViewModel.FromEventModel;
            set
            {
                _eventViewModel.FromEventModel = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime DateTime
        {
            get => _eventViewModel.DateTime;
            set
            {
                _eventViewModel.DateTime = value;
                NotifyOfPropertyChange();
            }
        }

        public PostEventViewModel EventViewModel
        {
            get => _eventViewModel;
            set
            {
                _eventViewModel = value;
                NotifyOfPropertyChange(() => EventViewModel);
                Refresh();
            }
        }

        public int Status
        {
            get => _eventViewModel.Status;
            set
            {
                _eventViewModel.Status = value;
                NotifyOfPropertyChange();
            }
        }
        #endregion

        #region - Attibutes - 
        private PostEventViewModel _eventViewModel;
        #endregion
    }
}
