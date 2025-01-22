using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Api.Ollama.Enums;
using Ironwall.Libraries.Api.Ollama.Helpers;
using Ironwall.Libraries.Api.Ollama.Models;
using System;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ollama;
using System.Net.Http;

/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 2:02:10 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
namespace Ironwall.Libraries.Api.Ollama.Services
{
    public class OllamaAiService : IService, IDisposable
    {
        #region - Ctors -
        public OllamaAiService(ILogService log
                            , SetupModel setup)
        {
            _log = log;
            _setup = setup;
        }
        #endregion
        #region - Implementation of Interface -
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            await OllamaPreparingTask(token).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken token = default)
        {
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _apiClient?.Dispose();
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private async Task OllamaPreparingTask(CancellationToken token = default)
        {
            if (token.IsCancellationRequested) return;

            if (!_setup.IsAvailable) return;

            var uri_string = $"{PREFIX}{_setup.IpAddress}:{_setup.Port}/api";
            _log.Info($"Try to connect AI Server({uri_string})....");
            var uri = new Uri(uri_string);
            //_ollama = new OllamaApiClient(uri);

            _isAvailable = false;
            StatusHandler?.Invoke(EnumOllamaStatus.NOT_AVAILABLE);

            try
            {
                //    var result = await _ollama.IsRunningAsync();
                //    if (result == false) throw new Exception($"");
                //    var response = await _ollama.ListLocalModelsAsync();
                //    ReceivedHandler?.Invoke($"### Model Lists");
                //    ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");
                //    foreach (var item in response)
                //    {
                //        ReceivedHandler?.Invoke($"{item.Name}({FormatHelper.FormatFileSize(item.Size)}) - {item.ModifiedAt}");
                //    }
                //    ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");

                //    // select a model which should be used for further operations
                //    var model = "phi4:latest";
                //    //_ollama.SelectedModel = "llama3.2:3b";
                //    _ollama.SelectedModel = model;



                //    _chat = new Chat(_ollama);


                //    await SendMessage("", token);



                var model = "phi4";


                _apiClient = new OllamaApiClient(httpClient: new HttpClient { Timeout = TimeSpan.FromMinutes(10) },
                                                 baseUri: uri);

                {
                    var response = await _apiClient.Models.ListModelsAsync();
                    ReceivedHandler?.Invoke(new MessageModel(null, $"--------------------------------------------------------------------------------------------------------------"));
                    foreach (var item in response.Models)
                    {
                        ReceivedHandler?.Invoke(new MessageModel(null, $"{item.Model1}({FormatHelper.FormatFileSize((long)item.Size)}) - {item.ModifiedAt}"));
                    }
                    ReceivedHandler?.Invoke(new MessageModel(null, $"--------------------------------------------------------------------------------------------------------------"));
                }
                


                _selectedModel = model;

                var preDefinedTemplate = new List<string>{

                "넌 이제부터 관제 프로그램의 AI Assistant야. 관제 프로그램은 군사 시설, 항만, 발전소 등에 설치되는 외곽 울타리 시스템 PIDS에 설치되는 보안 솔루션이야. 관제 프로그램 개발업체는 Sensorway라는 업체이고, 대한민국 고양시 삼송동 삼송 테크노벨리에 위치하고 있어. 대표이사는 엄두섭이고, 고려대학교 교수를 중임하고 있어. 너는 다음 룰을 지켜고, 해당 규칙에 맞게 대답해야되.\r\n 1. 한국어로 \"AI 관제 분석 결과 ~\" 하고 대답을 해야되.\r\n 2. 너는 대답의 길이를 최대 50자를 넘어가서는 안되.\r\n 3. 너는 관제 프로그램과 미리 학습된 내용이외에 정치, 사회, 종교, 문화, 예술, 시사, 연예, 오락 등에 관제 SW와 센서웨이에 관련되지 않은 어떠한 발언도 하면 절대 안되 너는 관제 프로그램의 AI Assistant라는 것을 절대 잊어서는 안돼.\r\n 4. 너는 학습할 내용은 [학습]이라는 접두어가 들어간 메시지는 저장하고 세션에 반영하도록 해야되. \r\n 5. 탐지 메시지는 다음과 같은 형태로 제공될 거야.",
                "예시 {\"command\":\"EVENT_DETECTION_REQUEST\",\"detail\":{{\"id\":813,\"group_event\":\"1\",\"device\":{{\"id\":22,\"device_number\":5,\"device_group\":5,\"device_name\":\"5번_센서\",\"device_type\":\"SmartSensor\",\"controller\":{{\"id\":1,\"device_number\":0,\"device_group\":1,\"device_name\":\"1번_제어기\",\"device_type\":\"Controller\",\"ip_address\":\"192.168.1.1\",\"version\":\"v1.0\",\"ip_port\":80}},\"version\":\"v1.0\"}},\"type_event\":\"Intrusion\",\"result\":6,\"status\":\"False\",\"datetime\":\"2025-01-13T15:03:27.41}",
                "command: 메시지의 타입을 나타내는 속성\r\nEVENT_DETECTION_REQUEST: 탐지 이벤트 요청 메시지\r\ndetail: 탐지 이벤트에 관한 디테일한 내용이 저장되어있다.\r\n  {{\"id\":813,\"group_event\":\"1\",\"device\":{{\"id\":22,\"device_number\":5,\"device_group\":5,\"device_name\":\"5번_센서\",\"device_type\":\"SmartSensor\",\"controller\":{{\"id\":1,\"device_number\":0,\"device_group\":1,\"device_name\":\"1번_제어기\",\"device_type\":\"Controller\",\"ip_address\":\"192.168.1.1\",\"version\":\"v1.0\",\"ip_port\":80}},\"version\":\"v1.0\"}}\r\n  id: 이벤트의 아이디이다.\r\n  group_event: 이벤트의 그룹 정보이다. 이벤트 그룹에 따라 클라이언트나 3rd Party에서 로직을 구현할 수 있다.\r\n  device: 이벤트가 발생하게된 장비의 정보이다.\r\n    {{\"id\":22,\"device_number\":5,\"device_group\":5,\"device_name\":\"5번_센서\",\"device_type\":\"SmartSensor\",\"controller\":{{\"id\":1,\"device_number\":0,\"device_group\":1,\"device_name\":\"1번_제어기\",\"device_type\":\"Controller\",\"ip_address\":\"192.168.1.1\",\"version\":\"v1.0\",\"ip_port\":80}},\"version\":\"v1.0\"}}\r\n    id: 센서 장비의 아이디이다.\r\n    device_number: 장비의 번호이다.\r\n    device_group: 장비의 그룹이다.  같은 그룹의 장비에 따라. 카메라 연동 장비의 동작을 그룹핑하여 Preset의 그룹을 묶어줄 수 있다.\r\n    device_name: 장비의 이름이다.\r\n    device_type: SmartSensor 로 다음과 같은 장비 타입들이 있다.(NONE=0, Controller=1, Multi=2, Fence=3, Underground=4, Contact=5, PIR=6, IoController=7, Laser=8, Cable=9, IpCamera=10, SmartSensor=11, SmartSensor2=12, SmartCompound=13, IpSpeaker=14, Radar=15, OpticalCable=16, Fence_Line=17)\r\n    version: 센서 장비의 펌웨어 버전이다.\r\n    Controller: 제어기는 센서를 총괄하는 장치이다. \r\n      {{\"id\":1,\"device_number\":0,\"device_group\":1,\"device_name\":\"1번_제어기\",\"device_type\":\"Controller\",\"ip_address\":\"192.168.1.1\",\"version\":\"v1.0\",\"ip_port\":80}}\r\n      id: 제어기 장비의 아이디이다.\r\n      device_number: 제어기 장비의 번호이다.\r\n      device_group: 제어기 장비의 그룹이다.  같은 그룹의 장비에 따라. 카메라 연동 장비의 동작을 그룹핑하여 Preset의 그룹을 묶어줄 수 있다.\r\n      device_name: 제어기 장비의 이름이다.\r\n      device_type: Controller로 제어기 장비 타입이다.\r\n      ip_address: 제어기는 ip 기반의 장비이고, ip 주소를 나타낸다.\r\n      version: 제어기 펌웨어 버전이다.\r\n      ip_port: 제어기 웹페이 접속 포트이다. \r\n      \r\n  type_event: 이벤트의 타입을 나타낸다. (Intrusion :침입, ContactOn: 접점 켜기, ContactOff: 접점 끄기, Connection: 연결보고, Action: 조치보고, Fault: 장애보고, WindyMode: 풍량모드)\r\n  result: 6 진동센서에 의한 탐지를 나타낸다.\r\n  status: false는 조치보고(Action Report)가 아직 되지 않는 이벤트로 사용자의 확인이 필요한 이벤트라는 의미이다.\r\n  datetime: 이벤트의 발생 시점이다. 다음이벤트와 연관성을 체크하는데 매우 중요한 요소이다.\r\n",
                "*장비의 종류\r\n1.Controller(제어기) : 제어기는 STM 계열의 센서의 정보를 취합하여 미들웨어라르 프로그램을 데이터를 송신하는 역할을 한다. 상향, 하향 RS485라인에 따라 2개의 케이블라인이 제공되어 이중화를 구현하였다.\r\n2.Multi(복합센서) : STM 기반의 센서로 열상 센서, PIR, 진동 센서, 근첩 센서 등 다양한 센서를 활용하여 탐지를 측정하고, 옵션에 따라 센서의 종류가 다를 수 있다.\r\n3.Fence(펜스센서, 진동센서) : 진동센서는 PIDS에서 가장 많이 활용된 센서로 3축 가속도 센서와 DSP를 활욯하여 적은 오탐지율과 탐지 고성능을 자랑하는 대표 센서이다.\r\n4.Underground(지중센서): 지중센서는 땅속에 설치하는 센서로 울타리를 넘을 수 있는 공간에서 낙하 시도시 땅의 진동을 감지하여 탐지 이벤트를 발생시킨다.\r\n5.Contact(접점센서): 접점센서는 센서라기보다는 일종의 릴레이이다. 외부로 부터 신호를 받으면 IO 릴레이 역할을 해준다.\r\n6.PIR: PIR의 센서 기능을 활용한 센서이다.\r\n7.IoController(접점제어기): 접점 제어기는 센서는 아니고 접점센서를 통합하는 제어기이다.\r\n8.Laser(레이저센서) :  레이저 센서는 복합센서에 레이저 센서가 추가로 장착된 센서로, 부산복합화력 발전소에 적용되었다.\r\n9.SmartSensor(스마트센서) : 중요시설 3, 4차에 적용되었으며, PIR, 진동센서, 초음파센서, IR 센서 등 다양한 센서가 복합적으로 적용되어있다.\r\n10.SmartSensor2(스마트센서2) : 카메라를 활용한 AI 기능을 탑재한 IP 기반의 센서이고, 진동센서, PIR 센서 등 탑재되어 있다.\r\n11.SmartCompound(스마트복합) : Radar, Long Range 혹은 Short Range를 활용한 AI Vision Camera 등을 활용하였으며, 진동센서 등을 활용한 최신 복합 스마트 센서이다.\r\n12.Lidar(라이다센서): 라이다를 활용한 센서로 사전감지 센서로 역할을 한다. 중요시설 3차 사업에 적용이 되었다.",
                };

                _chat = _apiClient.Chat(model);
                _chat.KeepAlive = -1;
                {
                    var response = await _chat.SendAsync(preDefinedTemplate[0], MessageRole.System, cancellationToken: token);
                    _log.Info($"{response.Role}: {response.Content}");
                }

                //foreach (var item in preDefinedTemplate)
                //{

                //    var response = await _chat.SendAsync(item, MessageRole.System, cancellationToken:token);
                //    _log.Info($"{response.Role}: {response.Content}");
                //}

                //foreach (var item in preDefinedTemplate)
                //{
                //    var response = _apiClient.Completions.GenerateCompletionAsync(_selectedModel, item, stream:true, keepAlive:-1);
                //    await foreach (var enumItem in response)
                //    {
                //        if (token.IsCancellationRequested)
                //        {
                //            throw new OperationCanceledException("Message processing was cancelled.");
                //        }
                //        _log.Info($"> {enumItem.Response}");
                //    }
                //}

            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
            finally
            {
                _isAvailable = true;
                StatusHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
            }
        }

        public Task GetModelInfo()
        {
            //if (_ollama == null) return;

            //var request = new ShowModelRequest() { Model = _ollama.SelectedModel };
            //var response = await _ollama.ShowModelAsync(request);

            //ModelInfo(response);

            return Task.CompletedTask;
        }

        // ProjectorInfo Output Method
        //private void ModelInfo(ShowModelResponse response)
        //{
        //    if (response.Info == null)
        //    {
        //        ReceivedHandler?.Invoke($"### Model is null.\n");
        //        return;
        //    }

        //    ReceivedHandler?.Invoke($"### Selected Model Info");
        //    ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");

        //    // ModelInfo 속성 순회
        //    foreach (var property in typeof(ModelInfo).GetProperties())
        //    {
        //        var propertyName = property.Name;

        //        var propertyValue = property.GetValue(response.Info, null);

        //        // Nullable<long> 처리
        //        var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        //        if (underlyingType == typeof(long))
        //        {
        //            propertyValue = propertyValue != null ? FormatHelper.FormatParamSize((long)propertyValue) : "null";
        //        }

        //        if (propertyValue is IDictionary<string, object> dictionary) // ExtraInfo 처리
        //        {
        //            ReceivedHandler?.Invoke($"{propertyName} : ");
        //            foreach (var kvp in dictionary)
        //            {
        //                ReceivedHandler?.Invoke($"   {kvp.Key} : {kvp.Value}");
        //            }
        //        }
        //        else
        //        {
        //            ReceivedHandler?.Invoke($"{propertyName} : {propertyValue?.ToString() ?? "null"}");
        //        }
        //    }

        //    ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");
        //}

        public Task SendMessage(MessageModel message, CancellationToken token = default)
        {
            ///이 서비스 클래스는 서버에 인스턴스가 생성되고, 클라이언트에서 메시지를 요청할 수도 있고, 서버 자체에서 
            ///메시지를 보낼 수 도 있다. 그리고 AI LLM 특성상 응답이 오기전까지 최소 3~4초의 시간이 소요된다. 따라서 
            ///많은 메시지가 한꺼번에 요청이 들어오면 그 메시지들을 큐에 FIFO 타입으로 넣었다가 아래와 같은 상태일때
            /// _isAvailable = true;
            /// StatusdHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
            ///SendTask에서 하나씩 처리한다. 만약을 위해서 Semaphore를 통해 하나씩 처리가 되도록 하는것도 좋은 방법이 되겠지?
            ///

            if (!_setup.IsAvailable) return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(message.Content))
                return Task.CompletedTask;

            lock (_locker)
            {
                // 메시지를 큐에 추가
                _messageQueue.Enqueue(message);
            }

            // 메시지 처리 태스크가 실행 중인지 확인
            ProcessQueue(token);

            return Task.CompletedTask;
        }

        private async void ProcessQueue(CancellationToken token = default)
        {
            // 큐를 비우는 작업이 이미 진행 중인지 확인
            if (_isProcessing)
                return;

            _isProcessing = true;

            try
            {
                while (_messageQueue.Count > 0)
                {
                    MessageModel nextMessage = null;

                    lock (_locker)
                    {
                        if (_messageQueue.Count == 0)
                            return;

                        // 큐에서 메시지를 하나 꺼냄
                        nextMessage = _messageQueue.Dequeue();
                    }

                    // 메시지를 처리 (하나씩 처리하도록 SemaphoreSlim 사용)
                    await _semaphore.WaitAsync(token);
                    try
                    {
                        await SendTask(nextMessage, token);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                _log?.Error($"Error while processing queue: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private Task SendTask(MessageModel model, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {

                    if (string.IsNullOrWhiteSpace(model.Content))
                        throw new Exception("Input message is empty or null.");

                    _isAvailable = false;
                    StatusHandler?.Invoke(EnumOllamaStatus.BUSY);

                    // Process message with Chat.SendAsync
                    var responseBuffer = new StringBuilder();
                    //if (_chat == null) throw new Exception("Chat instance is empty or null.");

                    //var response = _apiClient.Completions.GenerateCompletionAsync(_selectedModel, content, stream: true, keepAlive: -1);
                    //await foreach (var item in response)
                    //{
                    //    if (token.IsCancellationRequested)
                    //    {
                    //        throw new OperationCanceledException("Message processing was cancelled.");
                    //    }
                    //    //_log.Info($"> {item.Response}");

                    //    responseBuffer.Append(item.Response); // Collect tokens into the buffer
                    //}
                    //ReceivedHandler?.Invoke(responseBuffer.ToString());

                    var message = await _chat.SendAsync(message: model.Content, role: MessageRole.User, cancellationToken:token);
                    ReceivedHandler?.Invoke(new MessageModel(model.User, message.Content));

                }
                catch (OperationCanceledException)
                {
                    _log?.Warning("Operation was cancelled.");
                }
                catch (Exception ex)
                {
                    _log?.Error($"Raised {nameof(Exception)} : {ex.Message}");
                }
                finally
                {
                    _isAvailable = true;
                    StatusHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
                }

            }, token);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsAvailable => _isAvailable;
        #endregion
        #region - Attributes -
        private bool _isAvailable = false;
        private object _locker = new object();
        //private Chat? _chat;
        //private OllamaApiClient? _ollama;
        private ILogService _log;
        private SetupModel _setup;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 동시 실행 제한
        private readonly Queue<MessageModel> _messageQueue = new Queue<MessageModel>(); // 메시지 큐
        private bool _isProcessing = false; // 큐 처리 상태 플래그

        private string _selectedModel;
        private OllamaApiClient _apiClient;
        private Chat _chat;

        public event Action<EnumOllamaStatus> StatusHandler;
        public event Action<MessageModel> ReceivedHandler;

        private const string PREFIX = "http://";
        #endregion

    }
}