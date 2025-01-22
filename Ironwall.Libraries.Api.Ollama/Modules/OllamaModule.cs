using Autofac;
using Ironwall.Libraries.Api.Ollama.Models;
using Ironwall.Libraries.Api.Ollama.Services;
using Ironwall.Libraries.Base.Services;
using System;

/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 3:55:15 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
namespace Ironwall.Libraries.Api.Ollama.Modules
{
    public class OllamaModule : Module
    {
        #region - Ctors -
        public OllamaModule(string ipAddress = "192.168.202.195", int port = 11434, bool isAvailable = false, ILogService log = default, int count = 0)
        {
            _log = log;
            _count = count;
            _ipAddress = ipAddress;
            _port = port;
            _isAvailable = isAvailable;
        }
        #endregion
        #region - Implementation of Interface -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                var setupModel = new SetupModel()
                {
                    IpAddress = _ipAddress,
                    Port = _port,
                    IsAvailable = _isAvailable,
                };

                builder.RegisterInstance(setupModel).AsSelf().SingleInstance();
                builder.RegisterType<OllamaAiService>().As<IService>().AsSelf().SingleInstance().WithMetadata("Order", _count);
            }
            catch (Exception ex)
            {
                _log?.Error(ex.Message);
            }
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private int _count;
        private string _ipAddress;
        private int _port;
        private bool _isAvailable;
        #endregion
    }
}