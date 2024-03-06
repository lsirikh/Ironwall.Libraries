using Autofac;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Modules
{
    public class RedisModule : Module
    {
        #region - Overrides -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.Register(ctx =>
                {
                    var host = $"{IpAddressRedis}:{PortRedis}";
                    var configuration = ConfigurationOptions.Parse(host);
                    configuration.AllowAdmin = true;
                    configuration.Password = PasswordRedis;
                    return ConnectionMultiplexer.Connect(configuration).GetSubscriber();
                })
                .As<ISubscriber>()
                .SingleInstance();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region - Properties -
        public string IpAddressRedis { get; set; }
        public int PortRedis { get; set; }
        public string PasswordRedis { get; set; }
        #endregion
    }
}
