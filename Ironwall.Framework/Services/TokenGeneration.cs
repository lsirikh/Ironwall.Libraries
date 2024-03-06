using Ironwall.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Framework.Services
{
    public sealed class TokenGeneration : ITokenGeneration
    {
        #region - Ctors -
        public TokenGeneration()
        {
            Init();
            Run();
        }
        #endregion

        #region  - EventHandler -
        private void TokenTimeout(object sender, ElapsedEventArgs e)
        {
            Generate();
            TokenTimeoutEvent();
        }
        #endregion

        #region - Properties - 
        public string Token
        {
            get { return _token; }
            private set 
            { 
                _token = value; 
            }
        }

        public DateTime Expire
        {
            get { return _expire; }
            private set 
            { 
                _expire = value; 
            }
        }

        public int Session
        {
            get { return _session; }
            private set
            { 
                _session = value; 
            }
        }

        #endregion

        #region  - Methods - 
        public void Init()
        {
            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(TokenTimeout);
        }

        public void SetTimerEnable(bool value)
        {
            timer.Enabled = value;
        }

        public void Generate()
        {
            CreateToken();
            SetExpire();
            Debug.WriteLine($"============Generate({Token})============");
        }

        public void CreateToken() 
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());
            Token = token;
        }

        private void SetExpire()
        {
            Expire = DateTimeHelper.GetCurrentTimeWithoutMS() + TimeSpan.FromMinutes(Session);
        }

        public bool SetSession(int time = Period)
        {
            if (time == 0)
                return false;
            //Input Value will be minutes
            timer.Interval = TimeSpan.FromMinutes(time).TotalMilliseconds; ;
            Session = time;

            return true;
        }

        public void Run()
        {
            SetSession();
            SetTimerEnable(true);
            Generate();
        }
        #endregion

        #region - Attributes -
        private string _token;
        private DateTime _expire;
        private int _session;
        const int Period = 60; //Expire Duration 1 Hour By Default
        private Timer timer;
        public event ITokenGeneration.TokenTimeoutHandler TokenTimeoutEvent;
        #endregion
    }
}
 
