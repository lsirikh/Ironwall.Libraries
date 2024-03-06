using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VlcRTSP.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;

namespace Ironwall.Libraries.VlcRTSP.ViewModels
{
    public class VlcComponentViewModel
        : Screen
        , IVlcComponentViewModel
    {
        #region - Ctors -
        public VlcComponentViewModel()
        {
            _log = IoC.Get<ILogService>();
        }
        ~VlcComponentViewModel()
        {
            VlcControl = null;
            VlcView = null;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (!(view is VlcComponentView viewData))
                return;

            VlcView = viewData;
            VlcControl = VlcView.VlcControl;
            Visibility = false;
            var ret = await CreateVlcPlayer();
            if (ret)
                await StartVideo();
        }


        private Task<bool> CreateVlcPlayer()
        {
            return Task.Run(() =>
            {
                try
                {
                    var libDirectory = FindLib();

                    //var options = new string[] { "--network-caching=150", "--sout-mux-caching=150" };
                    //var options = new string[] { "--network-caching=200" };
                    //var options = new string[]
                    //{
                    //    "--network-caching=250"
                    //    ,"--sout-qsv-software"
                    //    , "--sout-mux-caching=300"
                    //    //, "--network-synchronisation"
                    //    , "--directx-use-sysmem"
                    //    , $"{VlcControl.ActualWidth}x{VlcControl.ActualHeight}"
                    //};

                    var destination = Path.Combine(GetCurrentDirectory(), "record.mp4");
                    var options = new[]
                    {
                        "--intf", "dummy", /* no interface                   */
                        ":sout=#file{dst=" + destination + "}",
                        ":sout-keep",
                        "--no-audio", /* we don't want audio decoding   */
                        "--no-video-title-show", /* nor the filename displayed     */
                        "--no-stats", /* no stats */
                        "--no-sub-autodetect-file", /* we don't want subtitles        */
                        "--no-snapshot-preview", /* no blending in dummy vout      */
                    };

                    VlcControl?.SourceProvider?.CreatePlayer(libDirectory, options /*pass your player parameters here*/);
                    //VlcControl?.SourceProvider?.MediaPlayer.SetVideoFormatCallbacks(this.VideoFormat, this.CleanupVideo);
                    VlcControl.SourceProvider.MediaPlayer.Log += LogginVlc;
                    IsCreated = true;
                }
                catch (AccessViolationException ex)
                {
                    _log.Error($"Raised AccessViolationException in {nameof(CreateVlcPlayer)} : {ex.Message}");
                    VlcControl.Dispose();
                    VlcControl = new VlcControl();
                    CreateVlcPlayer();
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(CreateVlcPlayer)} : {ex.Message}");
                    VlcControl.Dispose();
                    return false;
                }
                return true;
            });

        }

        //private void CleanupVideo(ref IntPtr userData)
        //{
        //}

        //private uint VideoFormat(out IntPtr userData, IntPtr chroma, ref uint width, ref uint height, ref uint pitches, ref uint lines)
        //{
        //}

        private string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        private DirectoryInfo FindLib()
        {
            //var currentAssembly = Assembly.GetEntryAssembly();
            //var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            var currentDirectory = GetCurrentDirectory();
            var currentDirectoryName = new FileInfo(currentDirectory).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            return new DirectoryInfo(System.IO.Path.Combine(currentDirectoryName, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //_eventAggregator?.SubscribeOnPublishedThread(this);
            await base.OnActivateAsync(cancellationToken);

        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            //_eventAggregator?.Unsubscribe(this);
            await base.OnDeactivateAsync(close, cancellationToken);
            await VlcDispose();
        }

        private Task VlcDispose()
        {
            return Task.Run(async () =>
            {
                try
                {
                    await StopVideo();

                    await Task.Delay(500);
                    VlcControl.SourceProvider.MediaPlayer.Log -= LogginVlc;
                    VlcControl.SourceProvider.MediaPlayer.ResetMedia();
                    VlcControl.Dispose();
                    VlcControl = null;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(VlcDispose)} : {ex.Message}");
                }
                finally
                {
                    GC.Collect(0);
                    GC.WaitForPendingFinalizers();
                }
            });
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void LogginVlc(object sender, VlcMediaPlayerLogEventArgs args)
        {
            string message = $"libVlc(Monitoring) : {args.Level} {args.Message} @ {args.Module}";
            if (args.Message.Contains("Buffering"))
                StreamingLog = args.Message;
            else
                StreamingLog = null;
        }
        public Task StartVideo()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    VlcControl?.SourceProvider.MediaPlayer.Play(new Uri($"rtsp://{UserId}:{Password}@{DeviceAddress}:{Port}/{RtspUrl}"));
                    Visibility = true;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in StartVideo : {ex.ToString()}");
                }
            });
        }

        public Task StopVideo()
        {
            return Task.Run(() =>
            {
                try
                {
                    if (VlcControl?.SourceProvider?.MediaPlayer != null
                    && VlcControl.SourceProvider.MediaPlayer.IsPlaying())
                    {
                        VlcControl.SourceProvider.MediaPlayer.Stop();
                    }
                    Visibility = false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in StopVideo : {ex.ToString()}");
                }
                finally
                {
                    GC.Collect(0);
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public VlcControl VlcControl { get; set; }

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

        public string StreamingLog
        {
            get { return _streamingLog; }
            set
            {
                _streamingLog = value;
                NotifyOfPropertyChange(() => StreamingLog);
            }
        }

        public string Name { get; set; }
        public string DeviceAddress { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string RtspUrl { get; set; }
        public bool IsCreated { get; private set; }

        public VlcComponentView VlcView { get; set; }
        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        private string _streamingLog;
        private bool _visibility;
        private int _thick;
        private ILogService _log;
        #endregion
    }
}
