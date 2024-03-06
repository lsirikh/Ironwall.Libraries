using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.CameraOnvif.Models;
using System.Diagnostics;
using System;
using Ironwall.Framework.Models.Devices;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.CameraOnvif.Providers;
using Ironwall.Libraries.Enums;
using Mictlanix.DotNet.Onvif.Ptz;
using Mictlanix.DotNet.Onvif.Common;
using System.Collections.Generic;

namespace Ironwall.Libraries.CameraOnvif.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 10:05:06 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PtzService
    {
        #region - Ctors -
        public PtzService(CameraDeviceProvider cameraDeviceProvider
                        , CameraMappingProvider cameraMappingProvider
                        , CameraPresetProvider cameraPresetProvider
                        , OnvifProvider onvifProvider
                        , OnvifSetupModel onvifSetupModel)
        {
            _deviceProvider = cameraDeviceProvider;
            _presetProvider = cameraPresetProvider;
            _mappingProvider = cameraMappingProvider;

            _onvifProvider = onvifProvider;

            _setupModel = onvifSetupModel;

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public ICameraMappingModel GetMappingModel(int idController, int idSensor)
        {
            try
            {
                //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(GetPresetGroup)}] was executed.");
                var selectedMapping = _mappingProvider.Where(entity => entity.Sensor.Controller.DeviceNumber == idController
                                                                && entity.Sensor.DeviceNumber == idSensor).FirstOrDefault();
                return selectedMapping;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(GetMappingModel)}({nameof(PtzService)}) : {ex.Message}");
                return null;
            }
        }

        public Task CameraPresetPtzControl(ICameraMappingModel model, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    IOnvifModel model1 = null;
                    IOnvifModel model2 = null;

                    foreach (var item in _onvifProvider
                        .Where(entity => entity.CameraDeviceModel.DeviceType == (int)EnumDeviceType.IpCamera).ToList())
                    {
                        if (item.CameraDeviceModel.Id == model.FirstPreset.ReferenceId
                        && IsModeCheck(item.CameraDeviceModel, EnumCameraMode.ONVIF))
                        {
                            model1 = item;
                        }
                        else if (item.CameraDeviceModel.Id == model.SecondPreset.ReferenceId
                        && IsModeCheck(item.CameraDeviceModel, EnumCameraMode.ONVIF))
                        {
                            model2 = item;
                        }
                    }

                    List<Task> tasks = new List<Task>();
                    if (model1?.CameraDeviceModel != null)
                    {
                        if (model1.Cts != null) model1.Cts.Cancel();
                        await Task.Delay(100);
                        model1.Cts = new CancellationTokenSource();

                        var homePreset = _presetProvider?.Where(entity => entity.ReferenceId == model1.CameraDeviceModel.Id)
                                        ?.Where(entity => entity.IsHome == true)?.FirstOrDefault();
                        tasks.Add(CamearPtzCycle(model1, model.FirstPreset, homePreset));
                    }

                    if (model2?.CameraDeviceModel != null)
                    {
                        if (model2.Cts != null) model2.Cts.Cancel();
                        await Task.Delay(100);
                        model2.Cts = new CancellationTokenSource();

                        var homePreset = _presetProvider?.Where(entity => entity.ReferenceId == model2.CameraDeviceModel.Id)
                                        ?.Where(entity => entity.IsHome == true)?.FirstOrDefault();
                        tasks.Add(CamearPtzCycle(model2, model.SecondPreset, homePreset));
                    }

                    if (tasks.Count > 0)
                        await Task.WhenAll(tasks).ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(CameraPresetPtzControl)} : {ex.Message}");
                }

            }, token);
        }

        private Task CamearPtzCycle(IOnvifModel model, ICameraPresetModel preset, ICameraPresetModel homePreset)
        {
            return Task.Run(async () =>
            {
                await CameraPresetMoveAsync(model, preset.PresetName);
                await Task.Delay(TimeSpan.FromSeconds(_setupModel.PtzTimeout), model.Cts.Token);
                if (model.Cts.IsCancellationRequested)
                {
                    Debug.WriteLine($"{model.CameraDeviceModel.DeviceName} was Cancelled!!!!!!!!!!!!!!!!!!!!!!!!!");
                    return;
                }
                await CameraPresetMoveAsync(model, homePreset.PresetName);
            });
        }

        private Task CameraPresetMoveAsync(IOnvifModel model, string preset = null)
        {
            return Task.Run(async () =>
            {
                if (model.OnvifControl == null)
                {
                    model.OnvifControl = new OnvifControl();
                    var host = model.CameraDeviceModel.IpAddress + ":" + model.CameraDeviceModel.Port;

                    await model.OnvifControl.DeviceReady(host, model.CameraDeviceModel.UserName, model.CameraDeviceModel.Password);
                    //Thread.Sleep(500);

                    await model.OnvifControl.CreateProfile();

                    if (model.PtzPresetProvider == null)
                    {
                        GetPresetsResponse presets = await model.OnvifControl.GetPresets();
                        model.PtzPresetProvider = new PtzPresetProvider();
                        foreach (var ptzInfo in presets?.Preset)
                        {
                            model.PtzPresetProvider.Add(ptzInfo);
                        }
                    }
                }

                if (preset != null)
                {
                    var ptzPreset = model.PtzPresetProvider.Where(t => t.Name == preset).FirstOrDefault();
                    await model.OnvifControl.GotoPreset(ptzPreset);

                    while (await CheckPtzMove(model, preset))
                    {
                        try
                        {
                            await Task.Delay(1000);
                        }
                        catch (TaskCanceledException ex)
                        {
                            await model.OnvifControl.StopPreset();
                            Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised TaskCanceledException : {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                        }
                    }
                }
            });
        }

        private bool IsModeCheck(ICameraDeviceModel item, EnumCameraMode onvif)
        {
            //CameraDeviceProvider에서 API로 세팅된 Camera 찾기
            return _deviceProvider.Where(entity => (entity.Id == item.Id) && (entity as ICameraDeviceModel).Mode == (int)onvif).Count() > 0 ? true : false;
        }

        private async Task<bool> CheckPtzMove(IOnvifModel item, string preset = null)
        {
            var status = await item.OnvifControl.GetPtzStatus();
            var panTiltMove = status.MoveStatus?.PanTilt;
            var zoomMove = status.MoveStatus?.Zoom;

            if (panTiltMove == null || zoomMove == null) return false;

            Debug.WriteLine($"Target Preset : {preset}, PTZ State : {panTiltMove}, Zoom State : {zoomMove}");

            if ((panTiltMove == MoveStatus.IDLE && zoomMove == MoveStatus.IDLE)
            || (panTiltMove == MoveStatus.UNKNOWN && zoomMove == MoveStatus.UNKNOWN))
            {
                item.IsMoving = false;
                return false;
            }
            item.IsMoving = true;
            return true;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private CameraDeviceProvider _deviceProvider;
        private CameraPresetProvider _presetProvider;
        private CameraMappingProvider _mappingProvider;
        private OnvifProvider _onvifProvider;
        private OnvifSetupModel _setupModel;
        #endregion
    }
}
