using OpenCvSharp;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System;
using Ironwall.Framework.Services;
using OpenCvSharp.Extensions;
using Ironwall.Libraries.OpenCvRtsp.UI.Defines;
using System.Windows.Controls;
using Ironwall.Libraries.OpenCvRtsp.UI.Utils;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Timers;
using Ironwall.Libraries.OpenCvRtsp.UI.Services;

namespace Ironwall.Libraries.OpenCvRtsp.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/18/2023 1:58:58 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class RtspComponentViewModel : INotifyPropertyChanged
    {

        #region - Ctors -
        public RtspComponentViewModel( CancellationTokenSource cts = default)
        {
            _cts = cts;
        }
        #endregion
        #region - Implementation of Interface -
        public Task Start(string rtspUrl, string name = default, bool isRecording = false, string fileName = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    _rtspService = new RtspService(rtspUrl, name, isRecording, fileName, _cts);
                    _rtspService.SizeChanged += _rtspService_SizeChanged;
                    _rtspService.Log += _rtspService_Log;
                    _rtspService.FpsUpdate += _rtspService_FpsUpdate;
                    _rtspService.GetFrame += _rtspService_GetFrame;



                    await Task.Delay(1000);
                    await _rtspService.Start().ConfigureAwait(false);

                    Name = name;
                    Visibility = true;
                }
                catch (InvalidOperationException ex)
                {
                    _cts?.Cancel();
                    Debug.WriteLine($"Raised TaskCanceledException in {nameof(Start)} : " + ex.ToString());
                }
                catch (TaskCanceledException ex)
                {
                    _cts?.Cancel();
                    Debug.WriteLine($"Raised TaskCanceledException in {nameof(Start)} : " + ex.ToString());
                }
                catch (Exception ex)
                {
                    _cts?.Cancel();
                    Debug.WriteLine($"Raised Exception in {nameof(Start)} : " + ex.ToString());
                }
            });

        }

        #region Deprecated
        //public Task Start(string rtspUrl, string name = default, bool isRecording = false, string fileName = default)
        //{
        //    return Task.Run(async () =>
        //    {
        //        try
        //        {
        //            if (name != null) Name = name;

        //            await Task.Delay(1000);
        //            //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]Task({nameof(Start)}) in {nameof(RtspComponentViewModel)} => it was started");
        //            Log?.Invoke(this, new LogEventArgs("RTSP streaming was started!"));

        //            _capture = new VideoCapture(rtspUrl); // RTSP URL을 생성자에 전달

        //            if (!_capture.IsOpened()) throw new InvalidOperationException(message:$"RTSP 스트리밍 리소스를 열지 못했습니다. 다시 시도해주세요.");

        //            if (_cts.IsCancellationRequested) return;

        //            // 프레임의 너비를 640px로 설정
        //            _capture?.Set(VideoCaptureProperties.FrameWidth, 1920 / 4);
        //            // 프레임의 높이를 480px로 설정
        //            _capture?.Set(VideoCaptureProperties.FrameHeight, 1080 / 4);

        //            //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]Task({nameof(Start)}) in {nameof(RtspComponentViewModel)} ==> _capture was creataed");
        //            Log?.Invoke(this, new LogEventArgs("RTSP streaming component was created!"));
        //            _totalRecvFrame = 0;
        //            _captureFps = _capture.Fps;
        //            var sleepTime = (long)_capture.Fps;

        //            CaptureThread = new Thread(() =>
        //            {
        //                var frame = new Mat();
        //                var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
        //                stopwatch.Start(); // 시간 측정 시작
        //                while (!_cts.IsCancellationRequested)
        //                {
        //                    try
        //                    {
        //                        stopwatch.Restart(); // 시간 측정 시작
        //                        if (!_capture.Read(frame))
        //                        {
        //                            _cts?.Cancel();
        //                            break;
        //                        }

        //                        if (processingQueue.Count <= MAX_QUEUE_SIZE)
        //                        {
        //                            if(_totalRecvFrame > FLUSH_FRAME_COUNT)
        //                            {
        //                                tranferQueue.Enqueue(frame);
        //                            }
        //                            _totalRecvFrame++;
        //                        }

        //                        stopwatch.Stop(); // 시간 측정 종료

        //                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
        //                        var processingTime = stopwatch.ElapsedMilliseconds;
        //                        sleepTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processingTime);

        //                        Thread.Sleep((int)sleepTime);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _cts.Cancel();
        //                        Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(Start)} : " + ex.ToString());
        //                    }
        //                }
        //                Debug.WriteLine($"{nameof(CaptureThread)} was finished");
        //            });

        //            CopyThread = new Thread(() =>
        //            {
        //                var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
        //                stopwatch.Start(); // 시간 측정 시작
        //                while (!_cts.IsCancellationRequested)
        //                {
        //                    try
        //                    {
        //                        stopwatch.Restart(); // 시간 측정 시작
        //                        if (tranferQueue.TryDequeue(out var frame))
        //                        {
        //                            if(isRecording) 
        //                            {
        //                                var clonedFrame = frame.Clone(); // 프레임 복사
        //                                recordingQueue.Enqueue(clonedFrame); // 녹화를 위한 프레임 복사
        //                            }
        //                            processingQueue.Enqueue(frame);
        //                        }
        //                        stopwatch.Stop(); // 시간 측정 종료

        //                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
        //                        var processTime = stopwatch.ElapsedMilliseconds;
        //                        var delayTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processTime);
        //                        Thread.Sleep((int)delayTime);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _cts.Cancel();
        //                        Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(Start)} : " + ex.ToString());
        //                    }
        //                }
        //                Debug.WriteLine($"{nameof(CopyThread)} was finished");
        //            });

        //            // recordingThread
        //            // 비디오 녹화 설정 (예: MP4 형식, 30fps, 640x480 해상도)
        //            if (isRecording)
        //            {
        //                RecordingThread = CreateRecordingThread(fileName);
        //                RecordingThread?.Start();
        //            }

        //            ProcessingThread = new Thread(async () =>
        //            {
        //                var refreshStopwatch = new Stopwatch();
        //                refreshStopwatch.Start();
        //                var stopwatch = new Stopwatch();

        //                var frameCounter = 0;
        //                while (!_cts.IsCancellationRequested)
        //                {
        //                    try
        //                    {
        //                        stopwatch.Restart();
        //                        if (processingQueue.TryDequeue(out var frame))
        //                        {
        //                            frameCounter++;
        //                            await ConvertToBitmapImage(frame);
        //                        }


        //                        // 1초마다 FPS 계산
        //                        if (refreshStopwatch.ElapsedMilliseconds >= 1000)
        //                        {
        //                            Fps = frameCounter; // 1초 동안 처리된 프레임 수가 FPS가 됩니다.
        //                            CalAverageFps(frameCounter);

        //                            // 카운터와 타이머 재설정
        //                            frameCounter = 0;
        //                            refreshStopwatch.Restart();
        //                        }
        //                        stopwatch.Stop(); // 시간 측정 종료

        //                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
        //                        var processTime = stopwatch.ElapsedMilliseconds;
        //                        var delayTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processTime);
        //                        Thread.Sleep((int)delayTime);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        _cts.Cancel();
        //                        Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(Start)} : " + ex.ToString());
        //                    }
        //                }
        //                refreshStopwatch.Stop();
        //                Debug.WriteLine($"{nameof(ProcessingThread)} was finished");
        //            });
        //            CaptureThread?.Start();
        //            CopyThread?.Start();
        //            ProcessingThread?.Start();

        //            Width = _capture.FrameWidth;
        //            Height = _capture.FrameHeight;

        //            Visibility = true;
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            _cts?.Cancel();
        //            Debug.WriteLine($"Raised TaskCanceledException in {nameof(Start)} : " + ex.ToString());
        //        }
        //        catch (TaskCanceledException ex)
        //        {
        //            _cts?.Cancel();
        //            Debug.WriteLine($"Raised TaskCanceledException in {nameof(Start)} : " + ex.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            _cts?.Cancel();
        //            Debug.WriteLine($"Raised Exception in {nameof(Start)} : " + ex.ToString());
        //        }
        //    });

        //}

        //private Thread CreateRecordingThread(string eventId = default)
        //{
        //    return new Thread(() =>
        //    {
        //        string directoryPath = "C:\\Recordings"; // 저장할 디렉터리의 절대 경로
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath); // 디렉터리가 없으면 생성
        //        }

        //        var recordFrame = 0;
        //        var copyFrame = 0;

        //        var fps = Math.Round(_captureFps, 0);
        //        var file = $"{eventId}_video({fps}).mp4";
        //        string filePath = Path.Combine(directoryPath, file);
        //        _videoWriter = new VideoWriter(filePath, FourCC.FromString("mp4v"), fps, new OpenCvSharp.Size((int)_capture.FrameWidth, (int)_capture.FrameHeight));
        //        Log?.Invoke(this, new LogEventArgs($"The video file({file}) is currently being saved!"));
        //        Mat lastFrame = null; // 마지막으로 처리된 프레임 저장
        //        var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
        //        stopwatch.Start();
        //        DateTime startTime = DateTime.Now;
        //        bool isFirst = true;
        //        while (!_cts.IsCancellationRequested)
        //        {
        //            try
        //            {
        //                stopwatch.Restart();
        //                if (recordingQueue.TryDequeue(out var frame))
        //                {
        //                    if (isFirst)
        //                    {
        //                        startTime = DateTime.Now;
        //                        isFirst = false;
        //                    }

        //                    // 녹화 처리
        //                    if(lastFrame != null) lastFrame?.Dispose();

        //                    _videoWriter?.Write(frame); // 복사본 녹화
        //                    lastFrame = frame.Clone(); // 마지막 프레임 저장
        //                    recordFrame++;
        //                }
        //                else if (lastFrame != null)
        //                {
        //                    // 프레임이 없는 경우 마지막 프레임을 다시 사용
        //                    // 필요한 경우 프레임 보간 로직을 추가
        //                    _videoWriter?.Write(lastFrame);
        //                    recordFrame++;
        //                    copyFrame++;
        //                }
        //                stopwatch.Stop(); // 시간 측정 종료

        //                // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
        //                var processTime = stopwatch.ElapsedMilliseconds;
        //                var delayTime = Math.Max(0, (int)Math.Round(1000 / fps, 1) - processTime - 1);

        //                Thread.Sleep((int)delayTime);
        //            }
        //            catch (Exception ex)
        //            {
        //                _cts.Cancel();
        //                Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(Start)} : " + ex.ToString());
        //            }
        //        }
        //        var timeSpan = DateTime.Now - startTime;
        //        var resultCount = Math.Round(timeSpan.TotalMilliseconds / 33.3);
        //        var msg = $"저장시간 :{Math.Round(timeSpan.TotalMilliseconds)}ms, 계산된 프레임 : {resultCount}, 실제 수신 프레임: {_totalRecvFrame}, 실제 저장 프레임: {recordFrame} ({copyFrame}), Fps: {(int)fps}";

        //        Log?.Invoke(this, new LogEventArgs($"Saving the video files{file} was complete."));
        //        Log?.Invoke(this, new LogEventArgs(msg));

        //        _videoWriter?.Release(); // 녹화 종료
        //        Debug.WriteLine($"{nameof(RecordingThread)} was finished");
        //    });
        //}

        //private void CalAverageFps(int frameCounter)
        //{
        //    _calAverageCount++;
        //    _calSumFps += frameCounter;
        //    _calAverageFps = _calSumFps / _calAverageCount;

        //    if (_calAverageFps > AVERAGE_FRAME_COUNT)
        //    {
        //        _calAverageCount = 0;
        //        _calSumFps = 0;
        //    }
        //}
        #endregion Deprecated

        public async Task Stop()
        {
            try
            {
                //_recordingTimer?.Stop();

                Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff")}]Task({nameof(Stop)}) in {nameof(RtspComponentViewModel)} is Started");
                Visibility = false;

                await _rtspService?.Stop();

                if (_cts != null && !_cts.IsCancellationRequested)
                    _cts?.Cancel();

                _cts?.Dispose();

                #region Deprecated
                //if (processingQueue.Count > 0)
                //{
                //    QueueClear();
                //    await Task.Delay(100);
                //}

                //if (CaptureThread != null && CaptureThread.Join(300)) CaptureThread.Abort();
                //if (CopyThread != null && CopyThread.Join(300)) CopyThread.Abort();
                //if (RecordingThread != null && RecordingThread.Join(300)) RecordingThread.Abort();
                //if (ProcessingThread != null && ProcessingThread.Join(300)) ProcessingThread.Abort();

                //if (_capture != null && !_capture.IsDisposed)
                //{
                //    _capture?.Dispose();
                //    Debug.WriteLine($"Rtsp process(VideoCapture) was terminated!");
                //}
                //if (_videoWriter != null && !_videoWriter.IsDisposed)
                //{
                //    _videoWriter?.Dispose();
                //    Debug.WriteLine($"Rtsp process(VideoWriter) was terminated!");
                //}
                #endregion Deprecated
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Stop)} : " + ex.ToString());
            }
        }
        #region Deprecated
        //private void QueueClear()
        //{
        //    while (!processingQueue.IsEmpty)
        //    {
        //        Debug.WriteLine("Dequeued...");
        //        processingQueue.TryDequeue(out _);
        //    }
        //    CurrentFrame = null;
        //    GC.Collect();
        //}
        #endregion Deprecated

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async void _rtspService_GetFrame(Mat frame)
        {
            await ConvertToBitmapImage(frame);
        }

        private void _rtspService_FpsUpdate(object sender, FpsEventArgs e)
        {
            Fps = e.Fps;
        }

        private void _rtspService_Log(object sender, LogEventArgs e)
        {
            Log?.Invoke(this, new LogEventArgs(e.Log, e.DateTime));
        }

        private void _rtspService_SizeChanged(object sender, SizeEventArgs e)
        {
            (Width, Height) = (e.Width, e.Height);
        }

        private Task ConvertToBitmapImage(Mat frame)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Convert the Mat to a Bitmap
                    using (var bitmap = BitmapConverter.ToBitmap(frame))
                    {
                        // Convert the Bitmap to a BitmapImage
                        using (var memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                            memoryStream.Position = 0;

                            DispatcherService.Invoke((System.Action)(() =>
                            {
                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.StreamSource = memoryStream;
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze();

                                CurrentFrame = bitmapImage;
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Visibility = false;
                    Debug.WriteLine($"Raised Exception in {nameof(ConvertToBitmapImage)} : " + ex.ToString());
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public BitmapImage CurrentFrame
        {
            get => _currentFrame;
            set => SetProperty(ref _currentFrame, ref value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, ref value);
        }

        public double Height
        {
            get => _heith;
            set => SetProperty(ref _heith, ref value);
        }

        public double Fps
        {
            get => _fps;
            set => SetProperty(ref _fps, ref value);
        }


        public bool Visibility
        {
            get => _visibility;
            set => SetProperty(ref _visibility, ref value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, ref value);
        }


        public ICommand SizeChangedCommand
        {
            get
            {
                if (_sizeChangedCommand == null)
                {
                    _sizeChangedCommand = new RelayCommand(param =>
                    {
                        var image = param as Image;
                        if (image != null)
                        {
                            // Access the ActualWidth and ActualHeight properties of the Image control
                            var width = image.ActualWidth;
                            var height = image.ActualHeight;
                            SizeChanged?.Invoke(this, new SizeEventArgs(width, height));
                            Debug.WriteLine($"Image Size {width}x{height}");
                            // Do something with width and height...
                        }
                    });
                }
                return _sizeChangedCommand;
            }
        }

        protected void SetProperty<T>(ref T oldValue, ref T newValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                oldValue = newValue;
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //public Thread CaptureThread { get; private set; }
        //public Thread CopyThread { get; private set; }
        //public Thread RecordingThread { get; private set; }
        //public Thread ProcessingThread { get; private set; }
        #endregion
        #region - Attributes -
        private BitmapImage _currentFrame;
        private double _fps;
        private double _width;
        private double _heith;
        private bool _visibility;
        private string _name;

        private CancellationTokenSource _cts;
        private RtspService _rtspService;

        public event PropertyChangedEventHandler PropertyChanged;
        private ICommand _sizeChangedCommand;
        public event EventHandler<SizeEventArgs> SizeChanged;
        public event EventHandler<LogEventArgs> Log;

        //private VideoCapture _capture;
        //private VideoWriter _videoWriter; // 비디오 녹화를 위한 객체

        //private ConcurrentQueue<Mat> tranferQueue = new ConcurrentQueue<Mat>();
        //private ConcurrentQueue<Mat> processingQueue = new ConcurrentQueue<Mat>();
        //private ConcurrentQueue<Mat> recordingQueue = new ConcurrentQueue<Mat>();
        //private object _locker = new object();

        //private int _totalRecvFrame;
        //private double _captureFps;
        //private int _calAverageCount;
        //private int _calSumFps;
        //private int _calAverageFps;
        //private System.Timers.Timer _recordingTimer;

        //private const int MAX_QUEUE_SIZE = 5; // 최대 큐 크기
        //private const int AVERAGE_FRAME_COUNT = 10; // 최대 큐 크기
        //private const int FLUSH_FRAME_COUNT = 10; // 최대 큐 크기
        #endregion
    }
}
