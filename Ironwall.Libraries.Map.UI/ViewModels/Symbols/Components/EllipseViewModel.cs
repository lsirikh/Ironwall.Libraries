using Caliburn.Micro;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols.Components
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/7/2023 3:58:26 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class EllipseViewModel : Screen
    {

        public EllipseViewModel(int index)
        {
            Index = index;
        }

        private double x;
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                NotifyOfPropertyChange(() => X);
            }
        }

        private double y;
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }

        private double _ellipseWidth = 5;
        public double EllipseWidth
        {
            get { return _ellipseWidth; }
            set
            {
                _ellipseWidth = value;
                NotifyOfPropertyChange(() => EllipseWidth);
            }
        }

        private double _ellipseHeight = 5;
        public double EllipseHeight
        {
            get { return _ellipseHeight; }
            set
            {
                _ellipseHeight = value;
                NotifyOfPropertyChange(() => EllipseHeight);
            }
        }

        public int Index { get; set; }
    }
}
