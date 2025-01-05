using Caliburn.Micro;
using Ironwall.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public abstract class EntityViewModel
        : Screen, IEntityViewModel, IDisposable
    {
        #region - Ctors -
        public EntityViewModel()
        {
        }

        public EntityViewModel(SemaphoreSlim semaphoreSlim)
        {
            EntityViewModel.semaphoreSlim = semaphoreSlim;
        }
        //Question SemaphoreSlim 활용 이유 및 의미
        public EntityViewModel(IEntityModel entityModel)
            : this(new SemaphoreSlim(1, 1))
        {
            this.entityModel = entityModel;
        }
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            //EventAggregator = IoC.Get<IEventAggregator>();
            EventAggregator?.SubscribeOnUIThread(this);
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            EventAggregator?.Unsubscribe(this);
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Implementations for IDisposable -
        public void Dispose()
        {
        }
        #endregion 
        #region - Properties -
        public int Id
        {
            get => entityModel.Id;
            set
            {
                entityModel.Id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string NameArea
        {
            get => entityModel.NameArea;
            set
            {
                entityModel.NameArea = value;
                NotifyOfPropertyChange(() => NameArea);
            }
        }

        public int TypeDevice
        {
            get => entityModel.TypeDevice;
            set
            {
                entityModel.TypeDevice = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public string NameDevice
        {
            get => entityModel.NameDevice;
            set
            {
                entityModel.NameDevice = value;
                NotifyOfPropertyChange(() => NameDevice);
            }
        }

        public int TypeShape
        {
            get => entityModel.TypeShape;
            set
            {
                entityModel.TypeShape = value;
                NotifyOfPropertyChange(() => TypeDevice);
            }
        }

        public double X1
        {
            get => entityModel.X1;
            set
            {
                entityModel.X1 = value;
                NotifyOfPropertyChange(() => X1);
            }
        }

        public double Y1
        {
            get => entityModel.Y1;
            set
            {
                entityModel.Y1 = value;
                NotifyOfPropertyChange(() => Y1);
            }
        }

        public double X2
        {
            get => entityModel.X2;
            set
            {
                entityModel.X2 = value;
                NotifyOfPropertyChange(() => X2);
            }
        }

        public double Y2
        {
            get => entityModel.Y2;
            set
            {
                entityModel.Y2 = value;
                NotifyOfPropertyChange(() => Y2);
            }
        }

        public double Width
        {
            get => entityModel.Width;
            set
            {
                entityModel.Width = value;
                NotifyOfPropertyChange(() => Width);
            }
        }

        public double Height
        {
            get => entityModel.Height;
            set
            {
                entityModel.Height = value;
                NotifyOfPropertyChange(() => Height);
            }
        }

        public double Angle
        {
            get => entityModel.Angle;
            set
            {
                entityModel.Angle = value;
                NotifyOfPropertyChange(() => Angle);
            }
        }
        public int IdController
        {
            get => entityModel.IdController;
            set
            {
                entityModel.IdController = value;
                NotifyOfPropertyChange(() => IdController);
            }
        }
        public int IdSensor
        {
            get => entityModel.IdSensor;
            set
            {
                entityModel.IdSensor = value;
                NotifyOfPropertyChange(() => IdSensor);
            }
        }

        public int Map
        {
            get => entityModel.Map;
            set
            {
                entityModel.Map = value;
                NotifyOfPropertyChange(() => Map);
            }
        }

        public bool Used
        {
            get => entityModel.Used;
            set
            {
                entityModel.Used = value;
                NotifyOfPropertyChange(() => Used);
            }
        }

        public bool Visibility
        {
            get => entityModel.Visibility;
            set
            {
                entityModel.Visibility = value;
                NotifyOfPropertyChange(() => Visibility);
            }
        }
        #endregion
        #region - Attributes -
        protected IEntityModel entityModel;
        protected static SemaphoreSlim semaphoreSlim;
        public IEventAggregator EventAggregator;
        #endregion
    }
}
