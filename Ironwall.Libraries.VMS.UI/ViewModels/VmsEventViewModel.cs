using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Sensorway.Events.Base.Enums;
using Sensorway.Events.Base.Models;
using System;
using System.Runtime.InteropServices;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 7/5/2024 8:51:08 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsEventViewModel : BaseViewModel
    {
        #region - Ctors -
        public VmsEventViewModel(IEventModel model)
        {
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
        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public EnumEventType Type
        {
            get { return _model.Type; }
            set
            {
                _model.Type = value;
                NotifyOfPropertyChange(() => Type);
            }
        }

        public int CameraId
        {
            get { return _model.CameraId; }
            set
            {
                _model.CameraId = value;
                NotifyOfPropertyChange(() => CameraId);
            }
        }

        public string CameraName
        {
            get { return _model.CameraName; }
            set
            {
                _model.CameraName = value;
                NotifyOfPropertyChange(() => CameraName);
            }
        }

        public string TargetPreset
        {
            get { return _model.TargetPreset; }
            set
            {
                _model.TargetPreset = value;
                NotifyOfPropertyChange(() => TargetPreset);
            }
        }

        public string HomePreset
        {
            get { return _model.HomePreset; }
            set
            {
                _model.HomePreset = value;
                NotifyOfPropertyChange(() => HomePreset);
            }
        }

        public int Duration
        {
            get { return _model.Duration; }
            set
            {
                _model.Duration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public IEventModel Model => _model;
        #endregion
        #region - Attributes -
        private IEventModel _model;
        #endregion
    }
}