using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/19/2023 3:54:24 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioBaseViewModel<T> : SelectableBaseViewModel, IAudioBaseViewModel<T> where T : IAudioBaseModel
    {

        #region - Ctors -
        public AudioBaseViewModel(T model)
        {
            _model = model;
        }
        #endregion
        #region - Implementation of Interface -
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
        public int Id => _model.Id;

        public T Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }
        #endregion
        #region - Attributes -
        protected T _model;

        
        #endregion
    }
}
