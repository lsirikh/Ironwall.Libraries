using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.ViewModels;
using Wpf.AxisAudio.Common.Models;
using Caliburn.Micro;
using System.Linq;

namespace Wpf.AxisAudio.Client.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/3/2023 1:36:43 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSensorViewModelProvider : EntityCollectionProvider<AudioSensorViewModel>, ILoadable
    {
        

        #region - Ctors -
        public AudioSensorViewModelProvider(AudioSensorProvider provider)
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
                    var viewModel = new AudioSensorViewModel(item);
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

        /// <summary>
        /// 프로그램 종료시 Uninitialize
        /// </summary>
        public void Uninitialize()
        {
            _provider.CollectionEntity.CollectionChanged -= CollectionEntity_CollectionChanged;
            Clear();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        private async void CollectionEntity_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (AudioSensorModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new AudioSensorViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioSensorModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioSensorModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (AudioSensorModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new AudioSensorViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (AudioSensorModel newItem in _provider.ToList())
                    {
                        var viewModel = new AudioSensorViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;
            }
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private AudioSensorProvider _provider;
        #endregion

    }
}
