using System;
using System.Timers;

namespace Ironwall.Libraries.Api.Common.Defines
{
    public abstract class ApiTaskTimer : IApiTaskTimer
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        public void InitTimer()
        {
            if (Timer != null)
                Timer.Close();

            Timer = new System.Timers.Timer();
            Timer.Elapsed += Tick;

            SetTimerInterval();
        }

        public void SetTimerEnable(bool value)
        {
            Timer.Enabled = value;
        }
        public bool GetTimerEnable() => Timer.Enabled;
        public void SetTimerInterval(int time = 1000)
        {
            Timer.Interval = time;
        }
        public double GetTimerInterval() => Timer.Interval;
        public void SetTimerStart()
        {
            SetTimerEnable(true);
            Timer.Start();
        }
        public void SetTimerStop()
        {
            Timer.Stop();
        }
        public void DisposeTimer()
        {
            Timer.Elapsed -= Tick;

            if (GetTimerEnable())
                Timer.Stop();

            if (Timer != null)
                Timer.Close();

            Timer.Dispose();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        protected abstract void Tick(object sender, ElapsedEventArgs e);
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private System.Timers.Timer Timer;
        #endregion
    }
}