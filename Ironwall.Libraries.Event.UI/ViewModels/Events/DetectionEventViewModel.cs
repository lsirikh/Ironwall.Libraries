using Caliburn.Micro;
using Ironwall.Framework.Models;
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
        public int Result
        {
            get { return (_model as IDetectionEventModel).Result; }
            set
            {
                (_model as IDetectionEventModel).Result = value;
                NotifyOfPropertyChange(() => Result);
            }
        }
        #endregion
        #region - Attributes -
        #endregion

    }
}
