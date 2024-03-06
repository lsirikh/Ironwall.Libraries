using Caliburn.Micro;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels
{
    public static class MapViewModelFactory
    {
        public static T Build<T>(IMapModel model) where T : MapViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }


        public static T Build<T>(ISymbolModel model) where T : class, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IShapeSymbolModel model) where T : class
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IObjectShapeModel model) where T : class
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

    }
}
