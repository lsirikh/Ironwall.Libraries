using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Event.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public class PreEventProvider
    : EntityCollectionProvider<PreEventViewModel>
    {

        #region - Overrides -
        public override void Add(PreEventViewModel item)
        {
            try
            {
                lock (_locker)
                {
                    CollectionEntity.Insert(0, item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
