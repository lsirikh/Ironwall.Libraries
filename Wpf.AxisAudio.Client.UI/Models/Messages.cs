using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using Wpf.AxisAudio.Client.UI.ViewModels;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/23/2023 3:39:37 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    /// <summary>
    /// GUI Event
    /// </summary>
    public class OpenAudioGroupingDialogMessageModel
    {
        public OpenAudioGroupingDialogMessageModel(AudioGroupViewModel model)
        {
            ViewModel = model;
        }

        public AudioGroupViewModel ViewModel { get; private set; }
    }

    public class OpenAudioPlayDialogMessageModel
    {
        public OpenAudioPlayDialogMessageModel(AudioViewModel model)
        {
            ViewModel = model;
        }

        public AudioViewModel ViewModel { get; }
    }

    public class OpenAudioSensorMatchingDialogMessageModel
    {
        public OpenAudioSensorMatchingDialogMessageModel()
        {
        }

    }

    public class RefreshAudioSensorSetupMessageModel
    {
        //public RefreshAudioSensorSetupMessageModel(List<SensorModels> models)
        //{
        //    Models = models;
        //}

        //public List<SensorModels> Models { get; }
    }

    public class RefreshAudioNamesMessageModel
    {
        public RefreshAudioNamesMessageModel(List<string> names)
        {
            Names = names;
        }

        public List<string> Names { get; private set; }
    }

    public class CallAudioGroupingDialogMessageModel
    {
        public CallAudioGroupingDialogMessageModel(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
    }

    public class UpdateAxisSymbolServiceMessageModel
    {
        public UpdateAxisSymbolServiceMessageModel(List<AudioModel> models)
        {
            Models = models;
        }

        public List<AudioModel> Models { get; }
    }

    //public class UpdateAxisAudioSymbolMessageModel
    //{
    //    public UpdateAxisAudioSymbolMessageModel(List<AudioModels> models)
    //    {
    //        Models = models;
    //    }

    //    public List<AudioModels> Models { get; }
    //}

    public class RequestSingleSpeakerStreamingMessageModel
    {
        public RequestSingleSpeakerStreamingMessageModel(string name, bool control)
        {
            Name = name;
            Control = control;
        }

        public string Name { get; }
        public bool Control { get; }
    }

    public class RequestGroupSpeakerStreamingMessageModel
    {
        public RequestGroupSpeakerStreamingMessageModel(string name, bool control)
        {
            Name = name;
            Control = control;
        }

        public string Name { get; }
        public bool Control { get; }
    }

    public class RequestClipPlayingMessageModel
    {
        public RequestClipPlayingMessageModel(string name, bool control)
        {
            Name = name;
            Control = control;
        }

        public string Name { get; }
        public bool Control { get; }
    }

    public class RequestGroupClipPlayingMessageModel
    {
        public RequestGroupClipPlayingMessageModel(string name, bool control)
        {
            Name = name;
            Control = control;
        }

        public string Name { get; }
        public bool Control { get; }
    }


}
