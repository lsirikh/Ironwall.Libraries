using Ironwall.Framework.Models;
using Ironwall.Framework.ViewModels;
using System;

namespace Ironwall.Framework.ViewModels
{
    static class EntityViewModelFactory
    {
        #region - Static Procedures -
        static T Build<T>(IEntityModel model) where T : EntityViewModel, new()
        {
            var viewModel = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return viewModel;
        }
        #endregion
    }
}
