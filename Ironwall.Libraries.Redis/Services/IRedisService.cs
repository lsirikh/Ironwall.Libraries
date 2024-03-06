namespace Ironwall.Libraries.Redis.Services
{
    public interface IRedisService : IMessageService<IRedisService> 
    {
        string Channel { get;}
    }
}