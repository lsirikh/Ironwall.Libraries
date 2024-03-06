using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Cameras.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Providers.ViewModels
{
    public abstract class CameraBaseViewModelProvider
        : BaseCommonProvider<ICameraBaseViewModel>, ILoadable
    {
        #region - Ctors -
        public CameraBaseViewModelProvider()
        {
            ClassName = nameof(CameraBaseViewModelProvider);
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
                Debug.WriteLine($"Raised Exception in {nameof(Finished)} : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(ICameraBaseViewModel item)
        {
            try
            {
                Debug.WriteLine($"[{item.Id}]{ClassName} was executed({CollectionEntity.Count()})!!!");
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

        public override async Task<bool> UpdatedItem(ICameraBaseViewModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated.Invoke(item);

                return true;
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({ClassName}) : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> DeletedItem(ICameraBaseViewModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted.Invoke(item);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({ClassName}) : ", ex.Message);
                return false;
            }
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
