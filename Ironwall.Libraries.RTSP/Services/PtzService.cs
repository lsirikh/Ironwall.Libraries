using Caliburn.Micro;
using Ironwall.Framework.Services;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Onvif.DataProviders;
using Ironwall.Libraries.RTSP.DataProviders;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.Models.Messages.Dialogs;
using Ironwall.Libraries.RTSP.ViewModels;
using Mictlanix.DotNet.Onvif.Ptz;
using OnvifControl.Libraries.Onvif.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Ironwall.Libraries.RTSP.Services
{
    public class PtzService
        //: CameraTaskTimer, IService
        : IService
    {
        #region - Ctors -
        public PtzService()
        {
            _eventAggergator = IoC.Get<IEventAggregator>();
            _deviceProvider = IoC.Get<CameraDeviceProvider>();
            _presetProvider = IoC.Get<CameraPresetProvider>();
            SetupModel = IoC.Get<CameraSetupModel>();
        }
        #endregion
        #region - Implementation of Interface -
        public Task ExecuteAsync(CancellationToken token = default)
        {
            _eventAggergator.SubscribeOnUIThread(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        public CameraPresetModel GetPresetGroup(int idController, int idSensor)
        {
            try
            {
                var presetGroup = _presetProvider.Where(t => t.IdController == idController
                && (t.IdSensorBgn <= idSensor && t.IdSensorEnd >= idSensor)).FirstOrDefault();
                return presetGroup?.Model;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in IsCameraAvailable : {ex.Message}");
                return null;
            }
        }

        public Task<List<CameraDeviceModel>> GetRelatedModel(CameraPresetModel model)
        {
           return Task<List<CameraDeviceModel>>.Factory.StartNew(() => 
            {
                List<CameraDeviceModel> modelList = new List<CameraDeviceModel>();
                foreach (var item in _deviceProvider)
                {
                    if (item.Name == model.CameraFirst || item.Name == model.CameraSecond)
                    {
                        modelList.Add(item.Model);
                    }
                }
                return modelList;
            });
        }

        public Task CameraPresetPtzControl(CameraPresetModel model)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    CameraDeviceViewModel model1 = null;
                    CameraDeviceViewModel model2 = null;
                    foreach (var item in _deviceProvider
                    .Where(t => t.TypeDevice == (int)EnumCameraType.PTZ).ToList())
                    {
                        if (item.Name == model.CameraFirst
                        && IsModeCheck(item, EnumCameraMode.ONVIF))
                        {
                            model1 = item;
                        }
                        else if (item.Name == model.CameraSecond
                        && IsModeCheck(item, EnumCameraMode.ONVIF))
                        {
                            model2 = item;
                        }
                    }

                    if (model1 != null)
                    {
                        if (model1.cts != null)
                            model1.cts.Cancel();

                        await Task.Delay(100);
                        model1.cts = new CancellationTokenSource();
                        await CamearPtzCycle(model1, model.HomePresetFirst, model.TargetPresetFirst, model.ControlTime);
                        
                    }

                    if (model2 != null)
                    {
                        if (model2.cts != null)
                            model2.cts.Cancel();

                        await Task.Delay(100);
                        model2.cts = new CancellationTokenSource();
                        await CamearPtzCycle(model2, model.HomePresetSecond, model.TargetPresetSecond, model.ControlTime);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetPtzControl]Raised Exception : {ex.Message}");
                }

            });
        }

        private bool IsModeCheck(CameraDeviceViewModel item, EnumCameraMode onvif)
        {
            //CameraDeviceProvider에서 API로 세팅된 Camera 찾기
            var deviceProvider = IoC.Get<CameraDeviceProvider>();
            return deviceProvider.Where(t => t.Name == item.Name
            && t.Mode == (int)onvif).Count() > 0 ? true : false;
        }

        private Task CamearPtzCycle(CameraDeviceViewModel model, string homePresetFirst, string targetPresetFirst, int controlTime)
        {
            return Task.Factory.StartNew(async() => 
            {
                try
                {
                    await CameraPresetMoveAsync(model, targetPresetFirst);
                    await Task.Delay(TimeSpan.FromSeconds(controlTime), model.cts.Token);
                    if (model.cts.IsCancellationRequested)
                    {
                        Debug.WriteLine($"{model.Name} was Cancelled!!!!!!!!!!!!!!!!!!!!!!!!!");
                        return;
                    }

                    await CameraPresetMoveAsync(model, homePresetFirst);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CamearPtzCycle]Raised Exception : {ex.Message}");
                }
                
            }, model.cts.Token);
        }

        /*public Task CameraPresetMoveAsync(CameraDeviceViewModel item, string preset)
        {
            return Task.Factory.StartNew(async() => 
            {
                try
                {
                    if(item.onvifDevice == null)
                    {
                        item.onvifDevice = DeviceFactory<CameraDevice>.Create(item.Model.ToCameraModel());
                        await item.onvifDevice.Connect();
                    }

                    item.isMoving = true;
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService]++++++{item.Model.Name}({preset}) PTZ Move Command!.....++++++");

                    await item.onvifDevice.GotoPresetAsync(preset);

                    item.isMoving = false;

                    //await onvifDevice.Stop();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                }
            });
        }*/

        public Task CameraPresetMoveAsync(CameraDeviceViewModel item, string preset)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    if(item.onvifControlService == null)
                    {
                        item.onvifControlService = new OnvifControlService();
                        var host = item.IpAddress + ":" + item.Port;

                        var ret = await item.onvifControlService.DeviceReady(host, item.UserName, item.Password);
                        //Thread.Sleep(500);
                        if (!ret)
                            return;

                        await item.onvifControlService.CreateProfile();

                        if (item.ptzPresetProvider == null)
                        {
                            GetPresetsResponse presets = await item.onvifControlService.GetPresets();
                            item.ptzPresetProvider = new PTZPresetProvider();
                            foreach (var ptzInfo in presets?.Preset)
                            {
                                item.ptzPresetProvider.Add(ptzInfo);
                            }
                        }
                    }

                    item.isMoving = true;
                    Debug.WriteLine($"[{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService]++++++{item.Model.Name}({preset}) PTZ Move Command!.....++++++");

                    var ptzPreset = item.ptzPresetProvider.Where(t => t.Name == preset).FirstOrDefault();
                    await item.onvifControlService.GotoPreset(ptzPreset);

                    item.isMoving = false;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}][PtzService][CameraPresetMoveAsync]Raised Exception : {ex.Message}");
                }
            });
        }

        


        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public CameraSetupModel SetupModel { get; }
        //public CameraDevice CameraDevice { get; private set; }
        #endregion
        #region - Attributes -
        private CameraDeviceProvider _deviceProvider;
        private CameraPresetProvider _presetProvider;
        private IEventAggregator _eventAggergator;

        private const int HOME_PRESET = 0;
        private const int TARGET_PRESET = 1;
        #endregion

    }
}
