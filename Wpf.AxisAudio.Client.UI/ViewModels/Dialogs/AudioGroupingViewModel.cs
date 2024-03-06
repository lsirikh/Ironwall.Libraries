using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Common;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/23/2023 9:43:42 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupingViewModel : BaseViewModel
    {

        #region - Ctors -
        public AudioGroupingViewModel(IEventAggregator eventAggregator
                                        , AudioGroupViewModelProvider audioGroupViewModelProvider
                                        , AudioViewModelProvider audioViewModelProvider
                                        , AudioDbService audioDbService
                                        , AudioSymbolViewModelProvider audioSymbolProvider) 
                                        : base(eventAggregator)
        {
            AudioGroupViewModelProvider = audioGroupViewModelProvider;
            AudioViewModelProvider = audioViewModelProvider;
            _audioDbService = audioDbService;
            _audioSymbolProvider = audioSymbolProvider;
            GroupedAudioViewModelProvider = new ObservableCollection<AudioViewModel>();
            //UngroupedAudioViewModelProvider = new ObservableCollection<AudioViewModel>();
            SelectedAudioViewModelProvider = new ObservableCollection<AudioViewModel>();
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            FetchGroupedAudioViewModels(SelectedAudioGroup);
            IsEnable = true;
            return base.OnActivateAsync(cancellationToken);
        }

        private void FetchGroupedAudioViewModels(AudioGroupViewModel selectedAudioGroup)
        {
            if (selectedAudioGroup != null)
            {
                GroupedAudioViewModelProvider.Clear();
                foreach (var item in AudioGroupViewModelProvider
                                    .Where(entity => entity == selectedAudioGroup)
                                    .ToList())
                {
                    foreach (var model in item.AudioModels)
                    {
                        var viewModel = AudioViewModelProvider.Where(entity => entity.Model == model).FirstOrDefault();
                        if (viewModel == null) continue;

                        if (!GroupedAudioViewModelProvider.Contains(viewModel))
                            GroupedAudioViewModelProvider.Add(viewModel);
                    }
                }
            } 
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            GroupedAudioViewModelProvider.Clear();
            SelectedAudioGroup = null;
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;

            var selectedGroup = comboBox.SelectedValue as AudioGroupViewModel;
            if (selectedGroup != null) 
            { 
                SelectedAudioGroup = selectedGroup;
                FetchGroupedAudioViewModels(SelectedAudioGroup);
                
            }
            _listView?.SelectedItems?.Clear();
        }

        public void AudioSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListView listView)) return;

            if(_listView != listView)
                _listView?.SelectedItems?.Clear();

            _listView = listView;

            SelectedAudioViewModelProvider.Clear();
            foreach (var item in listView?.SelectedItems.OfType<AudioViewModel>().ToList())
            {
                SelectedAudioViewModelProvider.Add(item);
            }
        }

        public Task ClickSaveGroupWithDevice()
        {
            return Task.Run(async () =>
            {
                try
                {
                    IsEnable = false;
                    await _audioDbService.InsertAudioGroupModel();
                    await _audioDbService.InsertAudioModel();
                    await Task.Delay(500);
                    IsEnable = true;
                }
                catch (Exception)
                {

                }
            });
        }

        public void ClickfromRighttoLeft()
        {
            if (SelectedAudioGroup == null) return;

            foreach (AudioViewModel item in SelectedAudioViewModelProvider)
            {
                if(SelectedAudioGroup.AudioModels.Contains(item.Model as AudioModel))
                {
                    SelectedAudioGroup.AudioModels.Remove(item.Model as AudioModel);
                    item.Groups.Remove(SelectedAudioGroup.Model as AudioGroupBaseModel);
                }
            }

            FetchGroupedAudioViewModels(SelectedAudioGroup);
            _listView?.SelectedItems?.Clear();
        }

        public void ClickfromLefttoRight()
        {
            if (SelectedAudioGroup == null) return;

            foreach (AudioViewModel item in SelectedAudioViewModelProvider)
            {
                if(!SelectedAudioGroup.AudioModels.Contains(item.Model as AudioModel))
                {
                    SelectedAudioGroup.AudioModels.Add(item.Model as AudioModel);
                    item.Groups.Add(SelectedAudioGroup.Model as AudioGroupBaseModel);
                }
            }
            FetchGroupedAudioViewModels(SelectedAudioGroup);
            _listView?.SelectedItems?.Clear();
        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public AudioGroupViewModel SelectedAudioGroup
        {
            get { return _selectedAudioGroup; }
            set 
            { 
                _selectedAudioGroup = value; 
                NotifyOfPropertyChange(() => SelectedAudioGroup);
            }
        }

        public AudioViewModelProvider AudioViewModelProvider { get; }
        public AudioGroupViewModelProvider AudioGroupViewModelProvider { get; }
        public ObservableCollection<AudioViewModel> GroupedAudioViewModelProvider { get; private set; }
        public ObservableCollection<AudioViewModel> SelectedAudioViewModelProvider { get; private set; }


        public bool IsEnable
        {
            get { return _isEnable; }
            set 
            { 
                _isEnable = value; 
                NotifyOfPropertyChange(() => IsEnable);
            }
        }

        #endregion
        #region - Attributes -
        private AudioDbService _audioDbService;
        private AudioSymbolViewModelProvider _audioSymbolProvider;
        private AudioGroupViewModel _selectedAudioGroup;
        private ListView _listView;
        private bool _isEnable;
        #endregion
    }
}
