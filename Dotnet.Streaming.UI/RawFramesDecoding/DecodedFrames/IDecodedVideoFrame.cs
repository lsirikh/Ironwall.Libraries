using System;

namespace Dotnet.Streaming.UI.RawFramesDecoding.DecodedFrames
{
    public interface IDecodedVideoFrame
    {
        void TransformTo(IntPtr buffer, int bufferStride, TransformParameters transformParameters);
    }
}