using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Event.UI.ViewModels;
using Ironwall.Libraries.Events.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public class PostEventProvider
         : EntityCollectionProvider<PostEventViewModel>
    {
        #region - Ctors - 
        public PostEventProvider(EventSetupModel setupModel)
        {
            SetupModel = setupModel;
        }
        #endregion

        #region - Overrides -
        public override void Add(PostEventViewModel item)
        {
            try
            {
                lock (_locker)
                {
                    CollectionEntity.Insert(0, item);
                    ClearRange(SetupModel.LengthMaxEventPrev, SetupModel.LengthMinEventPrev);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region - Procedures -
        public void ClearRange(int max = 255, int min = 20)
        {
            try
            {
                if (CollectionEntity.Count < max)
                    return;

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

        #region - Properties -
        private EventSetupModel SetupModel { get; }
        #endregion
    }
}
