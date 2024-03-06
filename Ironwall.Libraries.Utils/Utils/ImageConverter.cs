using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Ironwall.Libraries.Utils
{
    public sealed class ImageConverter : IValueConverter
    {   
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var dirPath = Directory.GetParent(Environment.CurrentDirectory).FullName;
            DirectoryInfo di = new DirectoryInfo(System.Environment.CurrentDirectory + @"\Profile");
            string uri;
            if (value != null)
            {
                try
                {
                    uri = Path.Combine(di.FullName, value as string);
                    FileInfo fi = new FileInfo(uri);
                    if (!fi.Exists)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message} ImageConverter Exception!!!!!!!");

                    ///LanSysWebGIS;component/Pictures/Icon/NoImageAvailable.jpg
                    if (parameter as string == "Full")
                    {
                        //uri = "pack://application:,,,/Ironwall.Monitoring.UI;Component/Resources/Images/Profile_default_style.png";
                        //uri = "pack://application:,,,/Ironwall.Libraries.UI.Controls;component/Resources/Images/Profile_default_style.png";
                        uri = "/Resources/Images/Profile_default_style.png";
                    }
                    else
                    {
                        //uri = "pack://application:,,,/Ironwall.Monitoring.UI;Component/Resources/Images/Profile_default_style_64.png";
                        //uri = "pack://application:,,,/Ironwall.Libraries.UI.Controls;component/Resources/Images/Profile_default_style_64.png";
                        uri = "/Resources/Images/Profile_default_style_64.png";
                    }
                }
            }
            else
            {
                ///LanSysWebGIS;component/Pictures/Icon/NoImageAvailable.jpg
                if(parameter as string == "Full")
                {
                    //uri = "pack://application:,,,/Ironwall.Monitoring.UI;Component/Resources/Images/Profile_default_style.png";
                    //uri = "pack://application:,,,/Ironwall.Libraries.UI.Controls;component/Resources/Images/Profile_default_style.png";
                    uri = "/Resources/Images/Profile_default_style.png";
                }
                else
                {
                    //uri = "pack://application:,,,/Ironwall.Monitoring.UI;Component/Resources/Images/Profile_default_style_64.png";
                    //uri = "pack://application:,,,/Ironwall.Libraries.UI.Controls;component/Resources/Images/Profile_default_style_64.png";
                    uri = "/Resources/Images/Profile_default_style_64.png";
                }
            }
            /*
                        ImageSource image = BitmapFrame.Create(new Uri(value as string), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
                        return image;
            */
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
