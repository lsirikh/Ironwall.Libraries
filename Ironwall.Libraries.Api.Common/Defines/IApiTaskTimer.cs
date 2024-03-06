using System;
using System.Timers;

namespace Ironwall.Libraries.Api.Common.Defines
{
    public interface IApiTaskTimer
    {
        /// <summary>
        /// Timer object was Initialized.
        /// </summary>
        public void InitTimer();
        /// <summary>
        /// Dispose Timer object and Related values.
        /// </summary>
        public void DisposeTimer();
        /// <summary>
        /// Timer object set enable or disable.
        /// </summary>
        public void SetTimerEnable(bool value);
        /// <summary>
        /// Get the status of Timer object as a boolean
        /// </summary>
        /// <returns>boolean true/false</returns>
        public bool GetTimerEnable();
        /// <summary>
        /// Set interval for the tick event of Timer object
        /// </summary>
        /// <param name="time"></param>
        public void SetTimerInterval(int time = 1);
        /// <summary>
        /// Get the interval value of Timer object
        /// </summary>
        /// <returns>boolean true/false</returns>
        public double GetTimerInterval();
        /// <summary>
        /// Timer object starts
        /// </summary>
        public void SetTimerStart();
        /// <summary>
        /// Timer object stops
        /// </summary>
        public void SetTimerStop();
    }
}