﻿using Ironwall.Framework.DataProviders;
using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Providers.ViewModels
{
    public class AccountBaseViewModelProvider
        : BaseCommonProvider<IAccountBaseViewModel>
    {
        #region - Ctors -
        public AccountBaseViewModelProvider()
        {
            ClassName = nameof(AccountBaseViewModelProvider);
        }


        #endregion
        #region - Implementation of Interface -
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

        public override async Task<bool> InsertedItem(IAccountBaseViewModel item)
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
        public override async Task<bool> UpdatedItem(IAccountBaseViewModel item)
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

                Debug.WriteLine($"Raised Exception in {nameof(UpdatedItem)}({ClassName}) : ", ex.Message);
                return false;
            }

            return true;
        }

        public override async Task<bool> DeletedItem(IAccountBaseViewModel item)
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

                Debug.WriteLine($"Raised Exception in {nameof(DeletedItem)}({ClassName}) : ", ex.Message);
                return false;
            }
            return true;
        }

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        //private Task<bool> Provider_Inserted(ILoginSessionModel item)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<bool> Provider_Updated(ILoginSessionModel item)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<bool> Provider_Deleted(ILoginSessionModel item)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<bool> Provider_Refresh()
        //{
        //    throw new NotImplementedException();
        //}
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
