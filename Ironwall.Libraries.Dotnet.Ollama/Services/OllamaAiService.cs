using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Dotnet.Ollama.Enums;
using Ironwall.Libraries.Dotnet.Ollama.Helpers;
using Ironwall.Libraries.Dotnet.Ollama.Models;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;
using System;
using System.Text;
using System.Windows;

namespace Ironwall.Libraries.Dotnet.Ollama.Services;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 2:02:10 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public class OllamaAiService : IService
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

        //throw new NotImplementedException();

        await OllamaPreparingTask(token);
    }

    public Task StopAsync(CancellationToken token = default)
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
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

        var uri_string = $"{PREFIX}{_setup.IpAddress}:{_setup.Port}";
        _log.Info($"Try to connect AI Server({uri_string})....");
        var uri = new Uri(uri_string);
        _ollama = new OllamaApiClient(uri);

        _isAvailable = false;
        StatusdHandler?.Invoke(EnumOllamaStatus.NOT_AVAILABLE);

        try
        {
            var result = await _ollama.IsRunningAsync();
            if (result == false) throw new Exception($"");
            var models = await _ollama.ListLocalModelsAsync();
            ReceivedHandler?.Invoke($"### Model Lists");
            ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");
            foreach (var item in models)
            {
                ReceivedHandler?.Invoke($"{item.Name}({FormatHelper.FormatFileSize(item.Size)}) - {item.ModifiedAt}");
            }
            ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");

            // select a model which should be used for further operations
            var model = "phi4:latest";
            //_ollama.SelectedModel = "llama3.2:3b";
            _ollama.SelectedModel = model;
            


            _chat = new Chat(_ollama);

            //var preDefinedTemplate =
            //"""
            //넌 이제부터 관제 프로그램의 AI Assistant야. 관제 프로그램은 군사 시설, 항만, 발전소 등에 설치되는 외곽 울타리 시스템 PIDS에 설치되는 보안 솔루션이야. 관제 프로그램 개발업체는 Sensorway라는 업체이고, 대한민국 고양시 삼송동 삼송 테크노벨리에 위치하고 있어. 대표이사는 엄두섭이고, 고려대학교 교수를 중임하고 있어. 너는 다음 룰을 지켜서 말해야되 1. "AI 관제 분석 결과 ~" 하고 대답을 해야되, 2. 너는 대답의 길이를 최대 50자를 넘어가서는 안되, 3. 너는 관제 프로그램과 미리 학습된 내용이외에 정치, 사회, 종교 등에 대한 발언은 하면 절대 안되 너는 관제 프로그램의 AI Assistant라는 것을 절대 잊어서는 안돼. 4. 탐지 메시지는 다음과 같은 형태로 제공될 거야. 다음의 내용을 분석해서 오탐인지 실제 침입자에 의한 탐지인지 분석해서 가이드를 해줘야되, {"command":"EVENT_DETECTION_REQUEST","detail":{"id":813,"group_event":"1","device":{"id":22,"device_number":5,"device_group":5,"device_name":"5번_센서","device_type":"SmartSensor","controller":{"id":1,"device_number":0,"device_group":1,"device_name":"1번_제어기","device_type":"Controller","ip_address":"192.168.1.1","version":"v1.0","ip_port":80},"version":"v1.0"},"type_event":"Intrusion","result":6,"status":"False","datetime":"2025-01-13T15:03:27.41"}} result:6은 진동센서에 의한 탐지를 말하고, status는 False는 아직 조치보고가 되지 않는 상태를 말하는 것이야. 추가 {"command":"EVENT_DETECTION_REQUEST","detail":{"id":814,"group_event":"1","device":{"id":21,"device_number":4,"device_group":4,"device_name":"4번_센서","device_type":"SmartSensor","controller":{"id":1,"device_number":0,"device_group":1,"device_name":"1번_제어기","device_type":"Controller","ip_address":"192.168.1.1","version":"v1.0","ip_port":80},"version":"v1.0"},"type_event":"Intrusion","result":6,"status":"False","datetime":"2025-01-13T15:07:10.22"}}
            //""";
            await SendMessage("", token);
        }
        catch (Exception ex)
        {
            _log.Error(ex.Message);
        }
        finally
        {
            _isAvailable = true;
            StatusdHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
        }
    }

    public async Task GetModelInfo()
    {
        if (_ollama == null) return;

        var request = new ShowModelRequest() { Model = _ollama.SelectedModel };
        var response = await _ollama.ShowModelAsync(request);

        ModelInfo(response);
    }

    // ProjectorInfo Output Method
    private void ModelInfo(ShowModelResponse response)
    {
        if (response.Info == null)
        {
            ReceivedHandler?.Invoke($"### Model is null.\n");
            return;
        }

        ReceivedHandler?.Invoke($"### Selected Model Info");
        ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");

        // ModelInfo 속성 순회
        foreach (var property in typeof(ModelInfo).GetProperties())
        {
            var propertyName = property.Name;

            var propertyValue = property.GetValue(response.Info, null);

            // Nullable<long> 처리
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (underlyingType == typeof(long))
            {
                propertyValue = propertyValue != null ? FormatHelper.FormatParamSize((long)propertyValue) : "null";
            }

            if (propertyValue is IDictionary<string, object> dictionary) // ExtraInfo 처리
            {
                ReceivedHandler?.Invoke($"{propertyName} : ");
                foreach (var kvp in dictionary)
                {
                    ReceivedHandler?.Invoke($"   {kvp.Key} : {kvp.Value}");
                }
            }
            else
            {
                ReceivedHandler?.Invoke($"{propertyName} : {propertyValue?.ToString() ?? "null"}");
            }
        }

        ReceivedHandler?.Invoke($"--------------------------------------------------------------------------------------------------------------");
    }

    public Task SendMessage(string content, CancellationToken token = default)
    {
        ///이 서비스 클래스는 서버에 인스턴스가 생성되고, 클라이언트에서 메시지를 요청할 수도 있고, 서버 자체에서 
        ///메시지를 보낼 수 도 있다. 그리고 AI LLM 특성상 응답이 오기전까지 최소 3~4초의 시간이 소요된다. 따라서 
        ///많은 메시지가 한꺼번에 요청이 들어오면 그 메시지들을 큐에 FIFO 타입으로 넣었다가 아래와 같은 상태일때
        /// _isAvailable = true;
        /// StatusdHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
        ///SendTask에서 하나씩 처리한다. 만약을 위해서 Semaphore를 통해 하나씩 처리가 되도록 하는것도 좋은 방법이 되겠지?
        ///

        if (string.IsNullOrWhiteSpace(content))
            return Task.CompletedTask;

        lock (_locker)
        {
            // 메시지를 큐에 추가
            _messageQueue.Enqueue(content);
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
                string? nextMessage;

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

    private Task SendTask(string content, CancellationToken token = default)
    {
        return Task.Run(async () =>
        {
            try
            {

                if (string.IsNullOrWhiteSpace(content))
                    throw new Exception("Input message is empty or null.");

                _isAvailable = false;
                StatusdHandler?.Invoke(EnumOllamaStatus.BUSY);

                // Process message with Chat.SendAsync
                var responseBuffer = new StringBuilder();
                if (_chat == null) throw new Exception("Chat instance is empty or null.");
                await foreach (var responseToken in _chat.SendAsync(content))
                {
                    if (token.IsCancellationRequested)
                    {
                        throw new OperationCanceledException("Message processing was cancelled.");
                    }

                    responseBuffer.Append(responseToken); // Collect tokens into the buffer
                }

                ReceivedHandler?.Invoke(responseBuffer.ToString());
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
                StatusdHandler?.Invoke(EnumOllamaStatus.AVAILABLE);
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
    private Chat? _chat;
    private OllamaApiClient? _ollama;
    private ILogService? _log;
    private SetupModel? _setup;

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // 동시 실행 제한
    private readonly Queue<string> _messageQueue = new Queue<string>(); // 메시지 큐
    private bool _isProcessing = false; // 큐 처리 상태 플래그

    public event Action<EnumOllamaStatus> StatusdHandler;
    public event Action<string> ReceivedHandler;

    private const string PREFIX = "http://";
    #endregion

}