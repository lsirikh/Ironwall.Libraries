using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Redis.Models;
using System;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Redis.Services
{
    public interface IMessageService<T> : IService
    {
        public T Connect(RedisSetupModel setupModel);
        public Task<T> ConnectAsync(RedisSetupModel setupModel);
        public Task PublishAsync(string channel, string msg);
        //event EventHandler<ChannelMessage> ChannelEventHandler;
        //event EventHandler<ChannelMessage> RedisSubscribeEvent;

        //동기식 수신 이벤트 핸들러
        event EventHandler<MessageArgsModel> RedisSubscribeEvent;
        //비동기식 수신 이벤트 핸들러
        event Func<MessageArgsModel, Task> RedisSubscribeEventAsync;

    }
}