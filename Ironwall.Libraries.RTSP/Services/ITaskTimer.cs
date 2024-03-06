namespace Ironwall.Libraries.RTSP.Services
{
    public interface ITaskTimer
    {
        /// <summary>
        /// 타이머 초기화
        /// </summary>
        public void InitTimer();
        public void SetTimerEnable(bool value);
        public bool SetSession(int time = 1);

    }
}