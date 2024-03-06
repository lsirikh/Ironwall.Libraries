using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/8/2023 6:02:06 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class BaseDeviceViewModel<T> : BaseCustomViewModel<T>, IBaseDeviceViewModel<T> where T : IBaseDeviceModel
    {

        #region - Ctors -
        public BaseDeviceViewModel()
        {
        }

        public BaseDeviceViewModel(T model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _model = model;
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
        public int DeviceGroup
        {
            get { return _model.DeviceGroup; }
            set
            {
                _model.DeviceGroup = value;
                NotifyOfPropertyChange(() => DeviceGroup);
            }
        }

        public int DeviceNumber
        {
            get { return _model.DeviceNumber; }
            set
            {
                _model.DeviceNumber = value;
                NotifyOfPropertyChange(() => DeviceNumber);
            }
        }

        public string DeviceName
        {
            get { return _model.DeviceName; }
            set
            {
                _model.DeviceName = value;
                NotifyOfPropertyChange(() => DeviceName);
            }
        }

        public int DeviceType
        {
            get { return _model.DeviceType; }
            set
            {
                _model.DeviceType = value;
                NotifyOfPropertyChange(() => DeviceType);
            }
        }

        public string Version
        {
            get { return _model.Version; }
            set
            {
                _model.Version = value;
                NotifyOfPropertyChange(() => Version);
            }
        }

        public int Status
        {
            get { return _model.Status; }
            set
            {
                _model.Status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
