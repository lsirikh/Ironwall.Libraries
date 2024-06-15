using Caliburn.Micro;
using Ironwall.Framework.Models;
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
    public class DetectionEventViewModel
        : MetaEventViewModel
        , IDetectionEventViewModel
    {
        #region - Ctors -
        public DetectionEventViewModel()
        {

        }

        public DetectionEventViewModel(IDetectionEventModel model)
            : base(model)
        {
            Result = model.Result;
            try
            {
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
        private int _result;

        public int Result
        {
            get { return _result; }
            set
            {
                _result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }

        

        #endregion
        #region - Attributes -
        #endregion

    }
}
