using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Base.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Devices.Services
{
    public interface IDeviceDbService : IService
    {
        Task CheckTable(string table, int? id = null);
        Task DeleteCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeletePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task DeleteSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraDeviceModel> FetchCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraMappingModel> FetchCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<ICameraMappingModel>> FetchCameraMappings(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<ICameraDeviceModel>> FetchCameras(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IControllerDeviceModel> FetchControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IControllerDeviceModel> FetchControllerDevice(int id, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<IControllerDeviceModel>> FetchControllerDevices(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraPresetModel> FetchPreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<ICameraPresetModel>> FetchPresets(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraProfileModel> FetchProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<ICameraProfileModel>> FetchProfiles(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ISensorDeviceModel> FetchSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<List<ISensorDeviceModel>> FetchSensorDevices(CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<int?> GetDeviceMaxId();
        Task<int?> GetMaxId(string tableName);
        Task<int?> GetOptionMaxId();
        Task<string> GetTableNameForId(int id);
        Task<bool> IsDeviceIdExists(int id);
        Task<bool> IsOptionIdExists(int id);
        Task<ICameraDeviceModel> SaveCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraMappingModel> SaveCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveCameraMappings(List<ICameraMappingModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveCameras(List<ICameraDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<IControllerDeviceModel> SaveControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveControllers(List<IControllerDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraPresetModel> SavePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SavePresets(List<ICameraPresetModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ICameraProfileModel> SaveProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveProfiles(List<ICameraProfileModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task<ISensorDeviceModel> SaveSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task SaveSensors(List<ISensorDeviceModel> models, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateCamera(ICameraDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateCameraMapping(ICameraMappingModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateControllerDevice(IControllerDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdatePreset(ICameraPresetModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateProfile(ICameraProfileModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
        Task UpdateSensorDevice(ISensorDeviceModel model, CancellationToken token = default, TaskCompletionSource<bool> tcs = null);
    }
}