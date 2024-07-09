using Ironwall.Framework.Models.Devices;
using System;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/2/2023 3:40:44 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraProfileViewModel : CameraOptionViewModel, ICameraProfileViewModel
    {

        #region - Ctors -
        public CameraProfileViewModel()
        {
            _model = new CameraProfileModel();
        }

        public CameraProfileViewModel(ICameraProfileModel model) : base(model)
        {
            _model = model;
        }

        #endregion
        #region - Implementation of Interface -
        //public override void Dispose()
        //{
        //    _model = new CameraProfileModel();
        //    GC.Collect();
        //}

        //public void UpdateModel(ICameraProfileModel model)
        //{
        //    _model = model;
        //    Refresh();
        //}
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
        public string Profile
        {
            get { return (_model as ICameraProfileModel).Profile; }
            set
            {
                (_model as ICameraProfileModel).Profile = value;
                NotifyOfPropertyChange(() => Profile);
            }
        }
        #endregion
        #region - Attributes -
        #endregion
    }
}
