using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Wpf.AxisAudio.Client.UI.Models;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.Services;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/18/2023 2:43:31 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioGroupViewModel : AudioBaseViewModel<IAudioGroupModel>
    {
        
        #region - Ctors -
        public AudioGroupViewModel(IAudioGroupModel model) :base(model)
        { 
            _eventAggregator = IoC.Get<IEventAggregator>();
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
        public async void OnClickGroupSetting(object sender, EventArgs e)
        {
            try
            {
                if (!(sender is MenuItem item)) return;
                AudioGroupViewModel viewModel = item.DataContext as AudioGroupViewModel;
                var viewModelProvider = IoC.Get<AudioGroupViewModelProvider>();
                var viewModelFromProvider = viewModelProvider.Where(entity => entity.GroupName == viewModel.GroupName).FirstOrDefault();
                
                await _eventAggregator.PublishOnUIThreadAsync(new OpenAudioGroupingDialogMessageModel(viewModelFromProvider));
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnClickGroupSetting)} of {nameof(AudioGroupViewModel)} : {ex}", true);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int GroupNumber
        {
            get { return _model.GroupNumber; }
            set 
            { 
                _model.GroupNumber = value;
                NotifyOfPropertyChange(nameof(GroupNumber));
            }
        }

        public string GroupName
        {
            get { return _model.GroupName; }
            set 
            { 
                _model.GroupName = value; 
                NotifyOfPropertyChange(nameof(GroupName));

            }
        }
        public ObservableCollection<AudioSensorModel> AudioSensorModels => _model?.SensorModels;
        public ObservableCollection<AudioModel> AudioModels => _model?.AudioModels;
        #endregion
        #region - Attributes -
        private ILogService _log;
        #endregion
    }
}
