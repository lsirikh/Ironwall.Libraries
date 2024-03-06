﻿using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Ironwall.Libraries.Map.UI.ViewModels;
using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.Common.Providers.Models;
using System.Threading;
using Ironwall.Libraries.Map.Common.Helpers;

namespace Ironwall.Libraries.Map.UI.Providers.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 1:16:09 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ObjectShapeBaseViewModelProvider<T> : SymbolBaseViewModelProvider<T> where T: ObjectShapeViewModel
    {

        #region - Ctors -
        public ObjectShapeBaseViewModelProvider(SymbolProvider provider)
        {
            ClassName = nameof(ObjectShapeBaseViewModelProvider<T>);
            _provider = provider;
        }
        #endregion
        #region - Implementation of Interface -
        public override Task<bool> Initialize(CancellationToken token = default)
        {
            _provider.Refresh += Provider_Initialize;
            _provider.Inserted += Provider_Insert;
            _provider.Updated += Provider_Update;
            _provider.Deleted += Provider_Delete;

            return Provider_Initialize();
        }

        public override void Uninitialize()
        {
            _provider.Refresh -= Provider_Initialize;
            _provider.Inserted -= Provider_Insert;
            _provider.Updated -= Provider_Update;
            _provider.Deleted -= Provider_Delete;
            _provider = null;

            Clear();

            GC.Collect();
        }
        #endregion
        #region - Overrides -
       
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task<bool> Provider_Initialize()
        {
            return Task.Run(() =>
            {
                try
                {
                    Clear();

                    foreach (var item in _provider.OfType<IObjectShapeModel>().ToList())
                    {
                        if (!(SymbolHelper.IsObjectCategory(item.TypeShape))) continue;

                        var viewModel = MapViewModelFactory.Build<T>(item);
                        //viewModel.ActivateAsync();
                        Add(viewModel);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Initialize)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;
            });
        }

        private Task<bool> Provider_Insert(ISymbolModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = MapViewModelFactory.Build<T>((IObjectShapeModel)item);
                    Add(viewModel, 0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Insert)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }
        private Task<bool> Provider_Update(ISymbolModel item)
        {
            return Task.Run(() =>
            {
                try
                {
                    var viewModel = MapViewModelFactory.Build<T>((IObjectShapeModel)item);
                    //viewModel.ActivateAsync();
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();

                    if (searchedItem != null)
                        searchedItem = viewModel;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Update)}({ClassName}) : {ex.Message}");
                    return false;
                }

                return true;

            });
        }

        private Task<bool> Provider_Delete(ISymbolModel item)
        {
            bool ret = false;
            return Task.Run(() =>
            {
                try
                {
                    var searchedItem = CollectionEntity.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (searchedItem != null)
                    {
                        Remove(searchedItem);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception in {nameof(Provider_Delete)}({ClassName}) : {ex.Message}");
                    return ret;
                }
                return ret;
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        protected SymbolProvider _provider;
        #endregion
    }
}
