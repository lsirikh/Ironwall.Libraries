using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Common.Models;
using Ironwall.Libraries.Base.Services;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/5/2024 7:40:42 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MicStreamingViewModel : BaseViewModel
    {

        #region - Ctors -
        public MicStreamingViewModel(IEventAggregator eventAggregator
                                    , ILogService log
                                    , AudioGroupViewModelProvider audioGroupViewModelProvider
                                    , AudioSymbolViewModelProvider audioSymbolProvider
                                    , AxisApiService axisApiService)
                                    : base(eventAggregator, log)
        {
            AudioGroupViewModelProvider = audioGroupViewModelProvider;
            _audioSymbolProvider = audioSymbolProvider;
            _axisApiService = axisApiService;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            
            await base.OnActivateAsync(cancellationToken);
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsPlay)
            {
                await _axisApiService.ProxyStreamingGroupRequest(SelectedGroupModel, false);
            }
            IsPlay = false;
            SelectedGroupModel = null;
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        public async void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.AddedItems.OfType<AudioGroupModel>().FirstOrDefault() is AudioGroupModel groupModel)) return;
            if (SelectedGroupModel != null && SelectedGroupModel == groupModel) return;

            if (IsPlay)
            {
                await _axisApiService.ProxyStreamingGroupRequest(SelectedGroupModel, false);
                IsPlay = false;
            }

            SelectedGroupModel = groupModel;

            _audioSymbolProvider.OfType<AudioSymbolViewModel>().ToList().ForEach(entity =>
            {
                entity.GroupSelection = false;
            });

            foreach (var item in SelectedGroupModel.AudioModels)
            {

                var symbol = _audioSymbolProvider.Where(entity => entity.NameDevice == item.DeviceName).OfType<AudioSymbolViewModel>().FirstOrDefault();
                symbol.GroupSelection = true;
            }
        }
        #endregion
        #region - Processes -
        public async void ClickToPlay()
        {
            if (SelectedGroupModel == null) return;

            await _axisApiService.ProxyStreamingGroupRequest(SelectedGroupModel, true);
            IsPlay = true;
        }

        public async void ClickToStop()
        {
            if (SelectedGroupModel == null) return;

            await _axisApiService.ProxyStreamingGroupRequest(SelectedGroupModel, false);
            IsPlay = false;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        private bool _isPlay;
        public bool IsPlay
        {
            get { return _isPlay; }
            set
            {
                _isPlay = value;
                NotifyOfPropertyChange(() => IsPlay);
            }
        }


        private AudioViewModel _audioViewModel;

        public AudioViewModel AudioViewModel
        {
            get { return _audioViewModel; }
            set
            {
                _audioViewModel = value;
                NotifyOfPropertyChange(() => AudioViewModel);
            }
        }
        public AudioGroupViewModelProvider AudioGroupViewModelProvider { get; }
        public AudioGroupModel SelectedGroupModel { get; private set; }
        #endregion
        #region - Attributes -
        private AudioSymbolViewModelProvider _audioSymbolProvider;
        private AxisApiService _axisApiService;
        #endregion
    }
}
