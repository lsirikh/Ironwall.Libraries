using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.UI.ViewModels;
using Caliburn.Micro;
using System.Linq;

namespace Wpf.Libraries.Surv.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 2:09:05 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvCameraViewModelProvider
    : EntityCollectionProvider<SurvCameraViewModel>, ILoadable
    {

        #region - Ctors -
        public SurvCameraViewModelProvider(SurvCameraModelProvider provider)
        {
            _provider = provider;
            _provider.CollectionEntity.CollectionChanged += CollectionEntity_CollectionChanged;
        }


        #endregion
        #region - Implementation of Interface -
        public Task<bool> Initialize(CancellationToken token = default)
        {
            try
            {
                Clear();
                foreach (var item in _provider)
                {
                    var viewModel = new SurvCameraViewModel(item);
                    Add(viewModel);
                }

                return Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Raised exception in {nameof(Initialize)} : {ex.Message} ");
                return Task.FromResult(false);
            }
        }

        public void Uninitialize()
        {
            _provider.CollectionEntity.CollectionChanged -= CollectionEntity_CollectionChanged;
            Clear();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (SurvCameraModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new SurvCameraViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvCameraModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvCameraModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (SurvCameraModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new SurvCameraViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (SurvCameraModel newItem in _provider.ToList())
                    {
                        var viewModel = new SurvCameraViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private SurvCameraModelProvider _provider;
        #endregion
    }
}
