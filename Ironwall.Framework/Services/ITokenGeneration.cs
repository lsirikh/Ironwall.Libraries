using System;

namespace Ironwall.Framework.Services
{
    public interface ITokenGeneration
    {
        public delegate void TokenTimeoutHandler();
        public event TokenTimeoutHandler TokenTimeoutEvent;

        public int Session { get; }
        public string Token { get; }
        public DateTime Expire { get; }
        public abstract void Init();
        public abstract void CreateToken();
        public abstract void SetTimerEnable(bool value);
        public abstract bool SetSession(int seconds);
        public abstract void Run();
    }
}