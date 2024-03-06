using Caliburn.Micro;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Ironwall.Framework.Services
{
    public abstract class IccsService : TaskService, IIccsService
    {
        #region - Ctors -
        public IccsService()
        {
        }

        public IccsService(IMessageService messageService)
        {
            MessageService = messageService;
        }
        #endregion

        #region - Abstracts -
        public abstract void BuildLookupTabel();
        public abstract void ProcessDetection(JToken target);
        public abstract void ProcessFault(JToken target);
        public abstract void ProcessConnection(JToken target);
        #endregion

        #region - Implementations for the TaskService's overrides -

        protected override Task RunTask(CancellationToken token = default)
        {
            return Task.Run(() =>
            {
                BuildLookupTabel();
                RegisterEventHandelers();
            });
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.Run(() => 
            { 
                UnregisterEventHandelers();
            });
        }

        private void ProcessChannelMessage(object sender, ChannelMessage channelMessage)
        {
            try
            {
                var channel = channelMessage.Channel;
                var message = channelMessage.Message;

                MessageParser(channel, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        

        public async void MessageParser(string channel, string message)
        {
            try
            {
                switch (channel)
                {
                    case "Channel1":
                        {
                            foreach (var item in JArray.Parse(message))
                            {
                                switch ((EnumEventType)item.Value<int>("command"))
                                {
                                    case EnumEventType.Connection:
                                        await Task.Run(() => ProcessConnection(item));
                                        break;
                                    case EnumEventType.Intrusion:
                                        await Task.Run(() => ProcessDetection(item));
                                        break;
                                    case EnumEventType.Fault:
                                        await Task.Run(() => ProcessFault(item));
                                        break;
                                    case EnumEventType.ContactOn:
                                        break;
                                    case EnumEventType.ContactOff:
                                        break;
                                    case EnumEventType.Action:
                                        break;
                                    case EnumEventType.WindyMode:
                                        break;
                                    default:
                                        throw new TypeAccessException();
                                }
                            }
                        }
                        break;
                    case "Channel2":
                        {
                            foreach (var item in JArray.Parse(message))
                            {
                                switch ((EnumEventType)item.Value<int>("command"))
                                {
                                    case EnumEventType.Intrusion:
                                        break;
                                    case EnumEventType.ContactOn:
                                        break;
                                    case EnumEventType.ContactOff:
                                        break;
                                    case EnumEventType.Connection:
                                        break;
                                    case EnumEventType.Action:
                                        break;
                                    case EnumEventType.Fault:
                                        break;
                                    case EnumEventType.WindyMode:
                                        break;
                                    default:
                                        throw new TypeAccessException();
                                }
                            }
                        }
                        break;
                    case "Channel3":
                        break;
                    case "Channel4":
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(MessageParser)} of {nameof(IccsService)} : {ex.Message}");
            }
        }

        protected virtual async void ProcessChannel1Message(object sender, ChannelMessage message)
        {
            try
            {
                foreach (var item in JArray.Parse(message.Message))
                {
                    switch ((EnumEventType)item.Value<int>("command"))
                    {
                        case EnumEventType.Intrusion:
                            await Task.Run(() => ProcessDetection(item));
                            break;

                        case EnumEventType.Fault:
                            await Task.Run(() => ProcessFault(item));
                            break;

                        case EnumEventType.Connection:
                            await Task.Run(() => ProcessConnection(item));
                            break;

                        case EnumEventType.WindyMode:
                            break;

                        case EnumEventType.ContactOn:
                            break;
                        case EnumEventType.ContactOff:
                            break;
                        default:
                            throw new TypeAccessException();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised {nameof(Exception)} in {nameof(MessageParser)} of {nameof(IccsService)} : {ex.Message}");
            }
        }

        protected virtual void ProcessChannel2Message(object sender, ChannelMessage message)
        {

        }
        #endregion

        #region - Procedures -
        private void RegisterEventHandelers()
        {
            MessageService.RedisSubscribeEvent += ProcessChannelMessage;
            MessageService.Channel1EventHandler += ProcessChannel1Message;
            MessageService.Channel2EventHandler += ProcessChannel2Message;
        }

        private void UnregisterEventHandelers()
        {
            MessageService.RedisSubscribeEvent -= ProcessChannelMessage;
            MessageService.Channel1EventHandler -= ProcessChannel1Message;
            MessageService.Channel2EventHandler -= ProcessChannel2Message;
        }
        #endregion
        #region - Properties -
        private IMessageService MessageService { get; }
        #endregion
    }
}
