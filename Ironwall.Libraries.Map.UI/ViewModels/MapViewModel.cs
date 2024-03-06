using Caliburn.Micro;
using Ironwall.Framework.Models.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    public class MapViewModel
        : MapBaseViewModel, IMapViewModel
    {
        #region - Ctors -
        public MapViewModel()
        {

        }

        public MapViewModel(IMapModel model)
        { 
            _model = model;
            
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Refresh()
        {
            base.Refresh();
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
        public string MapName
        {
            get => _model.MapName;
            set
            {
                _model.MapName = value;
                NotifyOfPropertyChange(() => MapName);
            }

        }
        public int MapNumber
        {
            get => _model.MapNumber;
            set
            {
                _model.MapNumber = value;
                NotifyOfPropertyChange(() => MapNumber);
            }
        }
        public string FileName
        {
            get => _model.FileName;
            set
            {
                _model.FileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }
        public string FileType
        {
            get => _model.FileType;
            set
            {
                _model.FileType = value;
                NotifyOfPropertyChange(() => FileType);
            }
        }
        public string Url
        {
            get => _model.Url;
            set
            {
                _model.Url = value;
                NotifyOfPropertyChange(() => Url);
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
        public bool Used
        {
            get => _model.Used;
            set
            {
                _model.Used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }
        public bool Visibility
        {
            get => _model.Visibility;
            set
            {
                _model.Visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        public IMapModel Model => _model;
        #endregion
        #region - Attributes -
        private IMapModel _model;
        #endregion
    }
}
