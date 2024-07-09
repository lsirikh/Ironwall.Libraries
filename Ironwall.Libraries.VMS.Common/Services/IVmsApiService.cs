using Ironwall.Framework.Models.Vms;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Enums;
using Sensorway.Accounts.Base.Models;
using Sensorway.Events.Base.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.VMS.Common.Services
{
    public interface IVmsApiService : IService
    {
        EnumVmsStatus Status { get; }

        Task ApiLoginProcess();
        Task ApiLogoutProcess();
        Task ApiKeepAliveProcess();
        Task ApiGetEventListProcess();
        Task ApiSetActionEventProcess(IActionEventModel emodel);
        Task<string> ApiLogin(IVmsApiModel model = null, bool isForced = false, TaskCompletionSource<bool> tcs = null);
        Task<string> ApiLogout(ILoginSessionModel model, TaskCompletionSource<bool> tcs = null);
        Task<string> ApiKeepAlive(ILoginSessionModel model, TaskCompletionSource<bool> tcs = null);
        Task<string> ApiGetEventList(ILoginSessionModel model, TaskCompletionSource<bool> tcs = null);
        Task<string> ApiSetActionEvent(ILoginSessionModel model, IActionEventModel emodel, TaskCompletionSource<bool> tcs = null);
    }
}