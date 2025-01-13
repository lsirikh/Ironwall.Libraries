using System;
using System.Diagnostics;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Dotnet.Streaming.UI.Models;
using Ironwall.Libraries.Base.Services;
using RtspClientSharp;
using RtspClientSharp.RawFrames;
using RtspClientSharp.Rtsp;

namespace Dotnet.Streaming.UI.RawFramesReceiving
{
    class RawFramesSource : IRawFramesSource
    {
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
        private readonly ILogService _log;
        private readonly ConnectionParameters _connectionParameters;
        private Task _workTask = Task.CompletedTask;
        private CancellationTokenSource _cancellationTokenSource;

        public EventHandler<RawFrame> FrameReceived { get; set; }
        public EventHandler<string> ConnectionStatusChanged { get; set; }

        public RawFramesSource(ConnectionParameters connectionParameters)
        {
            _log = IoC.Get<ILogService>();
            _connectionParameters =
                connectionParameters ?? throw new ArgumentNullException(nameof(connectionParameters));

            _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was created...");
        }

        public void Start()
        {
            Stop(); // 기존 작업 중단 및 리소스 정리

            _cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = _cancellationTokenSource.Token;

            _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was started...");
            _workTask = _workTask.ContinueWith(async p =>
            {
                await ReceiveAsync(token);
            }, token);

        }

        public async void Stop()
        {
            //_cancellationTokenSource.Cancel();
            _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was started to be stopped...");
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;

                _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was executed to cancel and dispose _cancellationTokenSource");
            }

            try
            {
                if (_workTask != null && !_workTask.IsCompleted)
                {
                    _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was executed to wait _workTask.");
                    await _workTask; // 비동기적으로 작업 완료 대기
                    _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was executed to dispose _workTask.");
                }
                    
            }
            //catch (AggregateException ex)
            //{
            //    foreach (var innerException in ex.InnerExceptions)
            //    {
            //        _log.Error($"Task exception: {innerException.Message}");
            //    }
            //}
            catch (Exception ex)
            {
                _log.Error($"Exception while stopping task: {ex.Message}");
            }
            finally
            {
                _workTask = Task.CompletedTask; // _workTask를 초기화하여 다시 사용할 준비
                _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was finished to be stopped...");
            }
        }

        private async Task ReceiveAsync(CancellationToken token)
        {
            using (var rtspClient = new RtspClient(_connectionParameters))
            {
                try
                {
                    rtspClient.FrameReceived += RtspClientOnFrameReceived;
                    while (!token.IsCancellationRequested)
                    {
                        OnStatusChanged("Connecting...");
                        _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was strated to connect camera...");

                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (InvalidCredentialException)
                        {
                            OnStatusChanged("Invalid login and/or password");
                            await Task.Delay(RetryDelay, token);
                            continue;
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.ToString());
                            _log.Error(e.Message);
                            await Task.Delay(RetryDelay, token);
                            continue;
                        }

                        _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was strated to receive frames...");
                        OnStatusChanged("Receiving frames...");

                        try
                        {
                            if (!token.IsCancellationRequested)
                                await rtspClient.ReceiveAsync(token);
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.ToString());
                            _log.Error(e.Message);
                            await Task.Delay(RetryDelay, token);
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    _log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was executed to throw {nameof(TaskCanceledException)}...");
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
                finally
                {
                    rtspClient.FrameReceived -= RtspClientOnFrameReceived;
                }
            }
        }

        private void RtspClientOnFrameReceived(object sender, RawFrame rawFrame)
        {
            try
            {
                //_log.Info($"{nameof(RawFramesSource)}({this.GetHashCode()}) was execuated to propagate a event for receiving frames...");
                FrameReceived?.Invoke(this, rawFrame);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception in RtspClientOnFrameReceived: {ex.Message}");
            }
        }

        private void OnStatusChanged(string status)
        {
            try
            {
                ConnectionStatusChanged?.Invoke(this, status);
            }
            catch (Exception ex)
            {
                _log.Error($"Exception in OnStatusChanged: {ex.Message}");
            }
        }
    }
}