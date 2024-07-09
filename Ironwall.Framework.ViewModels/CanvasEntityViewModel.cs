using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public abstract class CanvasEntityViewModel<T>
        : Screen
    {
        #region - Ctors
        public CanvasEntityViewModel(T entityProvider, bool visibility = true)
        {
            EntityProvider = entityProvider;
            Visibility = visibility;
        }
        #endregion

        #region - Methods -
        //public abstract ObservableCollection<T> MapEntityProvider(int mapNumber);
        #endregion

        #region - Properties -
        public T EntityProvider { get; }

        public bool Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }
        #endregion

        #region - Attributes -
        private bool visibility;
        #endregion
    }
}
