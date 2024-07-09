using System.Threading.Tasks;

namespace Ironwall.Framework
{
    interface IParentBootstrapper
    {
        Task Start();
        void Stop();
    }
}