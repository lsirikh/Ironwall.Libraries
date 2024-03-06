using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using System.Linq;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/5/2024 5:58:47 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioPlayViewModel : BaseViewModel
    {

        #region - Ctors -
        public AudioPlayViewModel(IEventAggregator eventAggregator
                                , AudioClipPlayViewModel audioClipPlayViewModel
                                , MicStreamingViewModel micStreamingViewModel
                                , AudioSymbolViewModelProvider audioSymbolProvider)       
                                : base(eventAggregator)
        {
            AudioClipPlayViewModel = audioClipPlayViewModel;
            MicStreamingViewModel = micStreamingViewModel;
            _audioSymbolProvider = audioSymbolProvider;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            AudioClipPlayViewModel.AudioViewModel = AudioViewModel;
            MicStreamingViewModel.AudioViewModel = AudioViewModel;

            await AudioClipPlayViewModel.ActivateAsync();
            await MicStreamingViewModel.ActivateAsync();
            await base.OnActivateAsync(cancellationToken);
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            AudioClipPlayViewModel.AudioViewModel = null;
            MicStreamingViewModel.AudioViewModel = null;

            _audioSymbolProvider.OfType<AudioSymbolViewModel>().ToList().ForEach(entity => 
            {
                entity.GroupSelection = false;
            });

            await AudioClipPlayViewModel.DeactivateAsync(true);
            await MicStreamingViewModel.DeactivateAsync(true);
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        
        public AudioViewModel AudioViewModel { get; set; }
        public AudioClipPlayViewModel AudioClipPlayViewModel { get; }
        public MicStreamingViewModel MicStreamingViewModel { get; }

        private AudioSymbolViewModelProvider _audioSymbolProvider;
        #endregion
        #region - Attributes -
        #endregion
    }
}
