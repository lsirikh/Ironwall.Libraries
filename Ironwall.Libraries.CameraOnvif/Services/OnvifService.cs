using Mictlanix.DotNet.Onvif.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.CameraOnvif.Utils;
using System.Linq;

namespace Ironwall.Libraries.CameraOnvif.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 9:53:59 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifService
    {
        #region - Ctors -
        public OnvifService()
        {

        }
        #endregion
        #region - Implementation of Interface -
        public async Task Initialize(string ipAddress, int port, string username, string password)
        {
            if (ipAddress == null || port == 0 || username == null || password == null) return;

            _onvifControl.SendLog += OnvifControl_SendLog;
            await Connect(ipAddress, port, username, password);
        }

        public async void Uninitialize()
        {
            try
            {
                if (cts != null)
                {
                    if (!cts.IsCancellationRequested) cts?.Cancel();
                    cts?.Dispose();
                }

                if (cts_move != null)
                {
                    if (!cts_move.IsCancellationRequested) cts_move?.Cancel();
                    cts_move?.Dispose();
                }

                if (cts_preset != null)
                {
                    if (!cts_preset.IsCancellationRequested) cts_preset?.Cancel();
                    cts_preset?.Dispose();
                }
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine($"Tokens was cancelled : " + ex.ToString());
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine($"Tokens was not initialized : " + ex.ToString());
            }

            await Task.Delay(100);

            _onvifControl.SendLog -= OnvifControl_SendLog;
        }
        #endregion
        #region - Overrides -
        public async Task Connect(string ipAddress, int port, string username, string password)
        {
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts?.Cancel();
                    await Task.Delay(50);
                    Debug.WriteLine($"Task was cancelled in {nameof(Connect)}");
                    cts?.Dispose();
                }

                var host = ipAddress + ":" + port;
                cts = new CancellationTokenSource();
                await _onvifControl.DeviceReady(host, username, password, cts.Token);
                await _onvifControl.CreateProfile(cts.Token);
                await _onvifControl.CreateVideoSource(cts.Token);

                connectFinishCallback?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Connect)} : " + ex.ToString());
            }

        }

        public async void TestOnvif(string ipAddress, int port, string username, string password)
        {

            try
            {
                if (cts != null)
                {
                    if (!cts.IsCancellationRequested) cts.Cancel();
                    await Task.Delay(50);
                }

                var host = ipAddress + ":" + port;
                cts = new CancellationTokenSource();
                await _onvifControl.MainAsync(host, username, password, cts.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(TestOnvif)} : " + ex.ToString());
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void OnvifControl_SendLog(string data)
        {
            ///////////////////////////////
            /// Log와 연동 시킬 방안을 확인
            ///////////////////////////////
            Debug.WriteLine(data);
        }

        public async void UpLeftButtonPressed()
        {
            Debug.WriteLine(nameof(UpLeftButtonPressed));

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(11, cts_move.Token);

            }
            catch { }
        }

        public async void UpButtonPressed()
        {
            Debug.WriteLine("UpButtonPressed");

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(1, cts_move.Token);

            }
            catch { }
        }

        public async void UpRightButtonPressed()
        {
            Debug.WriteLine(nameof(UpRightButtonPressed));

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(12, cts_move.Token);

            }
            catch { }
        }

        public async void DownLeftButtonPressed()
        {
            Debug.WriteLine(nameof(DownLeftButtonPressed));
            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(21, cts_move.Token);

            }
            catch { }
        }

        public async void DownButtonPressed()
        {
            Debug.WriteLine("DownButtonPressed");
            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(2, cts_move.Token);

            }
            catch { }
        }

        public async void DownRightButtonPressed()
        {
            Debug.WriteLine(nameof(DownRightButtonPressed));
            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(22, cts_move.Token);

            }
            catch { }
        }

        public async void LeftButtonPressed()
        {
            Debug.WriteLine("LeftButtonPressed");

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(3, cts_move.Token);

            }
            catch { }
        }

        public async void RightButtonPressed()
        {
            Debug.WriteLine("RightButtonPressed");

            if (_onvifControl.Profile_token == null)
                return;

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(4, cts_move.Token);

            }
            catch { }
        }

        public async void ZoomInButtonPressed()
        {
            Debug.WriteLine("ZoomInButtonPressed");


            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(5, cts_move.Token);

            }
            catch { }
        }

        public async void ZoomOutButtonPressed()
        {
            Debug.WriteLine("ZoomOutButtonPressed");


            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.MovePtz(6, cts_move.Token);

            }
            catch { }
        }

        public async void FocusInButtonPressed()
        {
            Debug.WriteLine("FocusInButtonPressed");


            try
            {
                //if (cts_move != null
                //    && !cts_move.IsCancellationRequested)
                //    cts_move.Cancel();

                cts_move = new CancellationTokenSource();
                await _onvifControl.FocusInOut(true, cts_move.Token);

            }
            catch { }
        }

        public async void FocusOutButtonPressed()
        {
            Debug.WriteLine("FocusOutButtonPressed");

            try
            {
                //if (cts_move != null
                //    && !cts_move.IsCancellationRequested)
                //    cts_move.Cancel();
                cts_move = new CancellationTokenSource();
                await _onvifControl.FocusInOut(false, cts_move.Token);

            }
            catch { }
        }

        public async void FocusNormalButtonPressed()
        {
            Debug.WriteLine("FocusNormalButtonPressed");

            try
            {
                cts_move = new CancellationTokenSource();
                await _onvifControl.FocusNormal(cts_move.Token);

            }
            catch { }
        }


        public void ButtonReleased()
        {
            Debug.WriteLine("ButtonReleased");
            cts_move?.Cancel();
        }

        public async Task GetPreset()
        {

            try
            {
                if (cts_preset != null)
                {
                    if (!cts_preset.IsCancellationRequested) cts_preset.Cancel();
                    await Task.Delay(50);
                }

                cts_preset = new CancellationTokenSource();
                PtzPresets = new ObservableCollection<PTZPreset>();

                if (_onvifControl.Ptz == null) throw new NullValueException($"PTZ was not supported!");

                var ret = await _onvifControl.GetPresets(cts_preset.Token);

                if (ret == null) throw new NullValueException($"Preset was null!");

                presetFinishCallback?.Invoke(this, new PresetEventArgs(PtzPresets));
            }
            catch (NullValueException ex)
            {
                Debug.WriteLine($"Raised {nameof(NullValueException)} in {nameof(GetPreset)} : " + ex.ToString());
            }
            catch
            {
            }
        }

        public async void GotoPresetButton()
        {
            try
            {
                if (cts_preset != null)
                {
                    if (!cts_preset.IsCancellationRequested) cts_preset.Cancel();
                    await Task.Delay(50);
                }

                cts_preset = new CancellationTokenSource();
                await _onvifControl.GotoPreset(SelectedPreset, cts_preset.Token);
            }
            catch { }
        }

        public void ClearPresetButton()
        {
            SelectedPreset = null;
            PtzPresets?.Clear();
        }

        public void SetPreset(string name)
        {
            try
            {
                SelectedPreset = PtzPresets?.Where(entity => entity.Name == name).FirstOrDefault();
            }
            catch
            {
            }

        }

        public PTZPreset SelectedPreset { get; set; }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<PTZPreset> PtzPresets { get; private set; }
        #endregion
        #region - Attributes -
        private OnvifControl _onvifControl = new OnvifControl();
        public CancellationTokenSource cts;
        public CancellationTokenSource cts_move;
        public CancellationTokenSource cts_preset;

        public event EventHandler connectFinishCallback;
        public event EventHandler presetFinishCallback;
        #endregion
    }
}
