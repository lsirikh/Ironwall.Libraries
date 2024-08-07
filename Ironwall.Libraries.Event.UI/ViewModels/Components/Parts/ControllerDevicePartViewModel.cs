﻿using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components.Parts
{
    public class ControllerDevicePartViewModel
        : Screen
    {
        #region - Ctors -
        public ControllerDevicePartViewModel(IControllerDeviceModel model)
        {
            DeviceModel = model;
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
            get=> DeviceModel.Id;
            set
            {
                DeviceModel.Id = value;
                NotifyOfPropertyChange();
            }
        }
        public int DeviceNumber 
        {
            get => DeviceModel.DeviceNumber;
            set
            {
                DeviceModel.DeviceNumber = value;
                NotifyOfPropertyChange();
            }
        }
        public string DeviceName
        {
            get => DeviceModel.DeviceName;
            set
            {
                DeviceModel.DeviceName = value;
                NotifyOfPropertyChange();
            }
        }
        public EnumDeviceType DeviceType 
        {
            get => DeviceModel.DeviceType;
            set
            {
                DeviceModel.DeviceType = value;
                NotifyOfPropertyChange();
            }
        }
        public string Version
        {
            get => DeviceModel.Version;
            set
            {
                DeviceModel.Version = value;
                NotifyOfPropertyChange();
            }
        }
        public EnumDeviceStatus Status 
        {
            get => DeviceModel.Status;
            set
            {
                DeviceModel.Status = value;
                NotifyOfPropertyChange();
            }
        }
        public string IpAddress 
        {
            get => DeviceModel.IpAddress;
            set
            {
                DeviceModel.IpAddress = value;
                NotifyOfPropertyChange();
            }
        }
        public int Port 
        {
            get => DeviceModel.Port;
            set
            {
                DeviceModel.Port = value;
                NotifyOfPropertyChange();
            }
        }
        public IControllerDeviceModel DeviceModel { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
