
using Ironwall.Libraries.Base.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ironwall.Framework.Services
{
    interface IMessageService
        : IService
    {   
        event EventHandler<ChannelMessage> Channel1EventHandler;
        event EventHandler<ChannelMessage> Channel2EventHandler;
        //event EventHandler<ChannelMessage> Channel3EventHandler;
        //event EventHandler<ChannelMessage> Channel4EventHandler;
        //event EventHandler<ChannelMessage> Channel5EventHandler;
        //event EventHandler<ChannelMessage> Channel6EventHandler;

        event EventHandler<ChannelMessage> RedisSubscribeEvent;
    }
}
