﻿using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using RtspClientSharp;
using RtspClientSharp.RawFrames;
using RtspClientSharp.Rtsp;

namespace Ironwall.Libraries.RTSP.RawFramesReceiving
{
    class RawFramesSource : IRawFramesSource
    {
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
        private readonly ConnectionParameters _connectionParameters;
        private Task _workTask = Task.CompletedTask;
        private CancellationTokenSource _cancellationTokenSource;

        public EventHandler<RawFrame> FrameReceived { get; set; }
        public EventHandler<string> ConnectionStatusChanged { get; set; }

        public RawFramesSource(ConnectionParameters connectionParameters)
        {
            _connectionParameters =
                connectionParameters ?? throw new ArgumentNullException(nameof(connectionParameters));
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = _cancellationTokenSource.Token;

            _workTask = _workTask.ContinueWith(async p =>
            {
                await ReceiveAsync(token);
            }, token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task ReceiveAsync(CancellationToken token)
        {
            try
            {
                using (var rtspClient = new RtspClient(_connectionParameters))
                {
                    rtspClient.FrameReceived += RtspClientOnFrameReceived;

                    while (true)
                    {
                        OnStatusChanged("Connecting...");
                        Debug.WriteLine($"Try Connecting Video... RawFramesSource(ReceiveAsync)");
                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (InvalidCredentialException ex)
                        {
                            OnStatusChanged("Invalid login and/or password");
                            Debug.WriteLine($"Rasied InvalidCredentialException in RawFramesSource(ReceiveAsync) : {ex.Message}");
                            await Task.Delay(RetryDelay, token);
                            continue;
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.ToString());
                            Debug.WriteLine($"Rasied RtspClientException in RawFramesSource(ReceiveAsync) : {e.Message}");
                            await Task.Delay(RetryDelay, token);
                            continue;
                        }

                        OnStatusChanged("Receiving frames...");
                        Debug.WriteLine($"Receiving frames... RawFramesSource(ReceiveAsync)");

                        try
                        {
                            await rtspClient.ReceiveAsync(token);
                        }
                        catch(SocketException e)
                        {
                            OnStatusChanged(e.ToString());
                            Debug.WriteLine($"Rasied SocketException in RawFramesSource(ReceiveAsync) : {e.Message}");
                        }
                        catch (RtspClientException e)
                        {
                            OnStatusChanged(e.ToString());
                            Debug.WriteLine($"Rasied RtspClientException in RawFramesSource(ReceiveAsync) : {e.Message}");
                            await Task.Delay(RetryDelay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void RtspClientOnFrameReceived(object sender, RawFrame rawFrame)
        {
            FrameReceived?.Invoke(this, rawFrame);
        }

        private void OnStatusChanged(string status)
        {
            ConnectionStatusChanged?.Invoke(this, status);
        }
    }
}