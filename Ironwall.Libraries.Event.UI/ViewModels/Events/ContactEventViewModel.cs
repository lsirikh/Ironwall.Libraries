using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public class ContactEventViewModel : MetaEventViewModel, IContactEventViewModel
    {
        #region - Ctors -
        public ContactEventViewModel()
        {
            _model = new ContactEventModel();
        }

        public ContactEventViewModel(IContactEventModel model) : base(model)
        {

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int ReadWrite
        {
            get { return (_model as IContactEventModel).ReadWrite; }
            set
            {
                (_model as IContactEventModel).ReadWrite = value;
                NotifyOfPropertyChange(() => ReadWrite);
            }
        }

        public int ContactNumber
        {
            get { return (_model as IContactEventModel).ContactNumber; }
            set
            {
                (_model as IContactEventModel).ContactNumber = value;
                NotifyOfPropertyChange(() => ContactNumber);
            }
        }

        public int ContactSignal
        {
            get { return (_model as IContactEventModel).ContactSignal; }
            set
            {
                (_model as IContactEventModel).ContactSignal = value;
                NotifyOfPropertyChange(() => ContactSignal);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
