using Caliburn.Micro;
using Ironwall.Libraries.RTSP.DataProviders;
using Ironwall.Libraries.RTSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Libraries.RTSP.Services
{
    public class CameraTaskTimer 
        //: ITaskTimer
    {
        #region - Ctors -
        public CameraTaskTimer()
        {
            _eventAggergator = IoC.Get<IEventAggregator>();
            SetupModel = IoC.Get<CameraSetupModel>();
            
        }
        #endregion
        #region - Implementation of Interface -
        

        public void SetTimerEnable(bool value)
        {
            timer.Enabled = value;
        }

        public bool SetInterval(int time = 1)
        {
            if (time == 0)
                return false;
            //Input Value will be minutes
            timer.Interval = TimeSpan.FromSeconds(time).TotalMilliseconds; ;
            return true;
        }

        private void InitTimer()
        {
            
            if (timer != null)
                timer.Close();

            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(Tick);
            SetTimerEnable(true);
            SetInterval(SetupModel.PtzTimeOut);
        }

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        private void Tick(object sender, ElapsedEventArgs e)
        {
        }

        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        
        public CameraSetupModel SetupModel { get; }
        public CameraDeviceProvider CameraDeviceProvider { get; }
        protected int Session { get; set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggergator;


        private Timer timer;
        #endregion
    }
}
