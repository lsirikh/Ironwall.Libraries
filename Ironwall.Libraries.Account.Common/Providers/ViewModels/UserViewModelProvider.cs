using Ironwall.Framework.Models.Accounts;
using Ironwall.Framework.ViewModels.Account;
using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Account.Common.Providers.ViewModels
{
    public class UserViewModelProvider
        :AccountBaseViewModelProvider
    {
        
        #region - Ctors -
        public UserViewModelProvider(UserProvider provider)
        {
            ClassName = nameof(UserViewModelProvider);
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
                    //Debug.WriteLine($"{nameof(EventProvider_Initialize)}({nameof(DetectionViewModelProvider)}) was executed!!!");
                    foreach (UserModel item in _provider.ToList())
                    {
                        var viewModel = ViewModelFactory.Build<UserViewModel>(item);
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
                    var viewModel = ViewModelFactory.Build<UserViewModel>(item as IUserModel);
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
                    var viewModel = ViewModelFactory.Build<UserViewModel>(item as IUserModel);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault() as IUserViewModel;

                    //Debug.WriteLine($"<<<<<<<<<<<<{ClassName} {nameof(Provider_Updated)}>>>>>>>>>>>>>>>>>");
                    //Debug.WriteLine($"{(searchedItem as IUserViewModel).MappingGroup}=====>{viewModel.MappingGroup}변경 ");
                    if (searchedItem != null)
                    {
                        searchedItem.IdUser  = viewModel.IdUser;
                        searchedItem.Password  = viewModel.Password;
                        searchedItem.Name  = viewModel.Name;
                        searchedItem.EmployeeNumber  = viewModel.EmployeeNumber;
                        searchedItem.Birth  = viewModel.Birth;
                        searchedItem.Phone  = viewModel.Phone;
                        searchedItem.Address  = viewModel.Address;
                        searchedItem.EMail  = viewModel.EMail;
                        searchedItem.Image  = viewModel.Image;
                        searchedItem.Position  = viewModel.Position;
                        searchedItem.Department  = viewModel.Department;
                        searchedItem.Company  = viewModel.Company;
                        searchedItem.Used  = viewModel.Used;
                        searchedItem.Level  = viewModel.Level;
                        //Debug.WriteLine($"변경된 이름 ====>{searchedItem.MappingGroup}");
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
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
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
        private UserProvider _provider;
        #endregion
    }
}
