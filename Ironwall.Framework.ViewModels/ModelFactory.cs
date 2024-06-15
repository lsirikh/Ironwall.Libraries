using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public static class ModelFactory
    {
        #region - Static Base Procedures -
        public static T Build<T>() where T : class, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            return instance;
        }

        public static T Build<T>(string id) where T : class, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] {id});
            return instance;
        }
        #endregion

        #region - From EventMapper To EventModel -
        public static T Build<T>(IConnectionEventMapper model, IBaseDeviceModel device) where T : ConnectionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, device });
            return instance;
        }
        public static T Build<T>(IDetectionEventMapper model, IBaseDeviceModel device) where T : DetectionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, device });
            return instance;
        }

        public static T Build<T>(IMalfunctionEventMapper model, IBaseDeviceModel device) where T : MalfunctionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, device });
            return instance;
        }

        public static T Build<T>(IContactEventMapper model, IBaseDeviceModel device) where T : ContactEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, device });
            return instance;
        }

        public static T Build<T>(IReportEventMapper model, IMetaEventModel eventModel) where T : ActionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, eventModel });
            return instance;
        }
        #endregion

        #region - From EventViewModel To EventModel -
        public static T Build<T>(IConnectionEventViewModel model) where T : ConnectionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
        
        public static T Build<T>(IDetectionEventViewModel model) where T : DetectionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IMalfunctionEventViewModel model) where T : MalfunctionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IContactEventViewModel model) where T : ContactEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IActionEventViewModel model) where T : ActionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
        #endregion

        #region - From EventRequestModel To EventModel -

        public static T Build<T>(IConnectionRequestModel model, IBaseDeviceModel deviceModel) where T : ConnectionRequestModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, deviceModel });
            return instance;
        }
        public static T Build<T>(IDetectionRequestModel model, IBaseDeviceModel deviceModel) where T : DetectionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, deviceModel });
            return instance;
        }


        public static T Build<T>(IMalfunctionRequestModel model, IBaseDeviceModel deviceModel) where T : MalfunctionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, deviceModel });
            return instance;
        }

        public static T Build<T>(IContactRequestModel model, IBaseDeviceModel deviceModel) where T : ContactEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, deviceModel });
            return instance;
        }

        public static T Build<T>(IActionRequestModel model, IMetaEventModel eventModel) where T : ActionEventModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, eventModel });
            return instance;
        }

        #endregion

        #region - From DeviceMapper To DeviceModel -
        /// <summary>
        /// Create Base Type Device Model Instance
        /// </summary>
        /// <typeparam name="T">BaseDeviceModel</typeparam>
        /// <param name="model">IDeviceMapperBase is required</param>
        /// <returns></returns>
        public static T Build<T>(IDeviceMapperBase model) where T : BaseDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(int controller, int sensor, int camera, DateTime date) where T : DeviceDetailModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { controller, sensor, camera, date });
            return instance;
        }


        public static T Build<T>(IDeviceInfoTableMapper model) where T : DeviceDetailModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }



        /// <summary>
        /// Create ControllerDeviceModel Type Device Model Instance
        /// </summary>
        /// <typeparam name="T">ControllerDeviceModel</typeparam>
        /// <param name="model">IControllerTableMapper is required</param>
        /// <returns></returns>
        public static T Build<T>(IControllerTableMapper model) where T : ControllerDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        /// <summary>
        /// Create SensorDeviceModel Type Device Model Instance
        /// </summary>
        /// <typeparam name="T">SensorDeviceModel</typeparam>
        /// <param name="model">IDeviceMapperBase is required</param>
        /// <param name="device">IControllerDeviceModel is required</param>
        /// <returns></returns>
        public static T Build<T>(IDeviceMapperBase model, IControllerDeviceModel device) where T : SensorDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, device });
            return instance;
        }

        /// <summary>
        /// Create CameraDeviceModel Type Device Model Instance
        /// </summary>
        /// <typeparam name="T">CameraDeviceModel</typeparam>
        /// <param name="model">ICameraTableMapper is required</param>
        /// <returns></returns>
        public static T Build<T>(ICameraTableMapper model, List<CameraPresetModel> presets, List<CameraProfileModel> profiles) where T : CameraDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, presets, profiles });
            return instance;
        }
        
        public static T Build<T>(IPresetTableMapper model) where T : CameraPresetModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model});
            return instance;
        }

        public static T Build<T>(IProfileTableMapper model) where T : CameraProfileModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(string id, string group, SensorDeviceModel sensor, CameraPresetModel preset1, CameraPresetModel preset2) where T : CameraMappingModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { id, group, sensor, preset1, preset2 });
            return instance;
        }

        public static T Build<T>(IMappingTableMapper model, SensorDeviceModel sensor, CameraPresetModel preset1, CameraPresetModel preset2) where T : CameraMappingModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model, sensor, preset1, preset2 });
            return instance;
        }


        public static T Build<T>(int mappings, DateTime update) where T : MappingInfoModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { mappings, update });
            return instance;
        }
        #endregion

        #region - From DeviceViewModel To DeviceModel -
        /// <summary>
        /// Create ControllerDeviceModel Type Device Model Instance From DeviceViewModel
        /// </summary>
        /// <typeparam name="T">ControllerDeviceModel</typeparam>
        /// <param name="model">IControllerDeviceViewModel is required</param>
        /// <returns></returns>
        public static T Build<T>(IControllerDeviceViewModel model) where T : ControllerDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        /// <summary>
        /// Create SensorDeviceModel Type Device Model Instance From DeviceViewModel
        /// </summary>
        /// <typeparam name="T">SensorDeviceModel</typeparam>
        /// <param name="model">ISensorDeviceViewModel is required</param>
        /// <returns></returns>
        public static T Build<T>(ISensorDeviceViewModel model) where T : SensorDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model});
            return instance;
        }

        /// <summary>
        /// Create CameraDeviceModel Type Device Model Instance From DeviceViewModel
        /// </summary>
        /// <typeparam name="T">CameraDeviceModel</typeparam>
        /// <param name="model">ICameraDeviceViewModel is required</param>
        /// <returns></returns>
        public static T Build<T>(ICameraDeviceViewModel model) where T : CameraDeviceModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
        #endregion

        #region - From DeviceRequestModel To DeviceModel -

        #endregion

        #region - RequestModel / ResponseModel -
        /*public static T Build<T>(string id) where T : ControllerInfoRequestModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { id });
            return instance;
        }

        public static T Build<T>(IControllerInfoResponseModel model) where T : List<ControllerDeviceModel>, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }*/
        #endregion

        #region - From Brk To RequestModel -
        //public static T Build<T>(int result) where T : DetectionDetailModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { result });
        //    return instance;
        //}

        //public static T Build<T>(int reason, int fStart, int fEnd, int SStart, int SEnd) where T : MalfunctionDetailModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { reason, fStart, fEnd, SStart, SEnd });
        //    return instance;
        //}

        //public static T Build<T>(int readWrite, int contactNumber, int contactSignal) where T : ContactDetailModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { readWrite, contactNumber, contactSignal });
        //    return instance;
        //}

        //public static T Build<T>(BrkConnection model) where T : ConnectionRequestModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
        //    return instance;
        //}

        //public static T Build<T>(BrkDectection model) where T : DetectionRequestModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
        //    return instance;
        //}

        //public static T Build<T>(BrkMalfunction model) where T : MalfunctionRequestModel, new()
        //{
        //    var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
        //    return instance;
        //}
        #endregion

        #region - From Requset To Account Model -
        public static T Build<T>(IAccountEditRequestModel model) where T : UserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] 
            {
                model.Details.Id,
                model.Details.IdUser,
                model.Details.Password,
                model.Details.Name,
                model.Details.EmployeeNumber,
                model.Details.Birth,
                model.Details.Phone,
                model.Details.Address,
                model.Details.EMail,
                model.Details.Image,
                model.Details.Position,
                model.Details.Department,
                model.Details.Company,
                model.Details.Level,
                model.Details.Used 
            });
            return instance;
        }

        public static T Build<T>(IAccountRegisterRequestModel model) where T : UserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[]
            {
                model.Id,
                model.IdUser,
                model.Password,
                model.Name,
                model.EmployeeNumber,
                model.Birth,
                model.Phone,
                model.Address,
                model.EMail,
                model.Image,
                model.Position,
                model.Department,
                model.Company,
                model.Level,
                model.Used
            });
            return instance;
        }

        public static T Build<T>(IAccountInfoResponseModel model) where T : UserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[]
            {
                model.Details.Id,
                model.Details.IdUser,
                model.Details.Password,
                model.Details.Name,
                model.Details.EmployeeNumber,
                model.Details.Birth,
                model.Details.Phone,
                model.Details.Address,
                model.Details.EMail,
                model.Details.Image,
                model.Details.Position,
                model.Details.Department,
                model.Details.Company,
                model.Details.Level,
                model.Details.Used
            });
            return instance;
        }
        #endregion

        #region - From Response To Account Model -
        public static T Build<T>(IUserModel model) where T : UserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(ILoginResultModel model) where T : LoginSessionModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        #endregion

        #region - From Data to Account Model-
        public static T Build<T>(
            int id,
            string userId, 
            string userPass, 
            string token, 
            string timeCreated, 
            string timeExpired) where T : LoginSessionModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] 
            { 
                id,
                userId,
                userPass,
                token,
                timeCreated,
                timeExpired,
            });
            return instance;
        }

        public static T Build<T>(
            string userId, 
            int userLevel, 
            int clientId, 
            int mode, 
            string timeCreated) where T : LoginUserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[]
            {
                userId,
                userLevel,
                clientId,
                mode,
                timeCreated,
            });
            return instance;
        }
        #endregion

        #region - From Account ViewModel to Account Model-
        public static T Build<T>(IUserViewModel model) where T : UserModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[]
            {
                model.Id,
                model.IdUser,
                model.Password,
                model.Name,
                model.EmployeeNumber,
                model.Birth,
                model.Phone,
                model.Address,
                model.EMail,
                model.Image,
                model.Position,
                model.Department,
                model.Company,
                model.Level,
                model.Used
            });
            return instance;
        }
        #endregion

        #region - From SymbolMapper to SymbolModel

        public static T Build<T>(int map, int symbol, int shapeSymbol, int objectShape, DateTime date) where T : SymbolDetailModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { map, symbol, shapeSymbol, objectShape, date });
            return instance;
        }


        public static T Build<T>(ISymbolInfoTableMapper model) where T : SymbolDetailModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }


        #endregion
    }
}
