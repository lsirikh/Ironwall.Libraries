using System.Timers;

namespace Ironwall.Libraries.Tcp.Common.Defines
{
    public abstract class TcpTaskTimer : ITcpTaskTimer
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public void InitTimer()
        {
            if (ConnectionTimer != null)
                ConnectionTimer.Close();

            ConnectionTimer = new System.Timers.Timer();
            ConnectionTimer.Elapsed -= ConnectionTick;
            ConnectionTimer.Elapsed += ConnectionTick;

            SetTimerInterval();
        }

        public void SetTimerEnable(bool value)
        {
            ConnectionTimer.Enabled = value;
        }
        public bool GetTimerEnable() => ConnectionTimer.Enabled;
        public void SetTimerInterval(int time = 1000)
        {
            ConnectionTimer.Interval = time;
        }
        public double GetTimerInterval() => ConnectionTimer.Interval;
        public void SetTimerStart()
        {
            SetTimerEnable(true);
            ConnectionTimer.Start();
        }
        public void SetTimerStop()
        {
            ConnectionTimer.Stop();
        }
        public void DisposeTimer()
        {
            ConnectionTimer.Elapsed -= ConnectionTick;

            if (GetTimerEnable())
                ConnectionTimer.Stop();

            if (ConnectionTimer != null)
                ConnectionTimer.Close();

            ConnectionTimer.Dispose();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        protected abstract void ConnectionTick(object sender, ElapsedEventArgs e);
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private System.Timers.Timer ConnectionTimer;
        #endregion
    }
}