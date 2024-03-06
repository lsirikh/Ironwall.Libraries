﻿using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.Libraries.Event.UI.Helpers
{
    public class TypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SensorTemplate { get; set; }
        public DataTemplate ControllerTemplate { get; set; }
        //public DataTemplate DetectionTemplate { get; set; }
        //public DataTemplate ConnectionTemplate { get; set; }
        //public DataTemplate MalfunctionTemplate { get; set; }
        //public DataTemplate ActionTemplate { get; set; }
        //public DataTemplate UserTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MalfunctionEventModel malfunction)
            {
                if (malfunction.Device is SensorDeviceModel)
                {
                    return SensorTemplate;
                }
                else if (malfunction.Device is ControllerDeviceModel)
                {
                    return ControllerTemplate;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
    }
}
