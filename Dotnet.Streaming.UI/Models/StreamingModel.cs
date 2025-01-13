using Caliburn.Micro;
using Dotnet.Streaming.UI.RawFramesReceiving;
using Ironwall.Libraries.Base.Services;
using RtspClientSharp;
using System;

namespace Dotnet.Streaming.UI.Models
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 5/3/2024 4:17:26 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class StreamingModel : IStreamingModel
    {
        #region - Ctors -
        public StreamingModel()
        {
            _log = IoC.Get<ILogService>();
            _log.Info($"{nameof(StreamingModel)}({this.GetHashCode()}) was created...");
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void Dispose()
        {
            if (_disposed)
                return;

            _log.Info($"{nameof(StreamingModel)}({this.GetHashCode()}) was disposed...");
            // Dispose 로직 추가
            _disposed = true;

            _realtimeVideoSource.SetRawFramesSource(null);
            _rawFramesSource = null;
        }
        public void Start(ConnectionParameters connectionParameters)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(StreamingModel));

            if (_rawFramesSource != null)
                return;

            _log.Info($"{nameof(StreamingModel)}({this.GetHashCode()}) was started...");

            connectionParameters.RtpTransport = RtpTransportProtocol.TCP;
            _rawFramesSource = new RawFramesSource(connectionParameters);
            _rawFramesSource.ConnectionStatusChanged += ConnectionStatusChanged;

            _realtimeVideoSource.SetRawFramesSource(_rawFramesSource);
            _realtimeAudioSource.SetRawFramesSource(_rawFramesSource);

            _rawFramesSource.Start();
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(StreamingModel));

            if (_rawFramesSource == null)
                return;

            _log.Info($"{nameof(StreamingModel)}({this.GetHashCode()}) was stopped...");

            _rawFramesSource.Stop();
            Dispose();
        }

        private void ConnectionStatusChanged(object sender, string s)
        {
            StatusChanged?.Invoke(this, s);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IVideoSource VideoSource => _realtimeVideoSource;
        public bool IsDisposed => _disposed;
        #endregion
        #region - Attributes -
        private readonly RealtimeVideoSource _realtimeVideoSource = new RealtimeVideoSource();
        private readonly RealtimeAudioSource _realtimeAudioSource = new RealtimeAudioSource();

        private IRawFramesSource _rawFramesSource;

        public event EventHandler<string> StatusChanged;
        private bool _disposed;
        private ILogService _log;
        #endregion
    }
}