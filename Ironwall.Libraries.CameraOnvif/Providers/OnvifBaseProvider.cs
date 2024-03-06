using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.CameraOnvif.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Ironwall.Libraries.CameraOnvif.Providers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 2:38:36 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class OnvifBaseProvider : BaseCommonProvider<IOnvifModel>
    {

        #region - Ctors -
        public OnvifBaseProvider()
        {
            ClassName = nameof(OnvifBaseProvider);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public override async Task<bool> Finished()
        {
            try
            {
                Debug.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}]{nameof(Finished)}({ClassName}) was executed!!!");
                if (Refresh == null)
                    return false;

                bool ret = await Refresh?.Invoke();
                return ret;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Finished)} : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(IOnvifModel item)
        {
            try
            {
                //Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
                Add(item);

                if (Inserted == null)
                    return false;

                bool ret = await Inserted?.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)} : ", ex.Message);
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(IOnvifModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t == item).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated?.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({nameof(IOnvifModel)}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(IOnvifModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t == item).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted?.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({nameof(ICameraMappingModel)}) : ", ex.Message);
                return false;
            }
            return true;
        }

        public Task<bool> ClearData()
        {
            return Task.Run(async () =>
            {
                try
                {
                    Clear();
                    await Finished();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        public override event RefreshItems Refresh;
        public override event Insert Inserted;
        public override event Update Updated;
        public override event Delete Deleted;
        #endregion
    }
}
