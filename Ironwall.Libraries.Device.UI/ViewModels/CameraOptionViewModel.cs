﻿using Ironwall.Framework.Models.Devices;
using System;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/27/2023 10:28:33 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraOptionViewModel : BaseOptionViewModel<IBaseOptionModel>, ICameraOptionViewModel
    {

        #region - Ctors -
        public CameraOptionViewModel()
        {
            _model = new BaseOptionModel();
        }

        public CameraOptionViewModel(IBaseOptionModel model) : base(model)
        {
            _model = model;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new BaseOptionModel();
            GC.Collect();
        }
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
