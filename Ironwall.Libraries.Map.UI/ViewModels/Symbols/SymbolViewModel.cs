using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Libraries.Map.UI.Models.Messages;
using System;
using System.Security.AccessControl;
using System.Windows;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 11:13:14 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/
    public class SymbolViewModel : SymbolBaseViewModel<ISymbolModel>, ISymbolViewModel
    {

        #region - Ctors -
        public SymbolViewModel()
        {
            _model = new SymbolModel();
        }

        public SymbolViewModel(ISymbolModel model) : base(model)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void Dispose()
        {
            _model = new SymbolModel();
            GC.Collect();
        }

        public override async void OnClickSelect(object sender, EventArgs args)
        {
            if (!OnEditable) return;

            IsEditable = true;
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(true, this));
        }

        public override async void OnClickEdit(object sender, EventArgs args)
        {
            IsEditable = true;
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(true, this));
        }

        public override async void OnClickDelete(object sender, EventArgs args)
        {
            IsEditable = false;
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(false, this));
            await _eventAggregator.PublishOnUIThreadAsync(new DeleteShapeMessage(_model));

        }

        public override async void OnClickCopy(object sender, EventArgs args)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new CopyShapeMessage(_model));
        }

        public override async void OnClickExit(object sender, EventArgs args)
        {
            IsEditable = false;
            await _eventAggregator.PublishOnUIThreadAsync(new EditShapeMessage(false, this));
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
