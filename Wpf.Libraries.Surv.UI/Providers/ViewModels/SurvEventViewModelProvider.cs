using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Maps;
using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wpf.Libraries.Surv.Common.Models;
using Wpf.Libraries.Surv.Common.Providers.Models;
using Wpf.Libraries.Surv.UI.ViewModels;

namespace Wpf.Libraries.Surv.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 2:09:23 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvEventViewModelProvider
         : EntityCollectionProvider<SurvEventViewModel>, ILoadable
    {

        #region - Ctors -
        public SurvEventViewModelProvider(SurvEventModelProvider provider)
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
                    var apiModel = GetSurvApi(item.ApiId);
                    var cameraModel = GetSurvCamera(item.CameraId);
                    
                    var viewModel = new SurvEventViewModel(item, apiModel, cameraModel);
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
        private ISurvApiModel GetSurvApi(int apiId)
        {

            var apiProvider = IoC.Get<SurvApiModelProvider>();
            return apiProvider.Where(entity => entity.Id == apiId).FirstOrDefault();
        }

        private ISurvCameraModel GetSurvCamera(int cameraId)
        {
            var cameraProvider = IoC.Get<SurvCameraModelProvider>();
            return cameraProvider.Where(entity => entity.Id == cameraId).FirstOrDefault();
        }

        private async void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (SurvEventModel newItem in e.NewItems)
                    {
                        var apiModel = GetSurvApi(newItem.ApiId);
                        var cameraModel = GetSurvCamera(newItem.CameraId);

                        var viewModel = new SurvEventViewModel(newItem, apiModel, cameraModel);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (SurvEventModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (SurvEventModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (SurvEventModel newItem in e.NewItems)
                    {
                        var apiModel = GetSurvApi(newItem.ApiId);
                        var cameraModel = GetSurvCamera(newItem.CameraId);

                        var viewModel = new SurvEventViewModel(newItem, apiModel, cameraModel);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (SurvEventModel newItem in _provider.ToList())
                    {
                        var apiModel = GetSurvApi(newItem.ApiId);
                        var cameraModel = GetSurvCamera(newItem.CameraId);

                        var viewModel = new SurvEventViewModel(newItem, apiModel, cameraModel);
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
        private SurvEventModelProvider _provider;


        #endregion
    }
}
