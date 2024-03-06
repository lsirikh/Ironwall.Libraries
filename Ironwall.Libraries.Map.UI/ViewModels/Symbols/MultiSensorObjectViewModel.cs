using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Enums;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/27/2023 1:03:04 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MultiSensorObjectViewModel : ObjectShapeViewModel
    {
        #region - Ctors -
        public MultiSensorObjectViewModel()
        {
            TypeShape = (int)EnumShapeType.MULTI_SNESOR;
            TypeDevice = (int)EnumDeviceType.Multi;
            Width = 60d;
            Height = 60d;
        }
        
        public MultiSensorObjectViewModel(IObjectShapeModel model) : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            await base.OnActivateAsync(cancellationToken);

            //var alarmTask = AlarmTask(token: cts.Token);

            //var delayTask = Task.Run(async () =>
            //{
            //    await Task.Delay(5000);
            //    cts.Cancel();
            //}, cts.Token);

            //await Task.WhenAll(alarmTask, delayTask);
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
