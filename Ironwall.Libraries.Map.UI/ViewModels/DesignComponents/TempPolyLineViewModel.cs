using Caliburn.Micro;
using Ironwall.Libraries.Map.UI.Models;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols.Components;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ironwall.Libraries.Map.UI.ViewModels.DesignComponents
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/7/2023 3:30:40 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class TempPolyLineViewModel : Screen
    {

        #region - Ctors -
        public TempPolyLineViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Clear();
        }
       
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void AddEllipse(EllipseViewModel ellipseViewModel)
        {
            Ellipses.Add(ellipseViewModel);
            Refresh();
        }


        public void CreateLines() 
        {
            if(Ellipses.Count > 1)
            {
                var line = new LineViewModel();
                var index = Ellipses.Max(entity => entity.Index);
                var firstPoint = Ellipses.Where(e => e.Index == index - 1).FirstOrDefault();
                var lastPoint = Ellipses.Where(e => e.Index == index).FirstOrDefault();

                line.StartPoint = new Point(firstPoint.X + 2, firstPoint.Y + 2);
                line.EndPoint = new Point(lastPoint.X + 2, lastPoint.Y + 2);
                Lines.Add(line);
            }
            Refresh();
        }

        public void Clear()
        {
            Ellipses?.Clear();
            Lines?.Clear();
            Ellipses = new ObservableCollection<EllipseViewModel>();
            Lines = new ObservableCollection<LineViewModel>();
            Refresh();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<EllipseViewModel> Ellipses { get; private set; }
        public ObservableCollection<LineViewModel> Lines { get; private set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
