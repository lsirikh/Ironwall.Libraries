using System;

namespace Ironwall.Libraries.RTSP.RawFramesDecoding.DecodedFrames
{
    public interface IDecodedAudioFrame
    {
        DateTime Timestamp { get; }
        ArraySegment<byte> DecodedBytes { get; }
        AudioFrameFormat Format { get; }
    }
}