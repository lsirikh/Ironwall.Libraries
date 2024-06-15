using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Tcp.Common.Proivders.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ironwall.Libraries.Tcp.Common.ViewModels;
using Ironwall.Libraries.Tcp.Common.Models;

namespace Ironwall.Libraries.Tcp.Common.Proivders.ViewModels
{
    public class TcpUserViewModelProvider : TcpUserBaseViewModelProvider
    {
        #region - Ctors -
        public TcpUserViewModelProvider(TcpUserProvider provider)

        {
            ClassName = nameof(TcpUserViewModelProvider);
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
                    foreach (ITcpUserModel item in _provider.ToList())
                    {
                        var viewModel = TcpViewModelFactory.Build<TcpUserViewModel>(item);
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

        private Task<bool> Provider_Inserted(ITcpUserModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = TcpViewModelFactory.Build<TcpUserViewModel>(item);
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

        private Task<bool> Provider_Updated(ITcpUserModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = TcpViewModelFactory.Build<TcpUserViewModel>(item);
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    //if (searchedItem != null)
                    //{
                    //    searchedItem.IdUser = viewModel.IdUser;
                    //    searchedItem.Password = viewModel.Password;
                    //    searchedItem.GroupId = viewModel.GroupId;
                    //    searchedItem.EmployeeNumber = viewModel.EmployeeNumber;
                    //    searchedItem.Birth = viewModel.Birth;
                    //    searchedItem.Phone = viewModel.Phone;
                    //    searchedItem.Address = viewModel.Address;
                    //    searchedItem.EMail = viewModel.EMail;
                    //    searchedItem.Image = viewModel.Image;
                    //    searchedItem.Position = viewModel.Position;
                    //    searchedItem.Department = viewModel.Department;
                    //    searchedItem.Company = viewModel.Company;
                    //    searchedItem.Used = viewModel.Used;
                    //    searchedItem.Level = viewModel.Level;
                    //}

                    searchedItem.UserModel = item.UserModel;
                    searchedItem.TcpModel = item.TcpModel;
                    searchedItem.Refresh();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Updated)}({ClassName}) : {ex.Message}");
                    return false;
                }
                return true;
            });
        }

        private Task<bool> Provider_Deleted(ITcpUserModel item)
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
        private TcpUserProvider _provider;
        #endregion

        
    }
}
