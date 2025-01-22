using Caliburn.Micro;
using ControlzEx.Standard;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Api.Ollama.Enums;
using Ironwall.Libraries.Api.Ollama.Models;
using Ironwall.Libraries.Api.Ollama.Services;
using Ironwall.Libraries.Base.Services;
using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Ironwall.Libraries.Dotnet.Ollama.Ui.ViewModels;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 1:53:15 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public class ChatBoxViewModel : BaseViewModel
{
    #region - Ctors -
    public ChatBoxViewModel(IEventAggregator eventAggregator
                            , ILogService log
                            , OllamaAiService ollama) : base (eventAggregator, log)
    {
        _ollama = ollama;
    }
    #endregion
    #region - Implementation of Interface -
    #endregion
    #region - Overrides -
    protected override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        _ollama.StatusHandler += Ollama_StatusHandler;
        _ollama.ReceivedHandler += Ollama_ReceivedHandler;
        LoadText();
        Visibility = false;
        IsReady = true;
        //await RunOllama(cancellationToken).ConfigureAwait(false);
    }

    
    #endregion
    #region - Binding Methods -
    #endregion
    #region - Processes -
    //private async Task RunOllama(CancellationToken cancellationToken)
    //{
    //    var prefix = "http://";
    //    var ipAddress = "192.168.202.195";
    //    var port = 11434;
    //    var uri_string = $"{prefix}{ipAddress}:{port}";
    //    var uri = new Uri(uri_string);
    //    _ollama = new OllamaApiClient(uri);

    //    try
    //    {
    //        var result = await _ollama.IsRunningAsync();
    //        if (result == false) throw new Exception($"");
    //        var models = await _ollama.ListLocalModelsAsync();
    //        AppendTextToRichTextBox($"### Model Lists");
    //        AppendTextToRichTextBox($"--------------------------------------------------------------------------------------------------------------");
    //        foreach (var item in models)
    //        {

    //            AppendTextToRichTextBox($"{item.Name}({FormatHelper.FormatFileSize(item.Size)}) - {item.ModifiedAt}");
    //        }
    //        AppendTextToRichTextBox($"--------------------------------------------------------------------------------------------------------------");

    //        // select a model which should be used for further operations
    //        var model = "phi4:latest";
    //        //_ollama.SelectedModel = "llama3.2:3b";
    //        _ollama.SelectedModel = model;
    //        var request = new ShowModelRequest() { Model = model };
    //        var response = await _ollama.ShowModelAsync(request);
    //        DisplayProjectorInfo(response);
    //        _chat = new Chat(_ollama);

    //        //var preDefinedTemplate =
    //        //"""
    //        //넌 이제부터 관제 프로그램의 AI Assistant야. 관제 프로그램은 군사 시설, 항만, 발전소 등에 설치되는 외곽 울타리 시스템 PIDS에 설치되는 보안 솔루션이야. 관제 프로그램 개발업체는 Sensorway라는 업체이고, 대한민국 고양시 삼송동 삼송 테크노벨리에 위치하고 있어. 대표이사는 엄두섭이고, 고려대학교 교수를 중임하고 있어. 너는 다음 룰을 지켜서 말해야되 1. "AI 관제 분석 결과 ~" 하고 대답을 해야되, 2. 너는 대답의 길이를 최대 50자를 넘어가서는 안되, 3. 너는 관제 프로그램과 미리 학습된 내용이외에 정치, 사회, 종교 등에 대한 발언은 하면 절대 안되 너는 관제 프로그램의 AI Assistant라는 것을 절대 잊어서는 안돼. 4. 탐지 메시지는 다음과 같은 형태로 제공될 거야. 다음의 내용을 분석해서 오탐인지 실제 침입자에 의한 탐지인지 분석해서 가이드를 해줘야되, {"command":"EVENT_DETECTION_REQUEST","detail":{"id":813,"group_event":"1","device":{"id":22,"device_number":5,"device_group":5,"device_name":"5번_센서","device_type":"SmartSensor","controller":{"id":1,"device_number":0,"device_group":1,"device_name":"1번_제어기","device_type":"Controller","ip_address":"192.168.1.1","version":"v1.0","ip_port":80},"version":"v1.0"},"type_event":"Intrusion","result":6,"status":"False","datetime":"2025-01-13T15:03:27.41"}} result:6은 진동센서에 의한 탐지를 말하고, status는 False는 아직 조치보고가 되지 않는 상태를 말하는 것이야. 추가 {"command":"EVENT_DETECTION_REQUEST","detail":{"id":814,"group_event":"1","device":{"id":21,"device_number":4,"device_group":4,"device_name":"4번_센서","device_type":"SmartSensor","controller":{"id":1,"device_number":0,"device_group":1,"device_name":"1번_제어기","device_type":"Controller","ip_address":"192.168.1.1","version":"v1.0","ip_port":80},"version":"v1.0"},"type_event":"Intrusion","result":6,"status":"False","datetime":"2025-01-13T15:07:10.22"}}
    //        //""";
    //        //await SendMessage(preDefinedTemplate);
    //    }
    //    catch (Exception ex)
    //    {
    //        _log.Error(ex.Message);
    //    }

    //}

    //// ProjectorInfo Output Method
    //public void DisplayProjectorInfo(ShowModelResponse response)
    //{
    //    if (response.Info == null)
    //    {
    //        AppendTextToRichTextBox($"### Model is null.\n");
    //        return;
    //    }

    //    AppendTextToRichTextBox($"### Selected Model Info");
    //    AppendTextToRichTextBox($"--------------------------------------------------------------------------------------------------------------");

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
    //            AppendTextToRichTextBox($"{propertyName} : ");
    //            foreach (var kvp in dictionary)
    //            {
    //                AppendTextToRichTextBox($"   {kvp.Key} : {kvp.Value}");
    //            }
    //        }
    //        else
    //        {
    //            AppendTextToRichTextBox($"{propertyName} : {propertyValue?.ToString() ?? "null"}");
    //        }
    //    }

    //    AppendTextToRichTextBox($"--------------------------------------------------------------------------------------------------------------");
    //}

    private void LoadText()
    {
        // Sample text to initialize the RichTextBox
       
        _paragraph.Inlines.Add(new Run("### Sensorway AI Assistant..."));
        _paragraph.Inlines.Add(new LineBreak());
        _paragraph.Inlines.Add(new Run("### Send your message what you need to know..."));
        _document.Blocks.Add(_paragraph);

        // Notify the view about the updated content
        RichTextContent = _document;

    }

    // Handle Enter key event
    public async void OnTextBoxKeyDown(EventArgs e)
    {
        if (!(e is KeyEventArgs key)) return;
        if (key.Key == Key.Enter)
        {
            if (_ollama == null) return;
            AppendTextToRichTextBox($"User : {InputTextContent}");
            await _ollama.SendMessage(new MessageModel(null, InputTextContent)).ConfigureAwait(false);
            InputTextContent = string.Empty;
            key.Handled = true; // Prevent the default newline behavior
        }
    }

    

    private void Ollama_ReceivedHandler(MessageModel model)
    {
        AppendTextToRichTextBox($"AI: {model.Content}");
        
    }

    private void Ollama_StatusHandler(EnumOllamaStatus status)
    {
        switch (status) 
        {
            case EnumOllamaStatus.NOT_AVAILABLE:
                Visibility = true;
                break;
            case EnumOllamaStatus.AVAILABLE:
                Visibility = false;
                break;
            case EnumOllamaStatus.BUSY:
                Visibility = true;
                break;
            default:
                break;

        }
    }

    public async void InputButton()
    {
        if (_ollama == null) return;
        AppendTextToRichTextBox($"User : {InputTextContent}");
        await _ollama.SendMessage(new MessageModel(null, InputTextContent)).ConfigureAwait(false);
        InputTextContent = string.Empty;
    }

    private void AppendTextToRichTextBox(string text)
    {
        DispatcherService.Invoke((System.Action)(() =>
        {
            lock (_locker)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(text));
                RichTextContent.Blocks.Add(paragraph);
            }
        }));
    }
    #endregion
    #region - IHanldes -
    #endregion
    #region - Properties -
    

    public FlowDocument RichTextContent
    {
        get => _document;
        set
        {
            _document = value;
            NotifyOfPropertyChange(() => RichTextContent);
        }
    }

    private string _inputTextContent = "";

    public string InputTextContent
    {
        get { return _inputTextContent; }
        set { _inputTextContent = value; NotifyOfPropertyChange(() => InputTextContent); }
    }

    private bool _visibility = false;

    public bool Visibility
    {
        get { return _visibility; }
        set { _visibility = value; NotifyOfPropertyChange(() => Visibility); }
    }

    private bool _isReady = true;

    public bool IsReady
    {
        get { return _isReady; }
        set { _isReady = value; NotifyOfPropertyChange(() => IsReady); }
    }

    #endregion
    #region - Attributes -
    private FlowDocument _document = new FlowDocument();
    private Paragraph _paragraph = new Paragraph();
    private OllamaAiService _ollama;
    //private StringBuilder _responseBuffer = new StringBuilder();
    private object _locker = new object();
    //private Chat? _chat;
    //private OllamaApiClient? _ollama;
    #endregion
}