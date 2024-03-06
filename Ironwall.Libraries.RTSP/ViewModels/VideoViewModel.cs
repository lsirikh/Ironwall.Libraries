using Caliburn.Micro;
using RtspClientSharp;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.Sources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.Libraries.RTSP.ViewModels
{
    public abstract class VideoViewModel : Screen
    {
        
        #region - Ctors -
        public VideoViewModel(IVideoModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _eventAggregator = IoC.Get<IEventAggregator>();
            //ViewModelBinder.Bind(this, new VideoWindowView(), null);
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator.SubscribeOnPublishedThread(this);
            return base.OnActivateAsync(cancellationToken);

        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public void ThickUp(object sender, RoutedEventArgs e)
        {
            Thick = 2;
            Visibility = true;
        }
        public void ThickDown(object sender, RoutedEventArgs e)
        {
            Thick = 0;
            Visibility = false;
        }
        public void VideoClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked!!!");
        }
        public async void OnClickStart(object sender, RoutedEventArgs e)
        {
            await StartVideo();
        }
        public async void OnClickStop(object sender, RoutedEventArgs e)
        {
            await StopVideo();
        }
        #endregion
        #region - Processes -

        public async Task StartVideo()
        {
            await Task.Factory.StartNew(() => 
            {
                try
                {
                    string address = DeviceAddress;

                    Debug.WriteLine($"Start Video Streaming : {DeviceAddress}");
                    if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                        address = RtspPrefix + address;

                    if (!Uri.TryCreate(address, UriKind.Absolute, out Uri deviceUri))
                    {
                        Debug.WriteLine("Invalid device address");
                        //MessageBox.Show("Invalid device address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var credential = new NetworkCredential(UserId, Password);

                    var connectionParameters = !string.IsNullOrEmpty(deviceUri.UserInfo) ? new ConnectionParameters(deviceUri) :
                        new ConnectionParameters(deviceUri, credential);

                    connectionParameters.RtpTransport = RtpTransportProtocol.UDP;
                    connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);

                    _model.Start(connectionParameters);
                    _model.StatusChanged += VideoModelOnStatusChanged;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in VideoViewModel(StartVideo) : {ex.Message}");
                }
            });
        }

        public async Task StopVideo()
        {
            await Task.Factory.StartNew(() => 
            {
                try
                {
                    _model.Stop();
                    _model.StatusChanged -= VideoModelOnStatusChanged;

                    Status = string.Empty;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in VideoViewModel(StopVideo) : {ex.Message}");
                }
            });
            //await Task.Delay(100);
        }

        private void VideoModelOnStatusChanged(object sender, string s)
        {
            if(Application.Current != null)
                Application.Current.Dispatcher.Invoke(() => Status = s);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyOfPropertyChange(()=> Status);
            }
        }


        public int Thick
        {
            get { return _thick; }
            set 
            { 
                _thick = value;
                NotifyOfPropertyChange(() => Thick);
            }
        }


        public bool Visibility
        {
            get { return _visibility; }
            set 
            {
                _visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        public string Name { get; set; }

        public string DeviceAddress { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public IVideoSource VideoSource => _model.VideoSource;
        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        protected readonly IVideoModel _model;
        
        private int _thick;
        private bool _visibility;
        private string _status = string.Empty;

        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";
        #endregion
    }
}
