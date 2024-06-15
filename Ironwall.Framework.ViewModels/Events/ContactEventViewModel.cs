using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels.Events
{
    public class ContactEventViewModel
        : MetaEventViewModel, IContactEventViewModel
    {
        #region - Ctors -
        public ContactEventViewModel()
        {

        }

        public ContactEventViewModel(IContactEventModel model)
        {

            try
            {
                ReadWrite = model.ReadWrite;
                ContactNumber = model.ContactNumber;
                ContactSignal = model.ContactSignal;

                Device = ViewModelFactory.Build<SensorDeviceViewModel>(model.Device as ISensorDeviceModel);
            }
            catch (Exception)
            {

                throw;
            }
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
        private int _readWrite;

        public int ReadWrite
        {
            get { return _readWrite; }
            set
            {
                _readWrite = value;
                NotifyOfPropertyChange(() => ReadWrite);
            }
        }

        private int _contactNumber;

        public int ContactNumber
        {
            get { return _contactNumber; }
            set
            {
                _contactNumber = value;
                NotifyOfPropertyChange(() => ContactNumber);
            }
        }
        private int _contactSignal;

        public int ContactSignal
        {
            get { return _contactSignal; }
            set
            {
                _contactSignal = value;
                NotifyOfPropertyChange(() => ContactSignal);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
