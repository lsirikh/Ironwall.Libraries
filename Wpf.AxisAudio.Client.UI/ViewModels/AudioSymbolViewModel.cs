using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using System;
using Wpf.AxisAudio.Client.UI.Models;
using Wpf.AxisAudio.Common.Enums;
using System.Net.NetworkInformation;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using System.Linq;
using Ironwall.Libraries.Base.Services;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/8/2023 11:16:18 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSymbolViewModel : EntityViewModel
    {

        #region - Ctors -
        public AudioSymbolViewModel()
        {
        }

        public AudioSymbolViewModel(AudioSymbolModel entityModel, IEventAggregator eventAggregator) : base(entityModel)
        {
            DisplayName = nameof(AudioSymbolViewModel);
            EventAggregator = eventAggregator;
            
            _audioProvider = IoC.Get<AudioViewModelProvider>();
            _audioGroupProvider = IoC.Get<AudioGroupViewModelProvider>();
            _apiService = IoC.Get<AxisApiService>();
            _log = IoC.Get<ILogService>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void ChangeMode(int mode)
        {
            switch ((EnumAxisAudioDeviceState)mode)
            {
                case EnumAxisAudioDeviceState.NONE:
                    Status = (int)EnumAxisAudioDeviceState.NONE;
                    IsStreaming = false;
                    IsClipPlay = false;
                    break;
                case EnumAxisAudioDeviceState.ACTIVATED:
                    Status = (int)EnumAxisAudioDeviceState.ACTIVATED;
                    IsStreaming = false;
                    IsClipPlay = false;
                    break;
                case EnumAxisAudioDeviceState.MIC_STREAMING:
                    Status = (int)EnumAxisAudioDeviceState.MIC_STREAMING;
                    IsStreaming = true;
                    IsClipPlay = false;
                    break;
                case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING:
                    Status = (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING;
                    IsStreaming = false;
                    IsClipPlay = true;
                    break;
                case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING:
                    Status = (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING;
                    IsStreaming = true;
                    IsClipPlay = true;
                    break;
                default:
                    Status = (int)EnumAxisAudioDeviceState.NONE;
                    IsStreaming = false;
                    IsClipPlay = false;
                    break;
            }
        }

        public async void OnClickStreaming(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is MenuItem menu)) return;

                if (Status == (int)EnumAxisAudioDeviceState.NONE) return;

                

                if (!IsStreaming)
                {
                    //await EventAggregator.PublishOnUIThreadAsync(new RequestSingleSpeakerStreamingMessageModel(NameDevice, true));
                    await _apiService.ProxyStreamingRequest(NameDevice, true);
                }
                else
                {
                    await _apiService.ProxyStreamingRequest(NameDevice, false);
                    //await EventAggregator.PublishOnUIThreadAsync(new RequestSingleSpeakerStreamingMessageModel(NameDevice, false));
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnClickStreaming)} of {nameof(AudioSymbolViewModel)} : {ex}");
            }
        }

        //public async void OnClickGroupStreaming(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!(sender is MenuItem menu)) return;

        //        if (Status == (int)EnumAxisAudioDeviceState.NONE) return;

        //        if (!IsStreaming)
        //        {
        //            await _apiService.ProxyStreamingGroupRequest(NameDevice, true);
        //            //await EventAggregator.PublishOnUIThreadAsync(new RequestGroupSpeakerStreamingMessageModel(NameDevice, true));
        //        }
        //        else
        //        {
        //            await _apiService.ProxyStreamingGroupRequest(NameDevice, false);
        //            //await EventAggregator.PublishOnUIThreadAsync(new RequestGroupSpeakerStreamingMessageModel(NameDevice, false));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //}

        public async void OnClickClipPlaying(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is MenuItem menu)) return;

                if (Status == (int)EnumAxisAudioDeviceState.NONE) return;

                if (!IsClipPlay)
                {
                    await _apiService.ApiPlayClipRequest(NameDevice);
                }
                else
                {
                    await _apiService.ApiStopClipRequest(NameDevice);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnClickClipPlaying)} of {nameof(AudioSymbolViewModel)} : {ex}");
            }
        }

        //public async void OnClickGroupClipPlaying(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!(sender is MenuItem menu)) return;

        //        if (Status == (int)EnumAxisAudioDeviceState.NONE) return;

        //        if (!IsClipPlay)
        //        {
        //            await _apiService.ApiPlayClipGroupRequest(NameDevice);
        //        }
        //        else
        //        {
        //            await _apiService.ApiStopClipGroupRequest(NameDevice);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //}

        public async void OnClickPlaySetting(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is MenuItem item)) return;

                var viewModel = _audioProvider.Where(entity => entity.DeviceName == NameDevice).FirstOrDefault();
                await EventAggregator.PublishOnUIThreadAsync(new OpenAudioPlayDialogMessageModel(viewModel));

            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnClickPlaySetting)} of {nameof(AudioSymbolViewModel)} : {ex}");
            }
        }
        
        public async void OnClickGroupSetting(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is MenuItem item)) return;

                var viewModel = _audioProvider.Where(entity => entity.DeviceName == NameDevice).FirstOrDefault();
                var groupViewModel = _audioGroupProvider.Where(entity => viewModel.Groups.Any(innerEntity => innerEntity.GroupNumber == entity.GroupNumber)).FirstOrDefault();
                await EventAggregator.PublishOnUIThreadAsync(new OpenAudioGroupingDialogMessageModel(groupViewModel));
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnClickGroupSetting)} of {nameof(AudioSymbolViewModel)} : {ex}");
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsStreaming
        {
            get { return _isStreaming; }
            set
            {
                _isStreaming = value;
                NotifyOfPropertyChange(() => IsStreaming);
            }
        }


        public bool IsClipPlay
        {
            get { return _isClipPlay; }
            set
            {
                _isClipPlay = value;
                NotifyOfPropertyChange(() => IsClipPlay);
            }
        }


        public bool IsClipPlayGroup
        {
            get { return _isClipPlayGroup; }
            set
            {
                _isClipPlayGroup = value;
                NotifyOfPropertyChange(() => IsClipPlayGroup);
            }
        }

        public int IsStreamingGroup
        {
            get { return _isStreamigGroup; }
            set
            {
                _isStreamigGroup = value;
                NotifyOfPropertyChange(() => IsStreamingGroup);
            }
        }

        public int Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }


        public bool GroupSelection
        {
            get { return _groupSelection; }
            set 
            { 
                _groupSelection = value;
                NotifyOfPropertyChange(() => GroupSelection);
            }
        }


        #endregion
        #region - Attributes -
        private int _status;
        private bool _isClipPlay;
        private bool _isStreaming;
        private int _isStreamigGroup;
        private bool _isClipPlayGroup;
        private bool _groupSelection;
        private AxisApiService _apiService;
        private ILogService _log;
        private readonly AudioViewModelProvider _audioProvider;
        private readonly AudioGroupViewModelProvider _audioGroupProvider;
        #endregion
    }
}
