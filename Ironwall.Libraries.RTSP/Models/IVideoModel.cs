using RtspClientSharp;
using Ironwall.Libraries.RTSP.Sources;
using System;

namespace Ironwall.Libraries.RTSP.Models
{
    public interface IVideoModel
    {
        event EventHandler<string> StatusChanged;

        public IVideoSource VideoSource { get; }

        void Start(ConnectionParameters connectionParameters);
        void Stop();
    }
}