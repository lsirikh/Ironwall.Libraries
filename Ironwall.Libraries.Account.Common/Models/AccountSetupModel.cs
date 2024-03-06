using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Models
{
    public class AccountSetupModel : IAccountSetupModel
    {
        public string TableSession => Properties.Settings.Default.TableSession;
        public string TableLogin => Properties.Settings.Default.TableLogin;
        public string TableUser => Properties.Settings.Default.TableUser;
        public string PathDatabase => Properties.Settings.Default.PathDatabase;
        public string NameDatabase => Properties.Settings.Default.NameDatabase;


        public int SessionTimeout
        {
            get { return _sessionTimeout; }
            set
            {
                _sessionTimeout = value;
                Properties.Settings.Default.SessionTimeout = _sessionTimeout;
                Properties.Settings.Default.Save();
            }
        }
        private int _sessionTimeout = Properties.Settings.Default.SessionTimeout;
    }
}
