using Caliburn.Micro;
using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.ViewModels;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/18/2023 2:48:22 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupViewModelProvider : EntityCollectionProvider<AudioGroupViewModel>, ILoadable
    {
        private AudioGroupProvider _provider;

        #region - Ctors -
        public AudioGroupViewModelProvider(AudioGroupProvider provider)
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
                    var viewModel = new AudioGroupViewModel(item);
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
                    foreach (AudioGroupModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new AudioGroupViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioGroupModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioGroupModel oldItem in e.OldItems)
                    {
                        //_groupProvider.Remove(oldItem);
                        var viewModel = CollectionEntity.Where(entity => entity.Model.Id == oldItem.Id).FirstOrDefault();
                        await viewModel.DeactivateAsync(true);
                        Remove(viewModel);
                    }
                    foreach (AudioGroupModel newItem in e.NewItems)
                    {
                        //_groupProvider.Add(newItem);
                        var viewModel = new AudioGroupViewModel(newItem);
                        await viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    CollectionEntity.Clear();
                    foreach (AudioGroupModel newItem in _provider.ToList())
                    {
                        var viewModel = new AudioGroupViewModel(newItem);
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
        #endregion

    }
}
