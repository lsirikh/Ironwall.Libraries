using Gst.App;
using Gst;
using Ironwall.Libraries.OpenCvRtsp.UI.Utils;
using Ironwall.Libraries.OpenCvRtsp.UI.ViewModels;
using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Gst.Rtsp;

namespace Ironwall.Libraries.OpenCvRtsp.UI.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/25/2023 4:55:04 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class RtspService
    {

        #region - Ctors -
        public RtspService(string rtspUrl, string deviceName, bool isRecording = false, string fileName = default, CancellationTokenSource cts = default) 
        {
            _rtspUrl = rtspUrl;
            _deviceName = deviceName;
            _isRecording = isRecording;
            _fileName = fileName;
            _cts = new CancellationTokenSource();
            _playCts = cts;
        }
        #endregion
        #region - Implementation of Interface -
        
        #endregion
        #region - Overrides -
        public System.Threading.Tasks.Task Start()
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    _capture = new VideoCapture(_rtspUrl); // RTSP URL을 생성자에 전달
                    if (!_capture.IsOpened()) 
                        throw new InvalidOperationException($"RTSP 스트리밍 리소스를 열지 못했습니다.");

                    if (_cts.IsCancellationRequested) return;

                    // 프레임의 너비를 640px로 설정
                    _capture?.Set(VideoCaptureProperties.FrameWidth, 1920 / 4);
                    // 프레임의 높이를 480px로 설정
                    _capture?.Set(VideoCaptureProperties.FrameHeight, 1080 / 4);

                    Log?.Invoke(this, new LogEventArgs("RTSP streaming component was created!"));
                    _captureFps = _capture.Fps;

                    CaptureThread = CreateCaptureThread();
                    CopyThread = CreateCopyThread();
                    if (_isRecording) RecordingThread = CreateRecordingThread(_fileName, _deviceName);
                    ProcessingThread = CreateProcessingThread();

                    CaptureThread.Start();
                    CopyThread.Start();
                    if(_isRecording) RecordingThread?.Start();
                    ProcessingThread.Start();

                    SizeChanged?.Invoke(this, new SizeEventArgs(_capture.FrameWidth, _capture.FrameHeight));
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(Start)} : {ex.ToString()}");
                }
            });
        }

        public async System.Threading.Tasks.Task Stop()
        {
            try
            {
                Log?.Invoke(this, new LogEventArgs("RTSP streaming was finished!"));

                if (_cts != null && !_cts.IsCancellationRequested)
                    _cts?.Cancel();

                if (_playCts != null && !_playCts.IsCancellationRequested)
                    _playCts?.Cancel();

                _cts?.Dispose();
                _playCts?.Dispose();
                await System.Threading.Tasks.Task.Delay(300);

                if (processingQueue.Count > 0)
                {
                    QueueClear();
                }

                if (CaptureThread != null && CaptureThread.Join(300)) CaptureThread.Abort();
                if (CopyThread != null && CopyThread.Join(300)) CopyThread.Abort();
                if (RecordingThread != null && RecordingThread.Join(300)) RecordingThread.Abort();
                if (ProcessingThread != null && ProcessingThread.Join(300)) ProcessingThread.Abort();

                if (_capture != null && !_capture.IsDisposed)
                {
                    _capture?.Dispose();
                    System.Diagnostics.Debug.WriteLine($"Rtsp process(VideoCapture) was terminated!");
                }
                if (_videoWriter != null && !_videoWriter.IsDisposed)
                {
                    _videoWriter?.Dispose();
                    System.Diagnostics.Debug.WriteLine($"Rtsp process(VideoWriter) was terminated!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Raised Exception in {nameof(Stop)} : " + ex.ToString());
            }
        }
        private void QueueClear()
        {
            while (!processingQueue.IsEmpty)
            {
                System.Diagnostics.Debug.WriteLine("Dequeued...");
                processingQueue.TryDequeue(out _);
            }
            GC.Collect();
        }

        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void CalAverageFps(int frameCounter)
        {
            _calAverageCount++;
            _calSumFps += frameCounter;
            _calAverageFps = _calSumFps / _calAverageCount;

            if (_calAverageFps > AVERAGE_FRAME_COUNT)
            {
                _calAverageCount = 0;
                _calSumFps = 0;
            }
        }


        private Thread CreateCaptureThread()
        {
            return new Thread(() =>
            {
                var sleepTime = (long)_capture.Fps;
                var frame = new Mat();
                var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
                stopwatch.Start(); // 시간 측정 시작
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart(); // 시간 측정 시작
                        if (!_capture.Read(frame))
                        {
                            _cts?.Cancel();
                            break;
                        }

                        if (processingQueue.Count <= MAX_QUEUE_SIZE)
                        {
                            if (_totalRecvFrame > FLUSH_FRAME_COUNT)
                            {
                                tranferQueue.Enqueue(frame);
                            }
                            _totalRecvFrame++;
                        }

                        stopwatch.Stop(); // 시간 측정 종료

                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
                        var processingTime = stopwatch.ElapsedMilliseconds;
                        sleepTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processingTime);

                        Thread.Sleep((int)sleepTime);
                    }
                    catch (Exception ex)
                    {
                        _cts.Cancel();
                        System.Diagnostics.Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(CreateCaptureThread)} : " + ex.ToString());
                    }
                }
                System.Diagnostics.Debug.WriteLine($"{nameof(CaptureThread)} was finished");
            });
        }

        private Thread CreateCopyThread()
        {
            return new Thread(() =>
            {
                var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
                stopwatch.Start(); // 시간 측정 시작
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart(); // 시간 측정 시작
                        if (tranferQueue.TryDequeue(out var frame))
                        {
                            if (_isRecording)
                            {
                                var clonedFrame = frame.Clone(); // 프레임 복사
                                recordingQueue.Enqueue(clonedFrame); // 녹화를 위한 프레임 복사
                            }
                            processingQueue.Enqueue(frame);
                        }
                        stopwatch.Stop(); // 시간 측정 종료

                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
                        var processTime = stopwatch.ElapsedMilliseconds;
                        var delayTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processTime);
                        Thread.Sleep((int)delayTime);
                    }
                    catch (Exception ex)
                    {
                        _cts.Cancel();
                        System.Diagnostics.Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(CreateCopyThread)} : " + ex.ToString());
                    }
                }
                System.Diagnostics.Debug.WriteLine($"{nameof(CopyThread)} was finished");
            });
        }

        private Thread CreateProcessingThread()
        {
            return new Thread(() =>
            {
                var refreshStopwatch = new Stopwatch();
                refreshStopwatch.Start();
                var stopwatch = new Stopwatch();

                var frameCounter = 0;
                while (!_playCts.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart();
                        if (processingQueue.TryDequeue(out var frame))
                        {
                            frameCounter++;
                            //await ConvertToBitmapImage(frame);
                            GetFrame?.Invoke(frame);
                        }


                        // 1초마다 FPS 계산
                        if (refreshStopwatch.ElapsedMilliseconds >= 1000)
                        {
                            FpsUpdate?.Invoke(this, new FpsEventArgs(frameCounter)); // 1초 동안 처리된 프레임 수가 FPS가 됩니다.
                            CalAverageFps(frameCounter);

                            // 카운터와 타이머 재설정
                            frameCounter = 0;
                            refreshStopwatch.Restart();
                        }
                        stopwatch.Stop(); // 시간 측정 종료

                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
                        var processTime = stopwatch.ElapsedMilliseconds;
                        var delayTime = Math.Max(0, (int)Math.Round(1000 / _capture.Fps, 1) - processTime);
                        Thread.Sleep((int)delayTime);
                    }
                    catch (Exception ex)
                    {
                        _playCts.Cancel();
                        System.Diagnostics.Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(CreateProcessingThread)} : " + ex.ToString());
                    }
                }
                refreshStopwatch.Stop();
                System.Diagnostics.Debug.WriteLine($"{nameof(ProcessingThread)} was finished");
            });
        }

        private Thread CreateRecordingThread(string eventId = default, string deviceName = default)
        {
            return new Thread(() =>
            {
                string directoryPath = "C:\\Recordings"; // 저장할 디렉터리의 절대 경로
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // 디렉터리가 없으면 생성
                }

                var recordFrame = 0;
                var copyFrame = 0;

                var fps = Math.Round(_captureFps, 0);
                var file = $"{eventId}_{deviceName}_video({fps}).mp4";
                string filePath = Path.Combine(directoryPath, file);
                _videoWriter = new VideoWriter(filePath, FourCC.FromString("mp4v"), fps, new OpenCvSharp.Size((int)_capture.FrameWidth, (int)_capture.FrameHeight));
                Log?.Invoke(this, new LogEventArgs($"The video file({file}) is currently being saved!"));
                Mat lastFrame = null; // 마지막으로 처리된 프레임 저장
                var stopwatch = new Stopwatch(); // 시간 측정을 위한 Stopwatch 객체 생성
                stopwatch.Start();
                System.DateTime startTime = System.DateTime.Now;
                bool isFirst = true;
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        stopwatch.Restart();
                        if (recordingQueue.TryDequeue(out var frame))
                        {
                            if (isFirst)
                            {
                                startTime = System.DateTime.Now;
                                isFirst = false;
                            }

                            // 녹화 처리
                            if (lastFrame != null) lastFrame?.Dispose();

                            _videoWriter?.Write(frame); // 복사본 녹화
                            lastFrame = frame.Clone(); // 마지막 프레임 저장
                            recordFrame++;
                        }
                        else if (lastFrame != null)
                        {
                            // 프레임이 없는 경우 마지막 프레임을 다시 사용
                            // 필요한 경우 프레임 보간 로직을 추가
                            _videoWriter?.Write(lastFrame);
                            recordFrame++;
                            copyFrame++;
                        }
                        stopwatch.Stop(); // 시간 측정 종료

                        // 프레임 처리에 걸린 시간을 계산하여 대기 시간을 동적으로 조정
                        var processTime = stopwatch.ElapsedMilliseconds;
                        var delayTime = Math.Max(0, (int)Math.Round(1000 / fps, 1) - processTime - 1);

                        Thread.Sleep((int)delayTime);
                    }
                    catch (Exception ex)
                    {
                        _cts.Cancel();
                        System.Diagnostics.Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(CreateRecordingThread)} : " + ex.ToString());
                    }
                }
                var timeSpan = System.DateTime.Now - startTime;
                var resultCount = Math.Round(timeSpan.TotalMilliseconds / 33.3);
                var msg = $"저장시간 :{Math.Round(timeSpan.TotalMilliseconds)}ms, 계산된 프레임 : {resultCount}, 실제 수신 프레임: {_totalRecvFrame}, 실제 저장 프레임: {recordFrame} ({copyFrame}), Fps: {(int)fps}";

                Log?.Invoke(this, new LogEventArgs($"Saving the video files{file} was complete."));
                Log?.Invoke(this, new LogEventArgs(msg));

                _videoWriter?.Release(); // 녹화 종료
                System.Diagnostics.Debug.WriteLine($"{nameof(RecordingThread)} was finished");
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public VideoCapture VideoCaptuer => _capture;
        #endregion
        #region - Attributes -
        public Thread CaptureThread;
        public Thread CopyThread;
        public Thread RecordingThread;
        public Thread ProcessingThread;


        private VideoCapture _capture;      // 영상 캡쳐를 위한 객체
        private VideoWriter _videoWriter;   // 비디오 녹화를 위한 객체

        private ConcurrentQueue<Mat> tranferQueue = new ConcurrentQueue<Mat>();
        private ConcurrentQueue<Mat> processingQueue = new ConcurrentQueue<Mat>();
        private ConcurrentQueue<Mat> recordingQueue = new ConcurrentQueue<Mat>();
        //전체 RTSP를 제어하기 위한 CancellationTokenSource
        private CancellationTokenSource _cts;
        //화면 RTSP를 제어하기 위한 CancellationTokenSource
        private CancellationTokenSource _playCts;

        public delegate void FrameEvent(Mat frame);
        public event FrameEvent GetFrame;

        private int _totalRecvFrame;
        private double _captureFps;
        private int _calAverageCount;
        private int _calSumFps;
        private int _calAverageFps;
        private bool _isRecording;
        private string _fileName;
        private string _rtspUrl;
        private string _deviceName;
        private const int MAX_QUEUE_SIZE = 5; // 최대 큐 크기
        private const int AVERAGE_FRAME_COUNT = 10; // 최대 큐 크기
        private const int FLUSH_FRAME_COUNT = 10; // 최대 큐 크기

        public event EventHandler<LogEventArgs> Log;
        public event EventHandler<FpsEventArgs> FpsUpdate;
        public event EventHandler<SizeEventArgs> SizeChanged;

        private Pipeline _pipeline;
        private AppSink _appSink;
        #endregion
    }
}
