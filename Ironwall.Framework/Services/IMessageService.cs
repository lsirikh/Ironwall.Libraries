
using Ironwall.Libraries.Base.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Ironwall.Framework.Services
{
    public interface IMessageService
        : IService
    {
        public event EventHandler<ChannelMessage> Channel1EventHandler;
        public event EventHandler<ChannelMessage> Channel2EventHandler;

        public event EventHandler<ChannelMessage> RedisSubscribeEvent;
    }
}
