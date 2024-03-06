using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Events.Providers
{
    /*public abstract class EventBaseProvider
        : EntityCollectionProvider<IMetaEventModel>
    {
        #region - Overrides -
        public override void Add(IMetaEventModel item)
        {
            try
            {
                lock (_locker)
                {
                    //Debug.WriteLine($"Added Item({item.Id})");
                    CollectionEntity.Insert(0, item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Add)} : " ,ex.Message);
            }
        }
        #endregion 

        public async Task<bool> Finished()
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

                Debug.WriteLine($"Raised Exception in {nameof(Finished)}({nameof(EventBaseProvider)}) : ", ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertedItem(IMetaEventModel item)
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

                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)}({nameof(EventBaseProvider)}) : ", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdatedItem(IMetaEventModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if(searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({nameof(EventBaseProvider)}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> DeletedItem(IMetaEventModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if(searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({nameof(EventBaseProvider)}) : ", ex.Message);
                return false;
            }
            return true;
        }

        public event RefreshItems Refresh;
        public event Insert Inserted;
        public event Update Updated;
        public event Delete Deleted;

        public delegate Task<bool> RefreshItems();
        public delegate Task<bool> Update(IMetaEventModel item);
        public delegate Task<bool> Insert(IMetaEventModel item);
        public delegate Task<bool> Delete(IMetaEventModel item);
    }*/

    public abstract class EventBaseProvider
        : BaseCommonProvider<IMetaEventModel>
    {
        #region - Ctors -
        public EventBaseProvider()
        {
            ClassName = nameof(EventBaseProvider);
        }
        #endregion
        #region - Implementation of Interface -
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
                Debug.WriteLine($"Raised Exception in {nameof(Finished)}({nameof(ClassName)}) : ", ex.Message);
                return false;
            }
        }

        public override async Task<bool> InsertedItem(IMetaEventModel item)
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

                Debug.WriteLine($"Raised Exception in {nameof(InsertedItem)}({nameof(ClassName)}) : ", ex.Message);
                return false;
            }
        }
        public override async Task<bool> UpdatedItem(IMetaEventModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    searchedItem = item;

                if (Updated == null)
                    return false;

                bool ret = await Updated.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({nameof(ClassName)}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(IMetaEventModel item)
        {
            try
            {
                var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                if (searchedItem != null)
                    Remove(searchedItem);

                if (Deleted == null)
                    return false;

                bool ret = await Deleted.Invoke(item);
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({nameof(ClassName)}) : ", ex.Message);
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
