using Ironwall.Libraries.Base.Services;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Ironwall.Framework.Services
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 8/26/2024 11:06:03 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public abstract class GeneralMessageService
        : TaskService, IMessageService
    {
        #region - Ctors -
        public GeneralMessageService(ISubscriber subscriber, string nameChannel = default)
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
                RedisChannel patternChannel;
                // RedisChannel with Pattern
                if (!string.IsNullOrEmpty(NameChannel))
                {
                    patternChannel = RedisChannel.Pattern($"{NameChannel}*");
                }
                else
                {
                    patternChannel = RedisChannel.Pattern("Stream*");
                }
                
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