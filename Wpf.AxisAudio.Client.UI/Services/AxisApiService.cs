using Ironwall.Libraries.Api.Client.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System;
using Wpf.AxisAudio.Common.Providers;
using System.Collections.Generic;
using Wpf.AxisAudio.Common.Models;
using Newtonsoft.Json;
using Wpf.AxisAudio.Client.UI.Providers.Models;
using Ironwall.Libraries.Api.Common.Models;
using System.Net;
using System.Threading.Tasks;
using Ironwall.Libraries.Api.Common.Defines;
using Wpf.AxisAudio.Common.Enums;
using System.Linq;
using System.Security.Policy;
using Wpf.AxisAudio.Client.UI.Providers.ViewModels;
using Wpf.AxisAudio.Client.UI.ViewModels;
using Ironwall.Libraries.Base.Services;
using Caliburn.Micro;
using Wpf.AxisAudio.Client.UI.Models;
using System.Windows;
using Ironwall.Framework.Models.Maps.Symbols;
using Autofac.Core;
using MaterialDesignThemes.Wpf;
using System.Timers;

namespace Wpf.AxisAudio.Client.UI.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/15/2023 3:05:30 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AxisApiService : TaskService
    {

        #region - Ctors -
        public AxisApiService(ILogService logService
                            , IEventAggregator eventAggregator
                            , AudioSetupModel audioSetupModel
                            , ApiClientService apiClientService
                            , AudioProvider audioProviders
                            , AudioSensorProvider audioSensorProvider
                            , AudioGroupProvider audioGroupProvider
                            , AudioSymbolProvider audioSymbolProvider
                            , AudioViewModelProvider audioViewModelProvider
                            , AudioSymbolViewModelProvider audioSymbolViewModelprovider)
        {
            _setupModel = audioSetupModel;
            _apiClientService = apiClientService;
            _log = logService;
            _eventAggregator = eventAggregator;
            _audioProvider = audioProviders;
            _audioSensorProvider = audioSensorProvider;
            _audioGroupProvider = audioGroupProvider;
            _audioSymbolProvider = audioSymbolProvider;

            _audioViewModelProvider = audioViewModelProvider;
            _audioSymbolViewModelProvider = audioSymbolViewModelprovider;
            _apiClientService.Received += ApiClientService_Received;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task RunTask(CancellationToken token = default)
        {
            _eventAggregator.SubscribeOnUIThread(this);
            _ = Task.Run(async () =>
            {
                await ApiParamSettingRequest();
                InitTimer();
            });
            return Task.CompletedTask;
        }

        protected override async Task ExitTask(CancellationToken token = default)
        {
            try
            {
                await ApiStopAllClipRequest();
                await ProxyStreamInitRequest();
                timer.Stop();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ExitTask)} of {nameof(AxisApiService)} : {ex}", true);
            }
            
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        /// <summary>
        /// LookupTable을 구성하는 함수
        /// </summary>
        public void CreateLookupTable()
        {
            //LookupTable = from groupModel in _audioGroupViewModelProvider
            //              join sensorModel in _audioSensorProvider
            //              on groupModel.GroupName equals sensorModel.GroupName into sensorGroup
            //              //from sensorModel in sensorGroup.DefaultIfEmpty()
            //              join audioModel in _audioProvider
            //              on groupModel.GroupNumber equals audioModel.Groups into audioGroup
            //              //from audioModel in audioGroup.DefaultIfEmpty()

            //              select new AudioLookupModel
            //              {
            //                  GroupModel = groupModel,
            //                  SensorModels = sensorGroup,
            //                  AudioModels = audioGroup,
            //              };

            LookupTable = _audioGroupProvider.Select(groupModel => new AudioLookupModel
            {
                GroupModel = groupModel,
                SensorModels = groupModel.SensorModels,
                AudioModels = groupModel.AudioModels
            });



            _log.Info($"<<AxisApiService LookupTable>>", true);
            foreach (var item in LookupTable)
            {
                _log.Info($"GroupModel : {item?.GroupModel?.GroupName}", true);
                if(item?.SensorModels != null)
                {
                    foreach (var item1 in item?.SensorModels)
                    {
                        _log.Info($"Sensor : {item1?.ControllerId}-{item1?.SensorId}", true);
                    }
                }
                if(item?.AudioModels != null)
                {
                    foreach (var item2 in item?.AudioModels)
                    {
                        _log.Info($"Audio : {item2?.Id}-{item2?.DeviceName}", true);
                    }
                }
                
            }
        }

        public void CreateVisualLookupTable()
        {

            VisualLookupTable = from audioModel in _audioViewModelProvider
                                join symbolModel in _audioSymbolViewModelProvider
                                on audioModel.DeviceName equals symbolModel.NameDevice
                                into symbolGroup
                                from symbolModel in symbolGroup.DefaultIfEmpty()
                                select new AudioVisualLookupModel
                                {
                                    AudioModel = audioModel,
                                    SymbolModel = symbolModel as AudioSymbolViewModel,
                                };
            _log.Info($"<<AxisApiService VisualLookupTable>>", true);
            foreach (var item in VisualLookupTable)
            {
                _log.Info($"SymbolViewModel : {item?.AudioModel?.DeviceName}", true);
                _log.Info($"AudioViewModel : {item?.SymbolModel?.NameDevice}", true);
            }
        }
        /// <summary>
        /// 마이크 방송 서버로 초기화 작업을 수행
        /// </summary>
        /// <returns></returns>
        public async Task<string> ProxyStreamInitRequest()
        {
            try
            {
                var models = VisualLookupTable
                    .Select(entity => entity.AudioModel.Model).OfType<AudioModel>().ToList();

                var request = new StreamInitRequestMessage(models);
                string json = JsonConvert.SerializeObject(request);

                //별도의 ApiServerModel을 할당하여 동작시킨다.
                var apiServerModel = new ApiServerModel()
                {
                    Name = "StreamingProxyServer",
                    IpAddress = "localhost",
                    Port = 5400,
                    Url = "api/streaming_stop",
                };
                return await _apiClientService.PostRequest(apiServerModel, json);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ProxyStreamingRequest)} of {nameof(AxisApiService)} : {ex}", true);
                return null;
            }
        }
        /// <summary>
        /// Service API Server를 통해 Local마이크를 제어하기 위한 용도의 Streaming Control Request
        /// 따라서, Audio Bridge로 보내는 API와 로직을 달리한다.
        /// </summary>
        /// <param deviceName="models"></param>
        /// <param deviceName="control"></param>
        public async Task<string> ProxyStreamingRequest(List<AudioModel> models, List<AudioModel> requestModels, bool control)
        {
            try
            {
                var request = new StreamRequestMessage(models, requestModels, control);
                string json = JsonConvert.SerializeObject(request);

                //별도의 ApiServerModel을 할당하여 동작시킨다.
                var apiServerModel = new ApiServerModel()
                {
                    Name = "StreamingProxyServer",
                    IpAddress = "localhost",
                    Port = 5400,
                    Url = "api/streaming",
                };
                return await _apiClientService.PostRequest(apiServerModel, json);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ProxyStreamingRequest)} of {nameof(AxisApiService)} : {ex}", true);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public async Task ProxyStreamingRequest(string device, bool control)
        {
            try
            {
                var requestModels = VisualLookupTable
                    .Where(entity => entity.SymbolModel.NameDevice == device)
                    .Select(entity => entity.AudioModel.Model).OfType<AudioModel>().ToList();
                var models = VisualLookupTable
                    .Select(entity => entity.AudioModel.Model).OfType<AudioModel>().ToList();

                var request = new StreamRequestMessage(models, requestModels, control);
                string json = JsonConvert.SerializeObject(request);

                //별도의 ApiServerModel을 할당하여 동작시킨다.
                var apiServerModel = new ApiServerModel()
                {
                    Name = "StreamingProxyServer",
                    IpAddress = "localhost",
                    Port = 5400,
                    Url = "api/streaming",
                };

                var response = await _apiClientService.PostRequest(apiServerModel, json);

                var ret = JsonConvert.DeserializeObject<StreamResponseMessage>(response);
                if (ret != null && ret.IsSuccess)
                {
                    ret.Models.ForEach(model => 
                    {
                        var lookup = VisualLookupTable.Where(entity => entity.AudioModel.DeviceName == model.DeviceName).FirstOrDefault();

                        lookup.AudioModel.Mode = model.Mode;
                        lookup.SymbolModel.ChangeMode(model.Mode);
                    });
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ProxyStreamingRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="control"></param>
        /// <returns></returns>
        public async Task ProxyStreamingGroupRequest(IAudioBaseModel group, bool control)
        {
            try
            {
                //int groupNumber = VisualLookupTable
                //    .Where(entity => entity.SymbolModel.NameDevice == name)
                //    .Select(entity => entity.AudioModels.Model.Groups).FirstOrDefault();

                var requestModels = VisualLookupTable
                    .Where(entity => entity.AudioModel.Groups.Contains(group))
                    .Select(entity => entity.AudioModel.Model).OfType<AudioModel>().ToList();


                var models = VisualLookupTable
                    .Select(entity => entity.AudioModel.Model).OfType<AudioModel>().ToList();

                var request = new StreamRequestMessage(models, requestModels, control);
                string json = JsonConvert.SerializeObject(request);

                ////별도의 ApiServerModel을 할당하여 동작시킨다.
                var apiServerModel = new ApiServerModel()
                {
                    Name = "StreamingProxyServer",
                    IpAddress = "localhost",
                    Port = 5400,
                    Url = "api/streaming",
                };

                var response = await _apiClientService.PostRequest(apiServerModel, json);

                var ret = JsonConvert.DeserializeObject<StreamResponseMessage>(response);
                if (ret != null && ret.IsSuccess)
                {
                    ret.Models.ForEach(model =>
                    {
                        var lookup = VisualLookupTable.Where(entity => entity.AudioModel.DeviceName == model.DeviceName).FirstOrDefault();

                        lookup.AudioModel.Mode = model.Mode;
                        lookup.SymbolModel.ChangeMode(model.Mode);
                    });
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ProxyStreamingRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        /// <summary>
        /// Audio Bridge API Server로 전송하기 위한 로직
        /// 서버의 파라미터 값을 수정한다.
        /// Audio Bridge 등 Axis 오디오 장비에 API를 연동하기 위한 준비 작업이다.
        /// 초기화가 완료되면, SymbolAudioViewModel에 Mode가 변경되어, 맵상의 아이콘 색상이 변경된다.
        /// 초록: 정상, 회색: 비정상
        /// </summary>
        /// <returns></returns>
        public async Task ApiParamSettingRequest(bool isMonitor = false)
        {
            try
            {
                await _semaphore.WaitAsync();

                var url = "axis-cgi/param.cgi";

                _audioProvider.ToList().ForEach(async entity =>
                {
                    var formContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("action", "update"),
                        new KeyValuePair<string, string>("AudioSource.A0.AudioSupport", "yes"),
                        new KeyValuePair<string, string>("Audio.A0.Enabled", "yes"),
                        });

                    //모니터링 모드가 아니고, EnumAxisAudioDeviceState.NONE이 아니면 리턴
                    //정상초기화된 상태
                    if (!isMonitor && entity.Mode != (int)EnumAxisAudioDeviceState.NONE) return;


                    //모니터링 모드이고, EnumAxisAudioDeviceState.NONE, EnumAxisAudioDeviceState.ACTIVE 모드인 경우만
                    if (isMonitor &&
                    !((entity.Mode == (int)EnumAxisAudioDeviceState.NONE) ||
                    (entity.Mode == (int)EnumAxisAudioDeviceState.ACTIVATED))) return;


                    var ret = await AxisGetRequest(entity, url, EnumAxisApiCmd.SET_PARAM, formContent);
                    if (ret != "OK") return;

                    entity.Mode = (int)EnumAxisAudioDeviceState.ACTIVATED;

                    var visualLookup = VisualLookupTable
                    .Where(e => e.AudioModel.Model == entity).FirstOrDefault();
                    visualLookup.AudioModel.Refresh();
                    visualLookup.SymbolModel.ChangeMode((int)EnumAxisAudioDeviceState.ACTIVATED);

                    await ApiGetClipListRequest(entity);
                });
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiParamSettingRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Audio Bridge API Server로 전송하기 위한 로직
        /// 오디오 클립의 리스트를 가져오기 위해서 사용
        /// </summary>
        public Task ApiGetClipListRequest(AudioModel model)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var url = $"axis-cgi/param.cgi?action=list&deviceName=MediaClip";
                    string ret = "";
                    switch ((EnumAxisAudioDeviceState)model.Mode)
                    {
                        case EnumAxisAudioDeviceState.NONE:
                            break;
                        case EnumAxisAudioDeviceState.ACTIVATED:
                        case EnumAxisAudioDeviceState.MIC_STREAMING:
                        case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING:
                        case EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING:
                            ret = await AxisGetRequest(model, url, EnumAxisApiCmd.GET_AUDIO_CLIP_LIST);
                            if (ret != null)
                            {
                                MediaClipConfigModel clip = ParsingMedia(ret);
                                model.MediaClip = clip;

                                var visualLookup = VisualLookupTable.Where(entity => entity.AudioModel.DeviceName == model.DeviceName).FirstOrDefault();
                                visualLookup.AudioModel.Refresh();
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(ApiGetClipListRequest)} of {nameof(AxisApiService)} : {ex}", true);
                }

            });
        }

        /// <summary>
        /// Audio Bridge API Server로 전송하기 위한 로직
        /// 오디오 클립의 선택된 파일을 실행하기 위해서 사용 
        /// </summary>
        /// <param deviceName="deviceName"></param>
        /// <param deviceName="clip"></param>
        /// <param deviceName="repeat"></param>
        /// <param deviceName="volume"></param>
        /// <returns></returns>
        public async Task ApiPlayClipRequest(string deviceName, int clip = 0, int repeat = -1, int volume = 100)
        {
            try
            {
                var audioModel = VisualLookupTable
                .Where(entity => entity.SymbolModel.NameDevice == deviceName).FirstOrDefault();
                await PlayClipAudio(audioModel, repeat, volume);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiPlayClipRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        /// <summary>
        /// Audio Bridge API Server로 전송하기 위한 로직
        /// 오디오 클립의 선택된 파일을 실행하기 위해서 사용 
        /// </summary>
        /// <param deviceName="deviceName"></param>
        /// <param deviceName="clip"></param>
        /// <param deviceName="repeat"></param>
        /// <param deviceName="volume"></param>
        /// <returns></returns>
        public async Task ApiPlayClipGroupRequest(IAudioGroupBaseModel group, int clip = 0, int repeat = -1, int volume = 100)
        {
            try
            {
                var audioModels = VisualLookupTable
                    .Where(entity => entity.AudioModel.Groups.Contains(group))
                    .ToList();

                foreach (var item in audioModels)
                {
                    await PlayClipAudio(item, repeat, volume);
                }

            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiPlayClipRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        

        public async Task ApiStopClipRequest(string deviceName, int clip = 0)
        {
            try
            {
                var audioModel = VisualLookupTable
                .Where(entity => entity.SymbolModel.NameDevice == deviceName).FirstOrDefault();

                await StopClipAudio(audioModel, clip);
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiGetClipListRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        public async Task ApiStopClipGroupRequest(IAudioGroupBaseModel group, int clip = 0)
        {
            try
            {
                var audioModels = VisualLookupTable
                    .Where(entity => entity.AudioModel.Groups.Contains(group))
                    .ToList();

                foreach (var item in audioModels)
                {
                    await StopClipAudio(item, clip);
                }
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiGetClipListRequest)} of {nameof(AxisApiService)} : {ex}", true);
            }
        }

        public Task ApiStopAllClipRequest(int clip = 0)
        {
            try
            {
                var stopTasks = new List<Task>();
                foreach (var item in VisualLookupTable)
                {
                    stopTasks.Add( StopClipAudio(item, clip));
                }
                Task.WhenAll(stopTasks);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ApiGetClipListRequest)} of {nameof(AxisApiService)} : {ex}", true);
                return Task.CompletedTask;
            }
        }

        private Task PlayClipAudio(AudioVisualLookupModel audioModel, int repeat, int volume)
        {
            return Task.Run(async () => 
            {
                try
                {
                    string ret = "";

                    if (audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.NONE) return;
                    if (audioModel.AudioModel.MediaClip.MediaClips.Count == 0) return;

                    var media = audioModel.AudioModel.MediaClip.MediaClips
                        .Where(entity => entity.Name == $"{_setupModel.AudioFile}").FirstOrDefault();
                    var url = $"axis-cgi/mediaclip.cgi?action=play&clip={media?.Id}&repeat={repeat}&volume={volume}";
                    ret = await AxisGetRequest(audioModel.AudioModel.Model as AudioModel
                        , url
                        , EnumAxisApiCmd.PLAY_AUDIO_CLIP);
                    ret = await ParsingResponse(ret);

                    if (ret == "OK"
                    && audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.ACTIVATED)
                    {
                        audioModel.AudioModel.Mode = (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING;
                        audioModel.SymbolModel.ChangeMode((int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING);
                    }
                    else if (ret == "OK"
                    && audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.MIC_STREAMING)
                    {
                        audioModel.AudioModel.Mode = (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING;
                        audioModel.SymbolModel.ChangeMode((int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(PlayClipAudio)} of {nameof(AxisApiService)} : {ex}", true);
                }
            });
        }

        private Task StopClipAudio(AudioVisualLookupModel audioModel, int clip)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.NONE) return;
                    if (audioModel.AudioModel.MediaClip.MediaClips.Count == 0) return;

                    string ret = "";
                    var media = audioModel.AudioModel.MediaClip.MediaClips
                        .Where(entity => entity.Name == $"{_setupModel.AudioFile}").FirstOrDefault();
                    var url = $"axis-cgi/mediaclip.cgi?action=stop&clip={clip}";
                    ret = await AxisGetRequest(audioModel.AudioModel.Model as AudioModel, url, EnumAxisApiCmd.STOP_AUDIO_CLIP);
                    ret = await ParsingResponse(ret);

                    if (ret == "OK" && audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING)
                    {
                        audioModel.AudioModel.Mode = (int)EnumAxisAudioDeviceState.ACTIVATED;
                        audioModel.SymbolModel.ChangeMode((int)EnumAxisAudioDeviceState.ACTIVATED);
                    }
                    else if (ret == "OK" && audioModel.AudioModel.Mode == (int)EnumAxisAudioDeviceState.AUDIO_CLIP_PLAYING_WITH_MIC_STREAMING)
                    {
                        audioModel.AudioModel.Mode = (int)EnumAxisAudioDeviceState.MIC_STREAMING;
                        audioModel.SymbolModel.ChangeMode((int)EnumAxisAudioDeviceState.MIC_STREAMING);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(StopClipAudio)} of {nameof(AxisApiService)} : {ex}", true);
                }
            });
        }

        public Task<string> AxisGetRequest(AudioModel model, string url, EnumAxisApiCmd cmd, FormUrlEncodedContent formContent = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.Credentials = new NetworkCredential(model.UserName, model.Password);

                    using (HttpClient client = new HttpClient(handler))
                    {
                        // 3초 타임아웃 설정
                        client.Timeout = TimeSpan.FromSeconds(3);

                        try
                        {
                            // GET 요청 URL 구성
                            Uri requestUri = null;
                            if (formContent != null)
                            {
                                // FormUrlEncodedContent를 문자열로 변환
                                var formDataStr = await formContent.ReadAsStringAsync();
                                requestUri = new Uri($"http://{model.IpAddress}:{model.Port}/{url}?{formDataStr}");
                            }
                            else
                                requestUri = new Uri($"http://{model.IpAddress}:{model.Port}/{url}");

                            // GET 요청 전송
                            HttpResponseMessage response = await client.GetAsync(requestUri);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                response.EnsureSuccessStatusCode();
                                string responseBody = await response.Content.ReadAsStringAsync();
                                //_log.Info(responseBody, true);
                                return responseBody;
                            }
                            else if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                _log.Info($"Api account was not authorized!", true);
                                return null;
                            }
                            else
                            {
                                _log.Info($"Api Connection was failure!", true);
                                return null;
                            }
                        }
                        catch (HttpRequestException ex)
                        {
                            // 연결 실패 또는 다른 HTTP 요청 오류 처리
                            _log.Error($"Raised {nameof(HttpRequestException)} in {nameof(AxisGetRequest)} of {nameof(AxisApiService)} : {ex}", true);
                            return null;
                        }
                        catch (TaskCanceledException ex)
                        {
                            // 타임아웃 처리
                            _log.Error($"Raised {nameof(TaskCanceledException)} in {nameof(AxisGetRequest)} of {nameof(AxisApiService)} : {ex}", true);
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(AxisGetRequest)} of {nameof(AxisApiService)} : {ex}", true);
                    return null;
                }
            });
        }

        private MediaClipConfigModel ParsingMedia(string responseBody)
        {
            try
            {
                var config = new MediaClipConfigModel();
                if (responseBody == null) return config;

                var lines = responseBody?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    if (!line.Contains("root.MediaClip")) continue;

                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (key == "root.MediaClip.MaxGroups")
                    {
                        config.MaxGroups = int.Parse(value);
                    }
                    else if (key == "root.MediaClip.MaxUploadSize")
                    {
                        config.MaxUploadSize = int.Parse(value);
                    }
                    else
                    {
                        var mediaClipParts = key.Split('.');
                        if (mediaClipParts.Length != 4) continue;

                        var index = int.Parse(mediaClipParts[2].Substring(1));
                        var property = mediaClipParts[3];

                        if (!(config.MediaClips.Where(entity => entity.Id == index).Count() > 0))
                            config.MediaClips.Add(new MediaClipModel(index));

                        var mediaClip = config.MediaClips.Where(entity => entity.Id == index).FirstOrDefault();

                        switch (property)
                        {
                            case "Location":
                                mediaClip.Location = value;
                                break;
                            case "Name":
                                mediaClip.Name = value;
                                break;
                            case "Type":
                                mediaClip.Type = value;
                                break;
                        }
                    }
                }
                return config;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(ParsingMedia)} of {nameof(AxisApiService)} : {ex}", true);
                return null;
            }
        }

        private Task<string> ParsingResponse(string responseBody)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (responseBody == null) return null;

                    var lines = responseBody?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string ret = lines.FirstOrDefault().Trim();
                    return ret;
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(ParsingMedia)} of {nameof(AxisApiService)} : {ex}", true);
                    return null;
                }
            });
        }

        public Task SendEventCycle(AudioLookupModel lookupModel, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (lookupModel == null) return;
                    await ApiPlayClipGroupRequest(lookupModel.GroupModel);
                    await Task.Delay(-1, token);
                    await ApiStopClipGroupRequest(lookupModel.GroupModel);
                }
                catch (TaskCanceledException ex)
                {
                    await ApiStopClipGroupRequest(lookupModel.GroupModel);
                    _log.Info($"Task{nameof(SendEventCycle)} was cancelled : {ex.Message}", true);
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised {nameof(Exception)} in {nameof(SendEventCycle)} of {nameof(AxisApiService)} : {ex.Message}", true);
                }
            });
        }

        private void InitTimer()
        {
            if (timer != null)
            {
                timer.Elapsed -= new ElapsedEventHandler(Tick);
                timer.Close();
            }

            timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(Tick);
            SetTimerEnable(true);
            SetInterval();
        }
        public void SetTimerEnable(bool value)
        {
            timer.Enabled = value;
        }
        public bool SetInterval(int time = 60)
        {
            if (time == 0)
                return false;
            //Input Value will be minutes
            _intervalSecond = time;
            timer.Interval = TimeSpan.FromSeconds(_intervalSecond).TotalMilliseconds;
            timer.Start();
            return true;
        }

        private async void Tick(object sender, ElapsedEventArgs e)
        {
            _log.Info($"Axis Audio {_intervalSecond}s 간격 주기적 모니터링 시작.", true);
            await ApiParamSettingRequest(true);
        }

        private void ApiClientService_Received(string obj)
        {
            //Debug.WriteLine(obj);
            var model = JsonConvert.DeserializeObject<MediaClipConfigModel>(obj);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public IEnumerable<AudioLookupModel> LookupTable { get; set; }
        public IEnumerable<AudioVisualLookupModel> VisualLookupTable { get; set; }

        #endregion
        #region - Attributes -
        private AudioSetupModel _setupModel;
        private ApiClientService _apiClientService;
        private ILogService _log;
        private IEventAggregator _eventAggregator;
        private AudioProvider _audioProvider;
        private AudioSensorProvider _audioSensorProvider;
        private AudioGroupProvider _audioGroupProvider;
        private AudioSymbolProvider _audioSymbolProvider;
        private AudioViewModelProvider _audioViewModelProvider;
        private AudioSymbolViewModelProvider _audioSymbolViewModelProvider;
        private System.Timers.Timer timer;
        private int _intervalSecond;

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        #endregion
    }
}
