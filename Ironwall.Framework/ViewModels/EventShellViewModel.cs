using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public abstract class EventShellViewModel<T1, T2>
        : Conductor<IScreen>.Collection.OneActive
        where T1 : IScreen
        where T2 : IScreen
    {
        #region - Ctors -
        public EventShellViewModel(
              T1 preEventListViewModel
            , T2 postEventListViewModel
            , IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Items.Add(preEventListViewModel);
            Items.Add(postEventListViewModel);

            ActiveItem = Items[0];
        }
        #endregion

        #region - Overrides -

        protected override async void OnActivationProcessed(IScreen item, bool success)
        {
            base.OnActivationProcessed(item, success);
            foreach (IScreen data in Items)
            {
                if (data.GetType() == item.GetType())
                {
                    await data.ActivateAsync();
                    //_eventAggregator.SubscribeOnUIThread(data);
                }
                else
                {
                    await data.DeactivateAsync(true);
                    //_eventAggregator.Unsubscribe(data);
                }
            }
        }
        #endregion


        #region - Attributes -
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
