using Caliburn.Micro;
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
using System.Windows.Shapes;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/26/2023 11:14:15 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class FenceObjectViewModel : ObjectShapeViewModel
    {
        #region - Ctors -
        public FenceObjectViewModel()
        {
            ShapeFill = "#00FFFFFF";
            ShapeStroke = "#40FF00";
            ShapeStrokeThick = 4.0d;

            TypeShape = (int)EnumShapeType.FENCE;
            TypeDevice = (int)EnumDeviceType.Fence_Line;
            Points = new PointCollection();
        }
        public FenceObjectViewModel(IObjectShapeModel model)
            : base(model)
        {
            TypeShape = (int)EnumShapeType.FENCE;
            TypeDevice = (int)EnumDeviceType.Fence_Line;
            Points = new PointCollection();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            DispatcherService.Invoke((System.Action)(() =>
            {
                Points = new System.Windows.Media.PointCollection((_model as IObjectShapeModel).Points.Select(p => new Point(p.X, p.Y)));
                var firstP = Points.FirstOrDefault();
                var lastP = Points.LastOrDefault();

                if (firstP.X == lastP.X && firstP.Y == lastP.Y)
                    isClosed = true;
            }));

            #region Deprecated
            //var cts = new CancellationTokenSource();
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        if (cts.IsCancellationRequested) break;
            //
            //        IsAlarming = true;
            //        await Task.Delay(500);
            //        //Debug.WriteLine($"[{DateTime.Now.ToString("yy-MM-dd HH:mm:ss.ff")}] IsAlarming : {IsAlarming}");
            //
            //        if (cts.IsCancellationRequested) break;
            //
            //        IsAlarming = false;
            //        await Task.Delay(500);
            //        //Debug.WriteLine($"[{DateTime.Now.ToString("yy-MM-dd HH:mm:ss.ff")}] IsAlarming : {IsAlarming}");
            //    }
            //}, cts.Token);
            //Task.Run(async () =>
            //{
            //    await Task.Delay(5000);
            //    cts.Cancel();
            //
            //    IsAlarming = false;
            //}, cts.Token);
            #endregion

            return base.OnActivateAsync(cancellationToken);

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
        public double originWidth;
        public double originHeight;
        #endregion
    }
}
