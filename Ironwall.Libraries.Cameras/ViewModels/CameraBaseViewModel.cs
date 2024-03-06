using Caliburn.Micro;
using Ironwall.Libraries.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.ViewModels
{
    public abstract class CameraBaseViewModel
        : Screen
        , ICameraBaseViewModel
    {
        #region - Ctors -
        public CameraBaseViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }
        public CameraBaseViewModel(ICameraBaseModel model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            _model = model;
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
        public int Id
        {
            get { return _model.Id; }
            set
            {
                _model.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }
        #endregion
        #region - Attributes -
        protected IEventAggregator _eventAggregator;
        protected ICameraBaseModel _model;

        private bool _isSelected;
        #endregion
    }
}
