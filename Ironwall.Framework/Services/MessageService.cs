using Ironwall.Libraries.Base.Services;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace Ironwall.Framework.Services
{
    public abstract class MessageService 
        : TaskService, IMessageService
    {
        #region - Ctors -
        public MessageService(ISubscriber subscriber, string nameChannel)
        {
            Subscriber = subscriber;
            NameChannel = nameChannel;
        }
        #endregion

        #region - Implements abstracts -
        protected override async Task RunTask(CancellationToken token = default)
        {
            await Task.Run(delegate { RegisterSubscribers(); }, token);
        }

        protected override Task ExitTask(CancellationToken token = default)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region - Procedures -
        protected virtual void RegisterSubscribers()
        {
            try
            {
                // RedisChannel with Pattern
                RedisChannel patternChannel = RedisChannel.Pattern("Stream*");
                Subscriber.Subscribe(patternChannel, CommandFlags.PreferMaster).OnMessage(channelMessage =>
                {
                    RedisSubscribeEvent?.Invoke(this, channelMessage);
                });

            }
            catch (Exception)
            {

                throw;
            }
            //Subscriber.Subscribe(NameChannel).OnMessage((message) =>
            //        Channel1EventHandler?.Invoke(this, message));


        }
        #endregion

        #region - Delegators for EventHandlers -
        public event EventHandler<ChannelMessage> Channel1EventHandler;
        public event EventHandler<ChannelMessage> Channel2EventHandler;

        public event EventHandler<ChannelMessage> RedisSubscribeEvent;
        #endregion

        #region - Properties -
        protected ISubscriber Subscriber { get; }
        private string NameChannel { get; }
        #endregion

        
    }
}
