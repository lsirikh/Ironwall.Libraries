using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Ironwall.Libraries.LibVlcRtsp.UI.Views
{
    /// <summary>
    /// Interaction logic for VlcComponentView.xaml
    /// </summary>
    public partial class VlcComponentView : UserControl
    {
        public VlcComponentView()
        {

            // we need the VideoView to be fully loaded before setting a MediaPlayer on it.
            InitializeComponent();
            //_libVLC = new LibVLC();
            //_mediaPlayer = new MediaPlayer(_libVLC);
            //VideoView.Loaded += VideoView_Loaded;
            //Unloaded += VlcComponentView_Unloaded;
        }

        //private void VideoView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    VideoView.MediaPlayer = _mediaPlayer;
        //    if (!VideoView.MediaPlayer.IsPlaying)
        //    {
        //        using (var media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4")))
        //            VideoView.MediaPlayer.Play(media);
        //    }
        //}

        
        //private void VlcComponentView_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    _mediaPlayer.Stop();
        //    _mediaPlayer.Dispose();
        //    _libVLC.Dispose();
        //}

        //void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (VideoView.MediaPlayer.IsPlaying)
        //    {
        //        VideoView.MediaPlayer.Stop();
        //    }
        //}

       

        //LibVLC _libVLC;
        //MediaPlayer _mediaPlayer;
    }

}
