using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Libraries.Enums;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Base.Services;
using System;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 11:01:17 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class SymbolBaseViewModel<T> : Screen, ISymbolBaseViewModel<T> where T: ISymbolModel
    {

        #region - Ctors -
        public SymbolBaseViewModel()
        {
        }

        public SymbolBaseViewModel(T model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _log = IoC.Get<ILogService>();
            _model = model;

            
        }
        #endregion
        #region - Implementation of Interface -
        public virtual void UpdateModel(T model)
        {
            _model = model;
            Refresh();
        }
        #endregion
        #region - Overrides -
        public abstract void Dispose();
        public virtual void OnLoaded(object sender, RoutedEventArgs args) { }
        public abstract void OnClickSelect(object sender, EventArgs args);
        public abstract void OnClickEdit(object sender, EventArgs args);
        public abstract void OnClickExit(object sender, EventArgs args);
        public abstract void OnClickCopy(object sender, EventArgs args);
        public abstract void OnClickDelete(object sender, EventArgs args);
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _eventAggregator?.SubscribeOnUIThread(this);
            _log.Info($"## {this.GetType()} was activated ##");
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator?.Unsubscribe(this);
            Dispose();
            _log.Info($"## {this.GetType()} was deactivated ##");
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Id
        {
            get => _model.Id;
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }
        public double X
        {
            get => _model.X;
            set
            {
                _model.X = value;
                NotifyOfPropertyChange(() => X);
            }
        }
        public double Y
        {
            get => _model.Y;
            set
            {
                _model.Y = value;
                NotifyOfPropertyChange(() => Y);
            }
        }
        public double Z
        {
            get => _model.Z;
            set
            {
                _model.Z = value;
                NotifyOfPropertyChange(() => Z);
            }
        }
        public double Width
        {
            get => _model.Width;
            set
            {
                _model.Width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }
        public double Height
        {
            get => _model.Height;
            set
            {
                _model.Height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }
        public double Angle
        {
            get => _model.Angle;
            set
            {
                _model.Angle = value;
                NotifyOfPropertyChange(() => Angle);
            }
        }
        public bool IsShowLable
        {
            get => _model.IsShowLable;
            set
            {
                _model.IsShowLable = value;
                NotifyOfPropertyChange(() => IsShowLable);
            }
        }
        public string Lable
        {
            get => _model.Lable;
            set
            {
                _model.Lable = value;
                NotifyOfPropertyChange(() => Lable);
            }
        }
        public double FontSize
        {
            get => _model.FontSize;
            set
            {
                _model.FontSize = value;
                NotifyOfPropertyChange(() => FontSize);
            }
        }
        public string FontColor
        {
            get => _model.FontColor;
            set
            {
                _model.FontColor = value;
                NotifyOfPropertyChange(() => FontColor);
            }
        }
        public int TypeShape
        {
            get => _model.TypeShape;
            set
            {
                _model.TypeShape = value;
                NotifyOfPropertyChange(() => TypeShape);
            }
        }
        public bool IsVisible
        {
            get => _model.IsVisible;
            set
            {
                _model.IsVisible = value;
                NotifyOfPropertyChange(() => IsVisible);

            }
        }
        public int Layer
        {
            get => _model.Layer;
            set
            {
                _model.Layer = value;
                NotifyOfPropertyChange(() => Layer);
            }
        }
        public int Map
        {
            get => _model.Map;
            set
            {
                _model.Map = value;
                NotifyOfPropertyChange(() => Map);
            }
        }
        public bool IsUsed
        {
            get => _model.IsUsed;
            set
            {
                _model.IsUsed = value;
                NotifyOfPropertyChange(() => IsUsed);
            }
        }


        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public bool IsEditable
        {
            get { return _isEditable; }
            set 
            { 
                _isEditable = value; 
                NotifyOfPropertyChange(() => IsEditable);
            }
        }

        public bool OnEditable
        {
            get { return _onEditable; }
            set 
            { 
                _onEditable = value; 
                NotifyOfPropertyChange(() => OnEditable);
            }
        }

        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        protected ILogService _log;
        protected T _model;

        private bool _isEditable;
        private bool _onEditable;
        private bool isSelected;
        #endregion
    }
}
