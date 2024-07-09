using Ironwall.Framework.Services;
using System;
using System.Diagnostics;

namespace Ironwall.Framework.DataProviders
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/17/2024 3:10:32 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public abstract class BaseProvider<T> : EntityCollectionProvider<T>
    {
        #region - Ctors -
        protected BaseProvider()
        {
            
        }
        #endregion
        #region - Implementation of Interface -
        public virtual void Add(T item, int index)
        {
            try
            {
                DispatcherService.Invoke((System.Action)(() =>
                {
                    lock (_locker)
                    {
                        CollectionEntity.Insert(index, item);
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(Add)} : ", ex.Message);
            }
        }
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string ClassName { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}