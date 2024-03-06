using GLib;
using Gst.Video;
using Gst;
using System.Linq;
using System;
using Thread = System.Threading.Thread;
using System.Collections.Generic;


namespace Wpf.GstD3DStream.UI.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/19/2024 4:06:05 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class Playback
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public delegate void OnDrawReceivedEventHandler(Element videoSink, GLib.SignalArgs args);

        private Pipeline _pipeline;
        private Bus _bus;
        private MainLoop _mainLoop;
        private Thread _mainGlibThread;
        private Element _uriDecodeBin, _audioConvert, _videoConvert;
        private Element _audioSink, _videoSink;
        private VideoOverlayAdapter _adapter;
        private IntPtr _handle;

        private const bool _enableAudio = false;
        private const string _videoDecoder = "d3d11h264dec"; // This decoder will reduce CPU usage significantly

        private bool _enableOverlay;
        //private string _source = "rtsp://admin:sensorway1@192.168.202.150:8554/cam01_profile2";
        private string _source;


        public Playback(IntPtr hwnd, bool enableOverlay, string gstDebug = "", string uri = "")
        {
            _handle = hwnd;
            _enableOverlay = enableOverlay;

            Environment.SetEnvironmentVariable("GST_DEBUG", "4");

            //if (!String.IsNullOrEmpty(gstDebug))
            //{
            //    Environment.SetEnvironmentVariable("GST_DEBUG", gstDebug);
            //}
            _source = uri;

            Initialize();
        }

        public void Initialize()
        {
            InitGst();
            CreatePipeline();

            var ret = _pipeline.SetState(State.Playing);

            if (ret == StateChangeReturn.Failure)
            {
                Log("Unable to set the pipeline to the playing state.", LogLevelFlags.Error);
                return;
            }
        }

        public OnDrawReceivedEventHandler OnDrawSignalReceived;

        public void SetUri(string uri)
        {
            _source = uri;
        }
        public void InitGst()
        {
            Gst.Application.Init();
            GtkSharp.GstreamerSharp.ObjectManager.Initialize();
            _mainLoop = new MainLoop();
            _mainGlibThread = new Thread(_mainLoop.Run);
            _mainGlibThread.Start();
        }

        private void CreatePipeline()
        {
            Log("Initializing Pipeline..", LogLevelFlags.Debug);
            // 1.파이프라인 생성
            _pipeline = new Pipeline("pipeline0");

            // 2.버스 설정
            _bus = _pipeline.Bus;
            _bus.EnableSyncMessageEmission();
            _bus.AddSignalWatch();
            // 2-1.버스 이벤트 링크
            _bus.Message += OnBusMessage;
            _pipeline.AutoFlushBus = true;

            // 2-2.오버레이 모드가 아닐 경우 버스 Sync 이벤트 링크
            if (!_enableOverlay)
            {
                _bus.SyncMessage += OnBusSyncMessage;
            }

            CreateElements();

            if (_enableAudio)
            {
                _pipeline.Add(_uriDecodeBin, _audioConvert, _videoConvert, _audioSink, _videoSink);

                if (!_audioConvert.Link(_audioSink))
                {
                    Log("Audio sink could not be linked", LogLevelFlags.Error);
                    return;
                }
            }
            else
            {
                _pipeline.Add(_uriDecodeBin, _videoConvert, _videoSink);
            }

            if (!_videoConvert.Link(_videoSink))
            {
                Log("Video sink could not be linked", LogLevelFlags.FlagFatal);
                return;
            }
        }

        private void OnPadAdded(object sender, PadAddedArgs args)
        {
            try
            {
                var src = (Element)sender;
                var newPad = args.NewPad;


                var newPadCaps = newPad.CurrentCaps;
                // 캡슐화 정보 로깅
                if (newPadCaps == null)
                {
                    Log($"Caps for pad {newPad.Name} on element {src.Name}: {newPadCaps.ToString()}", LogLevelFlags.Debug);
                    newPadCaps.Dispose();
                    return;
                }
                var newPadStruct = newPadCaps.GetStructure(0);
                var newPadType = newPadStruct.Name;

                // 오디오 스트림 처리를 활성화하지 않았을 경우 오디오 관련 코드를 실행하지 않습니다.
                if (newPadType.StartsWith("audio/x-raw") && _enableAudio)
                {
                    // _audioConvert 요소가 null이 아닌지 확인합니다.
                    if (_audioConvert == null)
                    {
                        Log("Audio converter element is null", LogLevelFlags.Error);
                        return;
                    }

                    Pad sinkPad = _audioConvert.GetStaticPad("sink");
                    Log($"Received new pad '{newPad.Name}' from '{src.Name}':", LogLevelFlags.Debug);

                    if (sinkPad.IsLinked)
                    {
                        Log("We are already linked. Ignoring.", LogLevelFlags.Warning);
                        return;
                    }

                    var ret = newPad.Link(sinkPad);

                    if (ret != PadLinkReturn.Ok)
                    {
                        Log($"Type is {newPadType} but link failed.", LogLevelFlags.Error);
                    }

                    sinkPad.Dispose();
                }
                else if (newPadType.StartsWith("video/x-raw"))
                {
                    Pad sinkPad = _videoConvert.GetStaticPad("sink");
                    Log($"Received new pad '{newPad.Name}' from '{src.Name}':", LogLevelFlags.Debug);

                    if (sinkPad.IsLinked)
                    {
                        Log("We are already linked, ignoring", LogLevelFlags.Warning);
                        return;
                    }

                    var ret = newPad.Link(sinkPad);

                    if (ret != PadLinkReturn.Ok)
                    {
                        Log($"Type is {newPadType} but link failed", LogLevelFlags.Error);
                    }

                    sinkPad.Dispose();
                }
                else
                {
                    Log($"It has type '{newPadType}' which is not raw audio or video. Ignoring.", LogLevelFlags.Debug);
                    return;
                }

                newPadCaps.Dispose();
                newPadStruct.Dispose();
                newPad.Dispose();
            }
            catch (Exception ex)
            {
                Log($"Raised Exception : {ex.Message}", LogLevelFlags.Error);
            }
            
        }

        private void OnBusSyncMessage(object o, SyncMessageArgs sargs)
        {
            Message msg = sargs.Message;

            if (!Gst.Video.Global.IsVideoOverlayPrepareWindowHandleMessage(msg))
            {
                msg.Dispose();
                return;
            }

            Element src = msg.Src as Element;

            if (src == null)
            {
                msg.Dispose();
                return;
            }

            try
            {
                src["force-aspect-ratio"] = true;
            }
            catch (PropertyNotFoundException ex)
            {
                Console.WriteLine(ex.Message, ex);
            }

            Element overlay = (_pipeline)?.GetByInterface(VideoOverlayAdapter.GType);

            _adapter = new VideoOverlayAdapter(overlay.Handle);
            _adapter.WindowHandle = _handle;
            _adapter.HandleEvents(true);

            msg?.Dispose();
            overlay?.Dispose();
            src?.Dispose();
        }

        private static bool FilterVisFeatures(PluginFeature feature)
        {
            if (!(feature is ElementFactory))
            {
                return false;
            }

            var factory = (ElementFactory)feature;

            return (factory.GetMetadata(Gst.Constants.ELEMENT_METADATA_KLASS).ToLower().Contains("codec/decoder")) || (factory.GetMetadata(Gst.Constants.ELEMENT_METADATA_KLASS).ToLower().Contains("sink/video"));
        }
        private void SetPrimaryDecoder(string decoderName)
        {
            //레지스트리에서 정보를 찾는다.
            var registry = Gst.Registry.Get();
            /*
             * registry: GStreamer의 Registry 인스턴스입니다. Registry는 GStreamer 시스템에 등록된 모든 플러그인과 요소(element)의 정보를 관리합니다.
             * FeatureFilter 메소드: 이 메소드는 Registry에 등록된 모든 플러그인 중 특정 조건을 만족하는 플러그인만을 필터링하는 데 사용됩니다. 여기서는 FilterVisFeatures 함수를 필터 조건으로 사용합니다.
             * FilterVisFeatures 함수: 이 함수는 필터링 조건을 정의합니다. FeatureFilter 메소드에 의해 호출되며, 각 플러그인을 검사하여 특정 조건에 부합하는지 여부를 결정합니다.
             * false: 이 매개변수는 FeatureFilter 메소드에 전달되며, 필터링 과정에서 복사(copy)가 수행되는지 여부를 나타냅니다. false는 실제 객체의 복사본을 생성하지 않고 원본 객체를 사용하겠다는 의미입니다.
             * 코드의 전체적인 의미는, GStreamer의 Registry에서 FilterVisFeatures 함수가 정의한 조건에 부합하는 플러그인들을 선택하여 pluginList라는 변수에 할당하는 것입니다. 이렇게 필터링된 플러그인 목록은 이후의 로직에서 특정 작업을 수행하는 데 사용될 수 있습니다. 예를 들어, 특정 유형의 미디어 처리를 위한 플러그인을 찾거나, 사용 가능한 코덱 목록을 생성하는 데 이 목록이 사용될 수 있습니다.
             */
            var pluginList = registry.FeatureFilter(FilterVisFeatures, false);

            // First we find the current primary decoders
            var primaryPlugins = pluginList.Where(plugin => plugin.Rank == (uint)Rank.Primary);

            // And then downrank them 
            foreach (var plugin in primaryPlugins)
            {
                --plugin.Rank;
            }

            // Then we set our plugin to the primary rank
            PluginFeature d3d11Plugin = pluginList.FirstOrDefault(plugin => plugin.Name == decoderName);

            if (d3d11Plugin != null)
            {
                d3d11Plugin.Rank = (uint)Rank.Primary;
            }
        }

        protected bool CreateElements()
        {
            try
            {
                // 3. 비디오 싱크 생성
                _videoSink = ElementFactory.Make("d3d11videosink", "d3d11videosink0");

                if (_videoSink == null) return false;

                // 오버레이 모드일 경우
                if (_enableOverlay)
                {
                    // 비디오 싱크 텍스쳐 그리기 설정
                    _videoSink["draw-on-shared-texture"] = true;
                    // 비디오 싱크에 begin-draw 연결
                    _videoSink.Connect("begin-draw", VideoSink_OnBeginDraw);
                }

                // 주요 디코더 설정
                // _videoDecoder의 이름을 기준으로 Plug의 등록 리스트에서
                // 해당 플러그인을 인스턴스를 가져옴
                SetPrimaryDecoder(_videoDecoder);

                // 비디오 컨버터 설정
                _videoConvert = ElementFactory.Make("videoconvert", "videoconvert0");

                if (_videoSink == null)
                {
                    Log($"Could not locate Direct3D11", LogLevelFlags.Error);
                    _videoSink = ElementFactory.Make("autovideosink", "autovideosink0");
                }

                // 오디오를 사용하는 경우 관련 요소를 등록한다.
                if (_enableAudio)
                {
                    _audioConvert = ElementFactory.Make("audioconvert", "audioConvert");
                    _audioSink = ElementFactory.Make("autoaudiosink", "audioSink");
                }

                // uridecodebin 빈을 설정하여 uri 설정하고, Pad를 연결한다.
                _uriDecodeBin = ElementFactory.Make("uridecodebin", "source");
                _uriDecodeBin["uri"] = _source;
                _uriDecodeBin.PadAdded += OnPadAdded;
            }
            catch (Exception ex)
            {
                Log(ex.ToString(), LogLevelFlags.Critical, ex);
                return false;
            }
            return true;
        }

        private void OnBusMessage(object o, MessageArgs margs)
        {
            try
            {
                Message message = margs.Message;

                switch (message.Type)
                {
                    case MessageType.StateChanged:
                        if (message.Src == _uriDecodeBin)
                        {
                            message.ParseStateChanged(out State oldState, out State newState, out State pendingState);
                            Log($"UriDecodeBin State Changed: Old State={oldState}, New State={newState}", LogLevelFlags.Info);
                        }
                        break;
                    case MessageType.Eos:

                        Log("Replaying stream...", LogLevelFlags.Info);

                        var ret = _pipeline.SetState(Gst.State.Ready);

                        if (ret == StateChangeReturn.Async)
                            ret = _pipeline.GetState(out var state, out var pending, Gst.Constants.SECOND * 10L);

                        if (ret == StateChangeReturn.Success)
                        {
                            ret = _pipeline.SetState(Gst.State.Playing);
                            if (ret == StateChangeReturn.Async)
                                ret = _pipeline.GetState(out var state, out var pending, Gst.Constants.SECOND * 10L);
                        }
                        break;
                    case MessageType.Error:
                        message.ParseError(out GException err, out string debug);
                        if (debug == null)
                        {
                            debug = "none";
                        }
                        Log($"Error! Bus message: {debug}", LogLevelFlags.Error, err);
                        if (err.Code == 3)
                        {
                            System.Windows.Application.Current.Dispatcher.InvokeShutdown();
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                Log("Bus message error.", LogLevelFlags.Error, ex);
            }
        }

        private void VideoSink_OnBeginDraw(object o, GLib.SignalArgs args)
        {
            OnDrawSignalReceived?.Invoke((Element)o, args);
        }

        public void Log(string message, LogLevelFlags logLevel, Exception exception = null)
        {
            try
            {
                switch (logLevel)
                {
                    case LogLevelFlags.Debug:
                        _log.Debug(message, exception);
                        break;

                    case LogLevelFlags.Info:
                        _log.Info(message, exception);
                        break;

                    case LogLevelFlags.Warning:
                        _log.Warn(message, exception);
                        break;

                    case LogLevelFlags.Error:
                        _log.Error(message, exception);
                        break;

                    case LogLevelFlags.FlagFatal:
                        _log.Fatal(message, exception);
                        break;

                    default:
                        _log.Info(message, exception);
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Log error: {ex.Message}", ex);
            }
        }

        internal void Cleanup()
        {
            try
            {
                Log("Cleaning up pipeline..", LogLevelFlags.Info);
                if(_mainLoop != null && _mainLoop.IsRunning)
                    _mainLoop?.Quit();

                _adapter?.HandleEvents(false);
                _videoSink?.Disconnect("begin-draw", VideoSink_OnBeginDraw);

                if (_pipeline != null && _pipeline.Bus != null)
                {
                    _pipeline.Bus.Message -= OnBusMessage;
                    _pipeline.Bus.SyncMessage -= OnBusSyncMessage;
                }

                if (_uriDecodeBin != null)
                {
                    _uriDecodeBin.PadAdded -= OnPadAdded;
                }

                _pipeline?.SetState(State.Null);
                _pipeline?.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Log("Failed to cleanup resources", LogLevelFlags.Error, ex);
            }
        }
    }
}
