using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.Helpers
{
    public class ClassTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SensorTemplate { get; set; }
        public DataTemplate ControllerTemplate { get; set; }
        public DataTemplate DetectionTemplate { get; set; }
        public DataTemplate ConnectionTemplate { get; set; }
        public DataTemplate MalfunctionTemplate { get; set; }
        public DataTemplate ActionTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DetectionEventViewModel detectionVM)
            {
                if (detectionVM.Device is SensorDeviceViewModel)
                {
                    return SensorTemplate;
                }
                else if (detectionVM.Device is ControllerDeviceViewModel)
                {
                    return ControllerTemplate;
                }
                else
                    return null;
            }
            else if (item is MalfunctionEventViewModel malfunctionVM)
            {
                if (malfunctionVM.Device is SensorDeviceViewModel)
                {
                    return SensorTemplate;
                }
                else if (malfunctionVM.Device is ControllerDeviceViewModel)
                {
                    return ControllerTemplate;
                }
                else
                    return null;
            }
            else if (item is ActionEventViewModel action)
            {
                if (action.FromEvent is DetectionEventViewModel)
                    return DetectionTemplate;

                else if (action.FromEvent is MalfunctionEventViewModel)
                    return MalfunctionTemplate;
                else
                    return null;
            }
            else if (item is SensorDeviceViewModel sensor)
            {
                if (sensor.Controller is ControllerDeviceViewModel)
                    return ControllerTemplate;
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
