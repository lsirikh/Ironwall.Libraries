using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 12:41:19 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ObjectShapeViewModel : ShapeSymbolViewModel, IObjectShapeViewModel
    {

        #region - Ctors -
        public ObjectShapeViewModel()
        {
            _model = new ObjectShapeModel();
            ShapeFill = "#FFBF00";
            ShapeStroke = "#00FFFFFF";
            ShapeStrokeThick = 0d;

            IdController = 0;
            IdSensor = 0;

            NameArea = "";
            NameDevice = "";
        }
        public ObjectShapeViewModel(IObjectShapeModel model)
            : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new ObjectShapeModel();
            GC.Collect();
        }

        public Task AlarmTask(DateTime expTime = default, CancellationTokenSource cts = default)
        {
            //if (_cts != null && !_cts.IsCancellationRequested) _cts.Cancel();
            _cts = cts;
            return Task.Run(async () =>
            {
                try
                {
                    DateTime currentTime = DateTime.Now;
                    //DateTime expirationTime = currentTime + TimeSpan.FromSeconds(30);
                    var expirationTime = expTime == default ? DateTime.MaxValue : expTime;

                    while (expirationTime > DateTime.Now)
                    {
                        if (_cts.IsCancellationRequested) break;

                        IsAlarming = true;
                        await Task.Delay(500, _cts.Token);

                        if (_cts.IsCancellationRequested) break;

                        IsAlarming = false;
                        await Task.Delay(500, _cts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    IsAlarming = false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(AlarmTask)}({nameof(ObjectShapeViewModel)}) : {ex.Message}");
                }
                
            }, _cts.Token);
        }

        public Task MalfunctionTask(DateTime expTime = default, CancellationTokenSource cts = default)
        {
            //if (_cts != null && !_cts.IsCancellationRequested) _cts.Cancel();
            _cts = cts;
            return Task.Run(async () =>
            {
                try
                {
                    DateTime currentTime = DateTime.Now;
                    var expirationTime = expTime == default ? DateTime.MaxValue : expTime;

                    IsFault = true;
                    while (expirationTime > DateTime.Now)
                    {
                        if (cts.IsCancellationRequested) break;
                        await Task.Delay(500, _cts.Token);
                    }
                    IsFault = false;
                }
                catch (TaskCanceledException)
                {
                    IsFault = false;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception in {nameof(MalfunctionTask)}({nameof(ObjectShapeViewModel)}) : {ex.Message}");
                }

            }, _cts.Token);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public Task ExecuteCancel()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _log.Info($"Fault Task was cancelled!");
            }
            return Task.CompletedTask;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int IdController
        {
            get { return (_model as IObjectShapeModel).IdController; }
            set
            {
                (_model as IObjectShapeModel).IdController = value;
                NotifyOfPropertyChange(() => IdController);
            }
        }

        public int IdSensor
        {
            get { return (_model as IObjectShapeModel).IdSensor; }
            set
            {
                (_model as IObjectShapeModel).IdSensor = value;
                NotifyOfPropertyChange(() => IdSensor);
            }
        }

        public string NameArea
        {
            get { return (_model as IObjectShapeModel).NameArea; }
            set
            {
                (_model as IObjectShapeModel).NameArea = value;
                NotifyOfPropertyChange(() => NameArea);
            }
        }

        public string NameDevice
        {
            get { return (_model as IObjectShapeModel).NameDevice; }
            set
            {
                (_model as IObjectShapeModel).NameDevice = value;
                NotifyOfPropertyChange(() => NameDevice);
            }
        }

        public int TypeDevice
        {
            get { return (_model as IObjectShapeModel).TypeDevice; }
            set
            {
                (_model as IObjectShapeModel).TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public bool IsAlarming
        {
            get { return _isAlarming; }
            set 
            { 
                _isAlarming = value;
                NotifyOfPropertyChange(() => IsAlarming);
            }
        }

        public bool IsFault
        {
            get { return _isFault; }
            set
            {
                _isFault = value;
                NotifyOfPropertyChange(() => IsFault);
            }
        }
        #endregion
        #region - Attributes -
        private bool _isAlarming;
        private bool _isFault;
        private CancellationTokenSource _cts;
        #endregion
    }
}
