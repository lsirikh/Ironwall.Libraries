using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Events;

namespace Ironwall.Libraries.Account.Common.Providers.ViewModels
{
    public class SessionViewModelProvider
        : AccountBaseViewModelProvider
    {
        
        #region - Ctors -
        public SessionViewModelProvider(SessionProvider provider)
        {
            ClassName = nameof(SessionViewModelProvider);
            _provider = provider;

            _provider.Refresh += Provider_Refresh;
            _provider.Inserted += Provider_Inserted;
            _provider.Updated += Provider_Updated;
            _provider.Deleted += Provider_Deleted;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task<bool> Provider_Refresh()
        {
            return Task.Run(() =>
            {
                try
                {
                    Clear();
                    foreach (LoginSessionModel item in _provider.ToList())
                    {
                        var viewModel = ViewModelFactory.Build<LoginSessionViewModel>(item);
                        Add(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Refresh)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;
            });
        }

        private Task<bool> Provider_Inserted(IAccountBaseModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<LoginSessionViewModel>(item as ILoginSessionModel);
                    Add(viewModel, 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Inserted)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }

        private Task<bool> Provider_Updated(IAccountBaseModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = ViewModelFactory.Build<LoginSessionViewModel>(item as ILoginSessionModel);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                    {
                        searchedItem.Id = viewModel.Id;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Updated)}({ClassName}) : {ex.Message}");
                    return false;
                }
                return true;
            });
        }

        private Task<bool> Provider_Deleted(IAccountBaseModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t  => (t as ILoginSessionViewModel).Token == (item as ILoginSessionModel).Token).FirstOrDefault();
                    if (searchedItem != null)
                        Remove(searchedItem);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Deleted)}({ClassName}) : {ex.Message}");
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
        private SessionProvider _provider;
        #endregion
    }
}
