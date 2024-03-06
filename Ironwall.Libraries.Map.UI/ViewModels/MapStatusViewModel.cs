using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    public class MapStatusViewModel
        : Screen
    {
        #region - Ctors -
        public MapStatusViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string SelectedMap
        {
            get { return _selectedMap; }
            set 
            { 
                _selectedMap = value; 
                NotifyOfPropertyChange(() => SelectedMap);
            }
        }

        public int ModeWindy
        {
            get => modeWindy;
            set
            {
                modeWindy = value;
                NotifyOfPropertyChange(() => ModeWindy);
            }
        }

        #endregion
        #region - Attributes -
        private string _selectedMap;
        private int modeWindy;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
