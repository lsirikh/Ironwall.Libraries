using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Base.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Services
{
    public interface IEventDbService : IService
    {
        List<string> TableNames { get; }
        Task CheckTable(string table, int? id = null);
        Task DeleteActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IActionEventModel> FetchActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IActionEventModel>> FetchActionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IDetectionEventModel> FetchDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IDetectionEventModel> FetchDetectionEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IDetectionEventModel>> FetchDetectionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IMalfunctionEventModel> FetchMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IMalfunctionEventModel> FetchMalfunctionEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IMalfunctionEventModel>> FetchMalfunctionEvents(string from = null, string to = null, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IMetaEventModel> FetchMetaEvent(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<int?> GetEventMaxId();
        Task<int?> GetMaxId(string tableName);
        Task<string> GetTableNameForId(int id);
        Task<bool> IsEventIdExists(int id);
        Task<IActionEventModel> SaveActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IDetectionEventModel> SaveDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IMalfunctionEventModel> SaveMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateActionEvent(IActionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateDetectionEvent(IDetectionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateMalfunctionEvent(IMalfunctionEventModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
    }
}