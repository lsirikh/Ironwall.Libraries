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
            if (typeof(T).GetConstructor(new[] { typeof(IMapModel) }) == null)
            {
                throw new InvalidOperationException($"The type {typeof(T)} does not have a constructor that accepts IMapModel.");
            }

            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }


        public static T Build<T>(ISymbolModel model) where T : class, new()
        {
            if (typeof(T).GetConstructor(new[] { typeof(ISymbolModel) }) == null)
            {
                throw new InvalidOperationException($"The type {typeof(T)} does not have a constructor that accepts ISymbolModel.");
            }

            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IShapeSymbolModel model) where T : class
        {
            if (typeof(T).GetConstructor(new[] { typeof(IShapeSymbolModel) }) == null)
            {
                throw new InvalidOperationException($"The type {typeof(T)} does not have a constructor that accepts IShapeSymbolModel.");
            }

            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IObjectShapeModel model) where T : class
        {
            if (typeof(T).GetConstructor(new[] { typeof(IObjectShapeModel) }) == null)
            {
                throw new InvalidOperationException($"The type {typeof(T)} does not have a constructor that accepts IObjectShapeModel.");
            }

            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

    }
}
