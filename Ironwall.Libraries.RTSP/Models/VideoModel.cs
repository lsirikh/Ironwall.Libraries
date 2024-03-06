using RtspClientSharp;
using Ironwall.Libraries.RTSP.RawFramesReceiving;
using Ironwall.Libraries.RTSP.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Models
{
    public class VideoModel : IVideoModel
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public async void Start(ConnectionParameters connectionParameters)
        {
            if (_rawFramesSource != null)
                return;

            _rawFramesSource = new RawFramesSource(connectionParameters);
            _rawFramesSource.ConnectionStatusChanged += ConnectionStatusChanged;

            await _realtimeVideoSource.SetRawFramesSource(_rawFramesSource);
            _realtimeAudioSource.SetRawFramesSource(_rawFramesSource);

            _rawFramesSource.Start();
        }

        public async void Stop()
        {
            if (_rawFramesSource == null)
                return;

            _rawFramesSource.Stop();
            await _realtimeVideoSource.SetRawFramesSource(null);
            _rawFramesSource = null;
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void ConnectionStatusChanged(object sender, string s)
        {
            StatusChanged?.Invoke(this, s);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IVideoSource VideoSource => _realtimeVideoSource;
        #endregion
        #region - Attributes -
        private readonly RealtimeVideoSource _realtimeVideoSource = new RealtimeVideoSource();
        private readonly RealtimeAudioSource _realtimeAudioSource = new RealtimeAudioSource();

        private IRawFramesSource _rawFramesSource;
        public event EventHandler<string> StatusChanged;
        #endregion
    }
}
