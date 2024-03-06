using System;
using RtspClientSharp.RawFrames;

namespace Ironwall.Libraries.RTSP.RawFramesReceiving
{
    public interface IRawFramesSource
    {
        EventHandler<RawFrame> FrameReceived { get; set; }
        EventHandler<string> ConnectionStatusChanged { get; set; }

        void Start();
        void Stop();
    }
}