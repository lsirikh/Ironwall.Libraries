using Ironwall.Framework.Services;
using Ironwall.Libraries.Cameras.Providers;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.ViewModels;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Onvif.DataProviders;
using Ironwall.Libraries.Onvif.Models;
using Mictlanix.DotNet.Onvif.Ptz;
using OnvifControl.Libraries.Onvif.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Libraries.Cameras.Providers.Models;
using Mictlanix.DotNet.Onvif.Common;
using Ironwall.Libraries.Base.Services;
using Caliburn.Micro;

namespace Ironwall.Libraries.Onvif.Services
{
    public class PtzService
        //: CameraTaskTimer, IService
    {
        #region - Ctors -
        public PtzService(
            //CameraDeviceDataProvider cameraDeviceProvider
            //, CameraPresetDataProvider cameraPresetProvider
            ILogService log
            , CameraDeviceProvider cameraDeviceProvider
            , CameraPresetProvider cameraPresetProvider
            , OnvifProvider onvifProvider
            , CameraSetupModel cameraSetupModel)
        {
            _log = log;
            _deviceProvider = cameraDeviceProvider;
            _presetProvider = cameraPresetProvider;

            _onvifProvider = onvifProvider;
            SetupModel = cameraSetupModel;

            _locker = new object();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        public ICameraPresetModel GetPresetGroup(int idController, int idSensor)
        {
            try
            {
                //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(GetPresetGroup)}] was executed.");

                var presetGroup = _presetProvider
                    .Where(t => (t as ICameraPresetModel).IdController == idController
                && ((t as ICameraPresetModel).IdSensorBgn <= idSensor && (t as ICameraPresetModel).IdSensorEnd >= idSensor))
                    .FirstOrDefault() as ICameraPresetModel;
                return presetGroup;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in IsCameraAvailable : {ex.Message}");
                return null;
            }
        }

        public Task<List<ICameraDeviceModel>> GetRelatedModel(ICameraPresetModel model)
        {
            return Task<List<ICameraDeviceModel>>.Factory.StartNew(() =>
            {
                //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(GetRelatedModel)}] was executed.");

                List<ICameraDeviceModel> modelList = new List<ICameraDeviceModel>();
                foreach (ICameraDeviceModel item in _deviceProvider)
                {
                    if (item.Name == model.CameraFirst || item.Name == model.CameraSecond)
                    {
                        modelList.Add(item);
                    }
                }
                return modelList;
            });
        }

        //public Task CameraPresetPtzControl(ICameraPresetModel model)
        //{
        //    return Task.Factory.StartNew(async () =>
        //    {
        //        try
        //        {
        //            ICameraDeviceViewModel model1 = null;
        //            ICameraDeviceViewModel model2 = null;
        //            foreach (ICameraDeviceViewModel item in _deviceProvider
        //            .Where(t => t.TypeDevice == (int)EnumCameraType.PTZ).ToList())
        //            {
        //                if (item.Name == model.CameraFirst
        //                && IsModeCheck(item, EnumCameraMode.ONVIF))
        //                {
        //                    model1 = item;
        //                }
        //                else if (item.Name == model.CameraSecond
        //                && IsModeCheck(item, EnumCameraMode.ONVIF))
        //                {
        //                    model2 = item;
        //                }
        //            }

        //            if (model1 != null)
        //            {
        //                if (model1.Cts != null)
        //                    model1.Cts.Cancel();

        //                await Task.Delay(100);
        //                model1.Cts = new CancellationTokenSource();
        //                await CamearPtzCycle(model1, model.HomePresetFirst, model.TargetPresetFirst, model.ControlTime);

        //            }

        //            if (model2 != null)
        //            {
        //                if (model2.Cts != null)
        //                    model2.Cts.Cancel();

        //                await Task.Delay(100);
        //                model2.Cts = new CancellationTokenSource();
        //                await CamearPtzCycle(model2, model.HomePresetSecond, model.TargetPresetSecond, model.ControlTime);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetPtzControl]Raised Exception : {ex.Message}");
        //        }

        //    });
        //}

        public Task CameraPresetPtzControl(ICameraPresetModel model)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(CameraPresetPtzControl)}] was executed.");

                    IOnvifModel model1 = null;
                    IOnvifModel model2 = null;

                    foreach (var item in _onvifProvider
                    .Where(t => t.CameraDeviceModel.TypeDevice == (int)EnumCameraType.PTZ).ToList())
                    {
                        if (item.CameraDeviceModel.Name == model.CameraFirst
                        && IsModeCheck(item.CameraDeviceModel, EnumCameraMode.ONVIF))
                        {
                            model1 = item;
                        }
                        else if (item.CameraDeviceModel.Name == model.CameraSecond
                        && IsModeCheck(item.CameraDeviceModel, EnumCameraMode.ONVIF))
                        {
                            model2 = item;
                        }
                    }

                    #region - Commented -
                    //lock (_locker)
                    //{
                    //    model1 = new CameraOnvifModel();
                    //    model2 = new CameraOnvifModel();
                    //    foreach (ICameraDeviceViewModel item in _deviceProvider
                    //    .Where(t => t.TypeDevice == (int)EnumCameraType.PTZ).ToList())
                    //    {
                    //        if (item.Name == model.CameraFirst
                    //        && IsModeCheck(item, EnumCameraMode.ONVIF))
                    //        {
                    //            model1.CameraDeviceViewModel = item;
                    //        }
                    //        else if (item.Name == model.CameraSecond
                    //        && IsModeCheck(item, EnumCameraMode.ONVIF))
                    //        {
                    //            model2.CameraDeviceViewModel = item;
                    //        }
                    //    }
                    //}
                    #endregion

                    if (model1?.CameraDeviceModel != null)
                    {
                        //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(CameraPresetPtzControl)}] was executed. ==> {model1.CameraDeviceModel.Name}({model1.GetHashCode()}), Token({model1?.Cts?.GetHashCode()})");

                        if (model1.Cts != null)
                            model1.Cts.Cancel();

                        await Task.Delay(100);
                        model1.Cts = new CancellationTokenSource();
                        
                        await CamearPtzCycle(model1, model.HomePresetFirst, model.TargetPresetFirst, model.ControlTime);
                    }

                    if (model2?.CameraDeviceModel != null)
                    {
                        //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(CameraPresetPtzControl)}] was executed. ==> {model2.CameraDeviceModel.Name}({model2.GetHashCode()}), Token({model2?.Cts?.GetHashCode()})");

                        if (model2.Cts != null)
                            model2.Cts.Cancel();

                        await Task.Delay(100);
                        model2.Cts = new CancellationTokenSource();

                        await CamearPtzCycle(model2, model.HomePresetSecond, model.TargetPresetSecond, model.ControlTime);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetPtzControl]Raised Exception : {ex.Message}");
                }
            });
        }


        private bool IsModeCheck(ICameraDeviceModel item, EnumCameraMode onvif)
        {
            //CameraDeviceProvider에서 API로 세팅된 Camera 찾기
            return _deviceProvider.Where(t => (t as ICameraDeviceModel).Name == item.Name && (t as ICameraDeviceModel).Mode == (int)onvif).Count() > 0 ? true : false;
        }

        //private Task CamearPtzCycle(ICameraDeviceViewModel model, string homePresetFirst, string targetPresetFirst, int controlTime)
        //{
        //    return Task.Factory.StartNew(async () =>
        //    {
        //        try
        //        {
        //            await CameraPresetMoveAsync(model, targetPresetFirst);
        //            await Task.Delay(TimeSpan.FromSeconds(controlTime), model.Cts.Token);
        //            if (model.Cts.IsCancellationRequested)
        //            {
        //                Debug.WriteLine($"{model.Name} was Cancelled!!!!!!!!!!!!!!!!!!!!!!!!!");
        //                return;
        //            }

        //            await CameraPresetMoveAsync(model, homePresetFirst);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CamearPtzCycle]Raised Exception : {ex.Message}");
        //        }

        //    }, model.Cts.Token);
        //}

        private Task CamearPtzCycle(IOnvifModel model, string homePresetFirst, string targetPresetFirst, int controlTime)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(CamearPtzCycle)}] was executed.");
                    await CameraPresetMoveAsync(model, targetPresetFirst);
                    await Task.Delay(TimeSpan.FromSeconds(controlTime), model.Cts.Token);
                    if (model.Cts.IsCancellationRequested)
                    {
                        _log.Info($"{model.CameraDeviceModel.Name} was Cancelled!!!!!!!!!!!!!!!!!!!!!!!!!");
                        return;
                    }
                    
                    await CameraPresetMoveAsync(model, homePresetFirst);
                }
                catch (Exception ex)
                {
                    _log.Error($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CamearPtzCycle]Raised Exception : {ex.Message}");
                }

            }, model.Cts.Token);
        }

        //public Task CameraPresetMoveAsync(ICameraDeviceViewModel item, string preset)
        //{
        //    return Task.Factory.StartNew(async () =>
        //    {
        //        try
        //        {

        //            if (model.onvifControlService == null)
        //            {
        //                item.onvifControlService = new OnvifControlService();
        //                var host = item.IpAddress + ":" + item.Port;

        //                var ret = await item.onvifControlService.DeviceReady(host, item.UserName, item.Password);
        //                //Thread.Sleep(500);
        //                if (!ret)
        //                    return;

        //                await item.onvifControlService.CreateProfile();

        //                if (item.ptzPresetProvider == null)
        //                {
        //                    GetPresetsResponse presets = await item.onvifControlService.GetPresets();
        //                    item.ptzPresetProvider = new PTZPresetProvider();
        //                    foreach (var ptzInfo in presets?.Preset)
        //                    {
        //                        item.ptzPresetProvider.Add(ptzInfo);
        //                    }
        //                }
        //            }

        //            item.isMoving = true;
        //            Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService]++++++{item.Model.Name}({preset}) PTZ Move Command!.....++++++");

        //            var ptzPreset = item.ptzPresetProvider.Where(t => t.Name == preset).FirstOrDefault();
        //            await item.onvifControlService.GotoPreset(ptzPreset);

        //            item.isMoving = false;

        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
        //        }
        //    });
        //}

        public Task CameraPresetMoveAsync(IOnvifModel item, string preset = null)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    //Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][{nameof(CameraPresetMoveAsync)}] was executed.");

                    if (item.OnvifControlService == null)
                    {
                        item.OnvifControlService = new OnvifControlService();
                        var host = item.CameraDeviceModel.IpAddress + ":" + item.CameraDeviceModel.Port;

                        var ret = await item.OnvifControlService.DeviceReady(host, item.CameraDeviceModel.UserName, item.CameraDeviceModel.Password);
                        //Thread.Sleep(500);
                        if (!ret)
                            return;

                        await item.OnvifControlService.CreateProfile();

                        if (item.PtzPresetProvider == null)
                        {
                            GetPresetsResponse presets = await item.OnvifControlService.GetPresets();
                            item.PtzPresetProvider = new PTZPresetProvider();
                            foreach (var ptzInfo in presets?.Preset)
                            {
                                item.PtzPresetProvider.Add(ptzInfo);
                            }
                        }
                    }

                    
                    //_log.Error($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService]++++++{item.CameraDeviceModel.Name}({preset}) PTZ Move Command!.....++++++");

                    if(preset != null)
                    {
                        var ptzPreset = item.PtzPresetProvider.Where(t => t.Name == preset).FirstOrDefault();
                        await item.OnvifControlService.GotoPreset(ptzPreset);
                       
                        while (await CheckPtzMove(item, preset))
                        {
                            try
                            {
                                await Task.Delay(1000);
                            }
                            catch (TaskCanceledException ex)
                            {
                                await item.OnvifControlService.StopPreset();
                                _log.Error($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised TaskCanceledException : {ex.Message}");
                            }
                            catch (Exception ex)
                            {
                                _log.Error($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                }
            });
        }

        private async Task<bool> CheckPtzMove(IOnvifModel item, string preset = null)
        {
            var status = await item.OnvifControlService.GetPtzStatus();
            var panTiltMove = status.MoveStatus.PanTilt;
            var zoomMove = status.MoveStatus.Zoom;

            _log.Info($"Target Preset : {preset}, PTZ State : {panTiltMove}, Zoom State : {zoomMove}");

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
        public CameraSetupModel SetupModel { get; }

        private ILogService _log;
        #endregion
        #region - Attributes -
        //private ICameraOnvifModel model1;
        //private ICameraOnvifModel model2;
        private CameraDeviceProvider _deviceProvider;
        private CameraPresetProvider _presetProvider;
        private OnvifProvider _onvifProvider;
        private object _locker;
        private const int HOME_PRESET = 0;
        private const int TARGET_PRESET = 1;
        #endregion

    }
}
