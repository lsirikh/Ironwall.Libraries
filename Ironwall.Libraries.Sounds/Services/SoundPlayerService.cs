using NAudio.Wave;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;
using Ironwall.Libraries.Sounds.Models;
using System.Reflection;
using System.IO;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Sounds.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/10/2023 10:41:29 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoundPlayerService : IDisposable
    {

        #region - Ctors -
        public SoundPlayerService(ILogService log)
        {
            _object = new object();
            _log = log;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void SoundFileSetup(SoundModel model)
        {
            var file = GetSoundFile(model);

            //_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (_outputDevice == null)
            {
                _outputDevice = new WaveOutEvent();
                _outputDevice.PlaybackStopped += PlaybackStopped;
            }

            if (_outputDevice.PlaybackState == PlaybackState.Paused) 
                return;
            else if (_outputDevice.PlaybackState == PlaybackState.Playing)
                _outputDevice.Stop();

            _audioFileReader = new AudioFileReader(file);

            //_outputDevice?.Init(_audioFileReader);

            var loopStream = new LoopStream(_audioFileReader); // Create the loop stream

            _outputDevice?.Init(loopStream); // Use the loop stream instead of the audio file reader
        }

        private void PlaybackStopped(object sender, StoppedEventArgs e)
        {
            _log.Info("Playing effect sound was stopped!");
        }

        private string GetSoundFile(SoundModel model)
        {
            try
            {
                //get the current assembly
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var loc = $"{assembly.GetName().Name}.Resources.{model.Category}.{model.File}";

                string currentLocation = Assembly.GetExecutingAssembly().Location;
                string currentDirectory = Path.GetDirectoryName(currentLocation);
                string folderPath = Path.Combine(currentDirectory, $"Resources\\{model.Category}");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    //Debug.WriteLine($"Folder created at: {folderPath}");
                }
                else
                {
                    //Debug.WriteLine($"Folder already exists at: {folderPath}");
                }

                string targetFilePath = Path.Combine(folderPath, model.File);
               

                return targetFilePath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task PlayTaskAsync(SoundModel model, int timeDelta = default)
        {
            return Task.Run(async () => 
            {
                try
                {
                    if (_cts == null || _cts.IsCancellationRequested)
                        _cts = new CancellationTokenSource();
                    else
                    {
                        _cts.Cancel();
                        await Task.Delay(80);
                        _cts = new CancellationTokenSource();
                    }

                    if (_outputDevice == null)
                    {
                        SoundFileSetup(model);

                        _outputDevice?.Play();
                        Volume = Math.Round((decimal)(_audioFileReader.Volume * 100), 0);
                        UpdateVolumeEvent?.Invoke();
                    }

                    var startTime = DateTime.Now;

                    while (true)
                    {

                        await Task.Delay(40, _cts.Token);
                        
                        if(startTime + TimeSpan.FromSeconds((double)timeDelta) < DateTime.Now)
                            throw new TaskCanceledException();


                        if (_cts != null && _cts.IsCancellationRequested)
                            throw new TaskCanceledException();


                        switch (_outputDevice?.PlaybackState)
                        {
                            case PlaybackState.Stopped:
                                {
                                    throw new TaskCanceledException();
                                }
                            case PlaybackState.Playing:
                                {
                                    model.IsPlaying = true;

                                    UpdateStatusEvent?.Invoke((int)PlaybackState.Playing);
                                    UpdatePlayEvent?.Invoke();
                                }
                                break;
                            case PlaybackState.Paused:
                                {
                                    model.IsPlaying = false;
                                    UpdateStatusEvent?.Invoke((int)PlaybackState.Paused);
                                }
                                break;
                            default:
                                break;
                        }
                        
                        
                    }
                }
                catch (TaskCanceledException)
                {
                    _outputDevice?.Stop();
                    Dispose();
                    model.IsPlaying = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(PlayTaskAsync)}({nameof(SoundPlayerService)}) : {ex.Message}");
                }
            });

        }

        public async void Play(SoundModel model, int expTime = default)
        {
            if (_outputDevice == null)
                await PlayTaskAsync(model, expTime).ConfigureAwait(false);
            else
            {
                if (_outputDevice.PlaybackState == PlaybackState.Paused)
                    _outputDevice.Play();
                else if (_outputDevice.PlaybackState == PlaybackState.Playing)
                    await PlayTaskAsync(model, expTime).ConfigureAwait(false);
                else
                    return;
            } 
        }       

        public void Pause()
        {
            if (_outputDevice != null)
                _outputDevice.Pause();
        }


        public void IncreaseVolume()
        {
            if (_audioFileReader == null) return;

            _audioFileReader.Volume = Math.Min(1.0f, _audioFileReader.Volume + 0.1f);
            //Volume = Math.Round((decimal)(_audioFileReader.Volume * 100), 0);
            UpdateVolumeEvent?.Invoke();
        }

        public void DecreaseVolume()
        {
            if (_audioFileReader == null) return;

            _audioFileReader.Volume = Math.Max(0.0f, _audioFileReader.Volume - 0.1f);
            //Volume = Math.Round((decimal)(_audioFileReader.Volume * 100), 0);
            UpdateVolumeEvent?.Invoke();
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        public void Dispose()
        {
            if(_outputDevice != null)
            {
                _outputDevice.PlaybackStopped -= PlaybackStopped;
                _outputDevice?.Dispose();
                _outputDevice = null;
            }

            if(_audioFileReader != null)
            {
                _audioFileReader?.Dispose();
                _audioFileReader = null;
            }

            UpdateStatusEvent?.Invoke((int)PlaybackState.Stopped);
            GC.Collect();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public TimeSpan? CurrentPosition
        {
            get { return _audioFileReader?.CurrentTime; }
            set
            {
                if (_audioFileReader != null && value.HasValue)
                {
                    _audioFileReader.CurrentTime = value.Value;
                }
            }
        }

        public TimeSpan? TotalLength
        {
            get { return _audioFileReader?.TotalTime; }
        }



        public float? SoundVolume
        {
            get { return _audioFileReader?.Volume; }
            set 
            {
                if (_audioFileReader != null && value.HasValue)
                {
                    _audioFileReader.Volume = value.Value;
                }
            }
        }


        //private SoundModel _model;

        //public SoundModel Model
        //{
        //    get { return _model; }
        //    set 
        //    {
        //        _model = value; 
        //    }
        //}


        public decimal Volume { get; set; }
        #endregion
        #region - Attributes -
        private WaveOutEvent _outputDevice;
        private AudioFileReader _audioFileReader;
        private CancellationTokenSource _cts;
        private object _object;
        private ILogService _log;

        public delegate void UpdateVolume();
        public delegate void UpdateCurrent();
        public delegate void UpdateStatus(int status);
        public event UpdateVolume UpdateVolumeEvent;
        public event UpdateCurrent UpdatePlayEvent;
        public event UpdateStatus UpdateStatusEvent;
        #endregion
    }
}
