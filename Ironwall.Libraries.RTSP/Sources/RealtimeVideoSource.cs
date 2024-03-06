using RtspClientSharp.RawFrames;
using RtspClientSharp.RawFrames.Video;
using Ironwall.Libraries.RTSP.RawFramesDecoding.DecodedFrames;
using Ironwall.Libraries.RTSP.RawFramesDecoding.FFmpeg;
using Ironwall.Libraries.RTSP.RawFramesReceiving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Ironwall.Libraries.RTSP.Sources
{
    class RealtimeVideoSource : IVideoSource, IDisposable
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public void Dispose()
        {
            DropAllVideoDecoders();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task SetRawFramesSource(IRawFramesSource rawFramesSource)
        {
            try
            {
                if (_rawFramesSource != null)
                {
                    _rawFramesSource.FrameReceived -= OnFrameReceived;
                    await DropAllVideoDecoders();
                }

                _rawFramesSource = rawFramesSource;

                if (rawFramesSource == null)
                    return;

                rawFramesSource.FrameReceived += OnFrameReceived;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeVideoSource(SetRawFramesSource) : {ex.Message}");
            }
        }

        private void OnFrameReceived(object sender, RawFrame rawFrame)
        {
            try
            {
                if (!(rawFrame is RawVideoFrame rawVideoFrame))
                    return;

                FFmpegVideoDecoder decoder = GetDecoderForFrame(rawVideoFrame);

                IDecodedVideoFrame decodedFrame = decoder.TryDecode(rawVideoFrame);

                if (decodedFrame != null)
                    FrameReceived?.Invoke(this, decodedFrame);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeVideoSource(OnFrameReceived) : {ex.Message}");
            }
        }

        private FFmpegVideoDecoder GetDecoderForFrame(RawVideoFrame videoFrame)
        {
            try
            {
                FFmpegVideoCodecId codecId = DetectCodecId(videoFrame);
                if (!_videoDecodersMap.TryGetValue(codecId, out FFmpegVideoDecoder decoder))
                {
                    decoder = FFmpegVideoDecoder.CreateDecoder(codecId);
                    _videoDecodersMap.Add(codecId, decoder);
                }

                return decoder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeVideoSource(GetDecoderForFrame) : {ex.Message}");
                return null;
            }
        }

        private FFmpegVideoCodecId DetectCodecId(RawVideoFrame videoFrame)
        {
            try
            {
                if (videoFrame is RawJpegFrame)
                    return FFmpegVideoCodecId.MJPEG;
                if (videoFrame is RawH264Frame)
                    return FFmpegVideoCodecId.H264;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeVideoSource(DetectCodecId) : {ex.Message}");
            }

            throw new ArgumentOutOfRangeException(nameof(videoFrame));
        }

        private Task DropAllVideoDecoders()
        {
            return Task.Factory.StartNew(() => 
            {
                try
                {
                    foreach (FFmpegVideoDecoder decoder in _videoDecodersMap.Values)
                        decoder.Dispose();

                    _videoDecodersMap.Clear();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in RealtimeVideoSource(DropAllVideoDecoders) : {ex.Message}");
                }
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IRawFramesSource _rawFramesSource;

        private readonly Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder> _videoDecodersMap =
            new Dictionary<FFmpegVideoCodecId, FFmpegVideoDecoder>();
        public event EventHandler<IDecodedVideoFrame> FrameReceived;
        #endregion

    }
}
