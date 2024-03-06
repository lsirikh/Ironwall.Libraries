using Caliburn.Micro;
using System.Windows;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols.Components
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/7/2023 5:12:10 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class LineViewModel : Screen
    {

        #region - Ctors -
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
        public Point StartPoint
        {
            get { return _startPoint; }
            set 
            {
                _startPoint = value; 
                NotifyOfPropertyChange(() => StartPoint);   
            }
        }
        public Point EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                NotifyOfPropertyChange(() => _endPoint);
            }
        }
        #endregion
        #region - Attributes -
        private Point _startPoint;
        private Point _endPoint;
        #endregion
    }
}
