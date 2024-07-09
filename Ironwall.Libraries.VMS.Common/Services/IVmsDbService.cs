using Ironwall.Framework.Models.Vms;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.VMS.Common.Services
{
    public interface IVmsDbService
    {
        Task DeleteVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IVmsMappingModel> FetchVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IVmsMappingModel>> FetchVmsApiMappings(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IVmsSensorModel> FetchVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IVmsSensorModel>> FetchVmsApiSensors(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IVmsApiModel> FetchVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IVmsApiModel>> FetchVmsApiSettings(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<int?> GetMaxId(string tableName);
        Task<IVmsMappingModel> SaveVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveVmsApiMappings(List<IVmsMappingModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IVmsSensorModel> SaveVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveVmsApiSensors(List<IVmsSensorModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IVmsApiModel> SaveVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveVmsApiSettings(List<IVmsApiModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateVmsApiMapping(IVmsMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateVmsApiSensor(IVmsSensorModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateVmsApiSetting(IVmsApiModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
    }
}