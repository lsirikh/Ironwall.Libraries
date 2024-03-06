using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.Providers.Models;
using Ironwall.Libraries.Onvif.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.DataProviders
{
    public abstract class OnvifBaseProvider
        : BaseCommonProvider<IOnvifModel>, ILoadable
    {
        #region - Ctors -
        public OnvifBaseProvider()
        {
            ClassName = nameof(OnvifBaseProvider);
        }
        #endregion
        #region - Implementation of Interface -
        public abstract Task<bool> Initialize(CancellationToken token = default);
        public abstract void Uninitialize();
        #endregion
        #region - Overrides -
        public override async Task<bool> Finished()
        {
            try
            {
                if (Refresh == null)
                    return false;

                bool ret = await Refresh.Invoke();
                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Finished)}({ClassName}) : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(IOnvifModel item)
        {
            try
            {
                Add(item);

                if (Inserted == null)
                    return false;

                bool ret = await Inserted.Invoke(item);
                return ret;

            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)}({ClassName}) : ", ex.Message);
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

                bool ret = await Updated.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({ClassName}) : ", ex.Message);
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

                bool ret = await Deleted.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({ClassName}) : ", ex.Message);
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
        #region - Binding Methods -
        #endregion
        #region - Processes -
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
