﻿using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Devices.Providers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/9/2023 10:52:08 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class CameraViewModelProvider : WrapperDeviceViewModelProvider<ICameraDeviceModel, CameraDeviceViewModel>
    {
        #region - Ctors -
        public CameraViewModelProvider(DeviceProvider provider) : base(provider)
        {
            ClassName = nameof(CameraViewModelProvider);
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
