using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/27/2023 4:22:41 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class TriangleShapeViewModel : ShapeSymbolViewModel
    {

        #region - Ctors -
        public TriangleShapeViewModel()
        {
            TypeShape = (int)EnumShapeType.TRIANGLE;
            Width = 60d;
            Height = 60d;
        }
        public TriangleShapeViewModel(IShapeSymbolModel model)
            : base(model)
        {
            TypeShape = (int)EnumShapeType.TRIANGLE;
            Width = 60d;
            Height = 60d;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //int a = 35;

            //DispatcherService.Invoke((System.Action)(() =>
            //{
            //    Points = new System.Windows.Media.PointCollection() 
            //    { 
            //        new Point(a, 0),
            //        new Point(0, a*Math.Sqrt(3)),
            //        new Point(2*a, a*Math.Sqrt(3)) 
            //    };
            //    var firstP = Points.FirstOrDefault();
            //    var lastP = Points.LastOrDefault();

            //    if (firstP.X == lastP.X && firstP.Y == lastP.Y)
            //        isClosed = true;
            //}));
            //Debug.WriteLine($"OnActivateAsync");
            return base.OnActivateAsync(cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var scaleX = e.NewSize.Width / e.PreviousSize.Width;
            //var scaleY = e.NewSize.Height / e.PreviousSize.Height;

            //// 새로운 PointCollection을 생성하고 각 포인트의 좌표를 크기 변경 비율로 곱함
            //DispatcherService.Invoke((System.Action)(() =>
            //{
            //    PointCollection newPoints = new PointCollection(Points.Select(point => new Point(point.X * scaleX, point.Y * scaleY)));
            //    // 새로운 PointCollection을 기존 Points에 할당
            //    Points = newPoints;
            //}));
            //Debug.WriteLine($"OnSizeChanged");

        }
        
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
