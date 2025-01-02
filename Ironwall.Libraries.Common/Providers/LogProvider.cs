using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Base.DataProviders;
using Ironwall.Libraries.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Ironwall.Libraries.Common.Providers
{
    public class LogProvider: EntityCollectionProvider<LogModel>
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Add(LogModel item)
        {
            try
            {
                lock (_locker)
                {
                    CollectionEntity.Insert(0, item);
                    AddEvent?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void AddInfo(string msg, string param = null)
        {
            var log = InstanceFactory.Build<LogModel>();

            var logMsg = param != null ? $"[{param}]{msg}" : $"{msg}";
            log.Info(logMsg);

            Add(log);
        }

        public void AddWarning(string msg, string param = null)
        {
            var log = InstanceFactory.Build<LogModel>();

            var logMsg = param != null ? $"[{param}]{msg}" : $"{msg}";
            log.Warning(logMsg);

            Add(log);
        }

        public void AddError(string msg, string param = null)
        {
            var log = InstanceFactory.Build<LogModel>();

            var logMsg = param != null ? $"[{param}]{msg}" : $"{msg}";
            log.Error(logMsg);

            Add(log);
        }

        public void ClearRange(int max = 255, int min = 20)
        {
            try
            {
                if (CollectionEntity.Count < max) return;

                for (int index = max - 1; index > min; --index)
                {
                    var item = CollectionEntity[index];
                    item = null;
                    CollectionEntity.RemoveAt(index);
                }
                GC.Collect(); GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        public event Add_dele AddEvent;
        public delegate void Add_dele();
        #endregion
    }
}
