using System;

namespace Ironwall.Libraries.Base.DataProviders
{
    public static class InstanceFactory
    {
        #region - Static Procedures -
        public static T Build<T>() where T : class, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T));
            return instance;
        }
        #endregion
    }
}
