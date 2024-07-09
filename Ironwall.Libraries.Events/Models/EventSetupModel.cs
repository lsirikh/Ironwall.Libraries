using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Models
{
    public class EventSetupModel
    {
        public string TableDetection => Properties.Settings.Default.TableDetection;
        public string TableMalfunction => Properties.Settings.Default.TableMalfunction;
        public string TableAction => Properties.Settings.Default.TableAction;
        public string TableConnection => Properties.Settings.Default.TableConnection;
        public int LengthMaxEventPrev => Properties.Settings.Default.LengthMaxEventPrev;
        public int LengthMinEventPrev => Properties.Settings.Default.LengthMinEventPrev;

        public bool EventMapChange
        {
            get => _eventMapChange;
            set
            {
                _eventMapChange = value;
                Properties.Settings.Default.EventMapChange = _eventMapChange;
                Properties.Settings.Default.Save();
            }
        }

        public bool EventCardMapChange
        {
            get => _eventCardMapChange;
            set
            {
                _eventCardMapChange = value;
                Properties.Settings.Default.EventCardMapChange = _eventCardMapChange;
                Properties.Settings.Default.Save();
            }
        }



        public bool IsEventAutoDiscard
        {
            get => _isEventAutoDiscard;
            set 
            { 
                _isEventAutoDiscard = value;
                Properties.Settings.Default.IsEventAutoDiscard = value;
                Properties.Settings.Default.Save();
            }
        }


        public int EventTimeExpiration
        {
            get => _eventTimeExpiration;
            set 
            { 
                _eventTimeExpiration = value;
                Properties.Settings.Default.EventTimeExpiration = value;
                Properties.Settings.Default.Save();
            }
        }


        private bool _eventMapChange = Properties.Settings.Default.EventMapChange;
        private bool _eventCardMapChange = Properties.Settings.Default.EventCardMapChange;

        private int _eventTimeExpiration = Properties.Settings.Default.EventTimeExpiration;
        private bool _isEventAutoDiscard = Properties.Settings.Default.IsEventAutoDiscard;

    }
}
