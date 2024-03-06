using RtspClientSharp.RawFrames;
using RtspClientSharp.RawFrames.Audio;
using Ironwall.Libraries.RTSP.RawFramesDecoding;
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
    class RealtimeAudioSource : IAudioSource
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void SetRawFramesSource(IRawFramesSource rawFramesSource)
        {
            try
            {
                if (_rawFramesSource != null)
                    _rawFramesSource.FrameReceived -= OnFrameReceived;

                _rawFramesSource = rawFramesSource;

                if (rawFramesSource == null)
                    return;

                rawFramesSource.FrameReceived += OnFrameReceived;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeAudioSource(SetRawFramesSource) : {ex.Message}");
            }
        }

        private void OnFrameReceived(object sender, RawFrame rawFrame)
        {
            try
            {
                if (!(rawFrame is RawAudioFrame rawAudioFrame))
                    return;

                FFmpegAudioDecoder decoder = GetDecoderForFrame(rawAudioFrame);

                if (!decoder.TryDecode(rawAudioFrame))
                    return;

                IDecodedAudioFrame decodedFrame = decoder.GetDecodedFrame(new AudioConversionParameters() { OutBitsPerSample = 16 });

                FrameReceived?.Invoke(this, decodedFrame);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeAudioSource(OnFrameReceived) : {ex.Message}");
            }
        }

        private FFmpegAudioDecoder GetDecoderForFrame(RawAudioFrame audioFrame)
        {
            try
            {
                FFmpegAudioCodecId codecId = DetectCodecId(audioFrame);

                if (!_audioDecodersMap.TryGetValue(codecId, out FFmpegAudioDecoder decoder))
                {
                    int bitsPerCodedSample = 0;

                    if (audioFrame is RawG726Frame g726Frame)
                        bitsPerCodedSample = g726Frame.BitsPerCodedSample;

                    decoder = FFmpegAudioDecoder.CreateDecoder(codecId, bitsPerCodedSample);
                    _audioDecodersMap.Add(codecId, decoder);
                }

                return decoder;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeAudioSource(GetDecoderForFrame) : {ex.Message}");

                return null;
            }
        }

        private FFmpegAudioCodecId DetectCodecId(RawAudioFrame audioFrame)
        {
            try
            {
                if (audioFrame is RawAACFrame)
                    return FFmpegAudioCodecId.AAC;
                if (audioFrame is RawG711AFrame)
                    return FFmpegAudioCodecId.G711A;
                if (audioFrame is RawG711UFrame)
                    return FFmpegAudioCodecId.G711U;
                if (audioFrame is RawG726Frame)
                    return FFmpegAudioCodecId.G726;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in RealtimeAudioSource(DetectCodecId) : {ex.Message}");
            }

            throw new ArgumentOutOfRangeException(nameof(audioFrame));
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private IRawFramesSource _rawFramesSource;

        private readonly Dictionary<FFmpegAudioCodecId, FFmpegAudioDecoder> _audioDecodersMap =
            new Dictionary<FFmpegAudioCodecId, FFmpegAudioDecoder>();

        public event EventHandler<IDecodedAudioFrame> FrameReceived;
        #endregion
    }
}
