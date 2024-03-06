using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System;

namespace Ironwall.Libraries.MediaPlayer.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/25/2023 2:12:56 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MediaPlayerViewModel : Screen
    {

        #region - Ctors -
        public MediaPlayerViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void OnLoaded(object source, EventArgs e)
        {
            if (!(source is MediaElement me)) return;

            MediaElement = me;
            MediaElement.LoadedBehavior = MediaState.Manual;
            MediaElement.UnloadedBehavior = MediaState.Manual;
            NotifyOfPropertyChange(() => MediaElement);
            MediaElement.Source = new Uri("c:\\Recordings\\230825125513598_video(30).mp4"); // 비디오 파일 경로
            MediaElement.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                if (MediaElement.NaturalDuration.HasTimeSpan)
                    CurrentPosition = MediaElement.Position.TotalMilliseconds;
                else
                    CurrentPosition = 0d;

                PlayTime = MediaElement.Position.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        public void Play()
        {
            MediaElement.Stop();
            MediaElement.Play();
            if (MediaElement.NaturalDuration.HasTimeSpan)
                MaxPosition = MediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        public void Pause()
        {
            MediaElement.Pause();
        }

        public void Stop()
        {
            MediaElement.Stop();
        }

        public void IncreaseSpeed()
        {
            SpeedRatio += 0.1;
            MediaElement.SpeedRatio = SpeedRatio;
        }

        public void DecreaseSpeed()
        {
            SpeedRatio -= 0.1;
            MediaElement.SpeedRatio = SpeedRatio;
        }

        public void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ProgressBar progressBar)) return;

            double mouseX = e.GetPosition(progressBar).X;
            double ratio = mouseX / progressBar.ActualWidth;
            progressBar.Value = ratio * MaxPosition;
            MediaElement.Position = TimeSpan.FromMilliseconds(progressBar.Value);
        }

        //public void ProgressBar_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isDragging)
        //    {
        //        if (!(sender is ProgressBar progressBar)) return;

        //        double mouseX = e.GetPosition(progressBar).X;
        //        double ratio = mouseX / progressBar.ActualWidth;
        //        progressBar.Value = ratio * MaxPosition;
        //        MediaElement.Position = TimeSpan.FromMilliseconds(progressBar.Value);
        //    }
        //}

        //public void ProgressBar_MouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    isDragging = false;
        //    MediaElement.Play();
        //}
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public MediaElement MediaElement { get; set; }


        public string PlayTime
        {
            get { return _playTime; }
            set 
            {
                _playTime = value; 
                NotifyOfPropertyChange(()=>  PlayTime);
            }
        }


        public double CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                _currentPosition = value;
                NotifyOfPropertyChange(nameof(CurrentPosition));
            }
        }


        public double MaxPosition
        {
            get { return _maxPosition; }
            set
            {
                _maxPosition = value;
                NotifyOfPropertyChange(nameof(MaxPosition));
            }
        }

        public double Width
        {
            get { return _width; }
            set 
            {
                _width = value;
                NotifyOfPropertyChange(nameof(Width));
            }
        }

        public double Height
        {
            get { return _height; }
            set 
            {
                _height = value; 
                NotifyOfPropertyChange(nameof(Height));
            }
        }


        public double SpeedRatio
        {
            get { return _speedRatio; }
            set 
            {
                _speedRatio = value;
                NotifyOfPropertyChange(nameof(SpeedRatio));
            }
        }


        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        private double _speedRatio = 1.0;
        private DispatcherTimer timer;
        private bool isDragging = false;
        private string _playTime;
        private double _currentPosition;
        private double _maxPosition;
        private double _width;
        private double _height;
        #endregion
    }
}
