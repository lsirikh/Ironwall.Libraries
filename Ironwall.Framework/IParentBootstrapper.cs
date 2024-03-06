using System.Threading.Tasks;

namespace Ironwall.Framework
{
    public interface IParentBootstrapper
    {
        Task Start();
        void Stop();
    }
}