using Caliburn.Micro;
using Ironwall.Libraries.Sounds.Models;
using Ironwall.Libraries.Sounds.Services;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace Ironwall.Libraries.Sound.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/11/2023 8:21:00 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoundViewModel : UIBaseViewModel
    {
        #region - Ctors -
        public SoundViewModel(IEventAggregator eventAggregator
                            , SoundPlayerService soundPlayerService
                            , SoundSetupModel setupModel)
                            : base(eventAggregator)
        {
            SoundPlayerService = soundPlayerService;

            Items = new List<SoundModel>()
            {
                //new SoundModel()
                //{
                //    Id = 1,
                //    Name = "BGM1",
                //    File = "BGM1.mp3",
                //    Category = "Backgrounds",
                //    IsPlaying = false,
                //},
                //new SoundModel()
                //{
                //    Id = 2,
                //    Name = "BGM2",
                //    File = "BGM2.mp3",
                //    Category = "Backgrounds",
                //    IsPlaying = false,
                //},
                new SoundModel()
                {
                    Id = 3,
                    Name = "EventMessage_Sound",
                    File = "EventMessage_Sound.mp3",
                    Category = "Effects",
                    IsPlaying = false,
                },
                new SoundModel()
                {
                    Id = 4,
                    Name = "InputMessage_Sound",
                    File = "InputMessage_Sound.mp3",
                    Category = "Effects",
                    IsPlaying = false,
                },
                new SoundModel()
                {
                    Id = 4,
                    Name = "InputMessage2_Sound",
                    File = "InputMessage2_Sound.mp3",
                    Category = "Effects",
                    IsPlaying = false,
                },
                new SoundModel()
                {
                    Id = 4,
                    Name = "Warning_Sound",
                    File = "Warning_Sound.wav",
                    Category = "Effects",
                    IsPlaying = false,
                },
            };
            SelectedModel = Items.FirstOrDefault();
            _setupModel = setupModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            SoundPlayerService.UpdatePlayEvent += SoundPlayerService_UpdatePlayEvent;
            SoundPlayerService.UpdateVolumeEvent += SoundPlayerService_UpdateVolumeEvent;
            SoundPlayerService.UpdateStatusEvent += SoundPlayerService_UpdateStatusEvent;
            PlayStatus = true;
            return base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            SoundPlayerService.UpdatePlayEvent -= SoundPlayerService_UpdatePlayEvent;
            SoundPlayerService.UpdateVolumeEvent -= SoundPlayerService_UpdateVolumeEvent;
            SoundPlayerService.UpdateStatusEvent -= SoundPlayerService_UpdateStatusEvent;
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void ClickToPlay()
        {
            if (!_setupModel.IsSound) return;

            if (SelectedModel != null)
            {
                //PlayStatus = false;
                SoundPlayerService.Play(SelectedModel, _setupModel.DiscardTime);
            }
        }

        public void ClickToPause()
        {
            //PlayStatus = true;
            SoundPlayerService.Pause();
        }

        public void ClickToStop()
        {
            //PlayStatus = true;
            SoundPlayerService.Stop();
        }

        public void ClickToVolumeUp()
        {
            SoundPlayerService.IncreaseVolume();
        }
        public void ClickToVolumeDown()
        {
            SoundPlayerService.DecreaseVolume();
        }

        private void SoundPlayerService_UpdateVolumeEvent()
        {
            Refresh();
        }

        private void SoundPlayerService_UpdatePlayEvent()
        {
            Refresh();
        }

        private void SoundPlayerService_UpdateStatusEvent(int status)
        {
            switch (status)
            {
                case 0:
                    if (PlayStatus == true)
                        return;
                    PlayStatus = true;
                    PlayProgress = 100;
                    NotifyOfPropertyChange(() => PlayTime);
                    NotifyOfPropertyChange(() => Volume);
                    break;
                case 1:
                    if (PlayStatus == false)
                        return;
                    PlayStatus = false;
                    break;
                case 2:
                    if (PlayStatus == true)
                        return;
                    PlayStatus = true;
                    break;
                default:
                    break;
            }
        }

        public void OnVolumeChanged(object sender, MouseButtonEventArgs e)
        {
            var progressBar = (ProgressBar)e.Source;
            Point clickPoint = e.GetPosition(progressBar);
            double newVolumeValue = clickPoint.X / progressBar.ActualWidth * progressBar.Maximum;

            // Update the view setupModel's Volume
            Volume = Math.Round((float)(newVolumeValue / 100), 1);
        }


        public void OnPlayPositionChanged(object sender, MouseButtonEventArgs e)
        {
            var progressBar = (ProgressBar)e.Source;
            Point clickPoint = e.GetPosition(progressBar);
            double newProgressValue = clickPoint.X / progressBar.ActualWidth * progressBar.Maximum;

            PlayProgress = newProgressValue;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        private bool _playStatus;

        public bool PlayStatus
        {
            get { return _playStatus; }
            set
            {
                _playStatus = value;
                NotifyOfPropertyChange(() => PlayStatus);
            }
        }

        private double? _volume;
        public double? Volume
        {
            get { return GetCurrenVolume(); }
            set
            {
                if (SoundPlayerService.SoundVolume.HasValue)
                {
                    SoundPlayerService.SoundVolume = (float)value;
                }
                _volume = value;
                NotifyOfPropertyChange(() => Volume);
            }
        }

        private double GetCurrenVolume()
        {
            if (SoundPlayerService.SoundVolume.HasValue)
            {
                return Math.Round((double)(SoundPlayerService.SoundVolume * 100), 1);
            }
            else
            {
                return 0d;
            }
        }

        private double? _playProgress;
        public double? PlayProgress
        {
            get { return GetCurrentProgress(); }
            set
            {
                if (SoundPlayerService.CurrentPosition.HasValue)
                {
                    // Calculate the TimeSpan corresponding to the new progress value
                    double? newProgressRatio = value / 100;
                    TimeSpan? newPosition = TimeSpan.FromSeconds((double)(SoundPlayerService?.TotalLength?.TotalSeconds * newProgressRatio == null ? 0d : SoundPlayerService?.TotalLength?.TotalSeconds * newProgressRatio));

                    // Set the SoundPlayerService CurrentPosition
                    SoundPlayerService.CurrentPosition = newPosition;

                }
                _playProgress = value;
                NotifyOfPropertyChange(() => PlayProgress);
            }
        }

        private double? GetCurrentProgress()
        {
            if (SoundPlayerService.CurrentPosition.HasValue && SoundPlayerService.TotalLength.HasValue)
            {
                return Math.Round((double)((SoundPlayerService?.CurrentPosition?.TotalSeconds / SoundPlayerService?.TotalLength?.TotalSeconds) * 100), 1);
            }
            else
            {
                return 0d;
            }
        }

        public string PlayTime
        {
            get { return GetCurrentTime(); }
        }

        private string GetCurrentTime()
        {
            return $"{SoundPlayerService?.CurrentPosition?.ToString(@"mm\:ss\.f")}/{SoundPlayerService?.TotalLength?.ToString(@"mm\:ss\.f")}";
        }

        private SoundModel _selectedModel;

        public SoundModel SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                NotifyOfPropertyChange(() => SelectedModel);
            }
        }

        private SoundSetupModel _setupModel;

        public IEnumerable<SoundModel> Items { get; set; }
        public SoundPlayerService SoundPlayerService { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
