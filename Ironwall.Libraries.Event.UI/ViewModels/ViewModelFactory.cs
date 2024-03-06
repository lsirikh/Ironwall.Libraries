using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/12/2023 11:52:19 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class ViewModelFactory
    {
        public static T Build<T>(IMetaEventModel model) where T : MetaEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IDetectionEventModel model) where T : DetectionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IMalfunctionEventModel model) where T : MalfunctionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }

        public static T Build<T>(IActionEventModel model) where T : ActionEventViewModel, new()
        {
            var instance = (T)Activator.CreateInstance(typeof(T), new object[] { model });
            return instance;
        }
    }
}
