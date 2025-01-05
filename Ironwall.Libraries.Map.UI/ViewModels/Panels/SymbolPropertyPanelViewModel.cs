using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Map.UI.Models.Messages;
using Ironwall.Libraries.Map.UI.ViewModels.Properties;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/5/2023 3:54:27 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolPropertyPanelViewModel : BaseViewModel
        , IHandle<EditShapeMessage>
    {

        #region - Ctors -
        public SymbolPropertyPanelViewModel(IEventAggregator eventAggregator
                                            , ILogService log
                                            , SymbolPropertyViewModel symbolPropertyViewModel)
                                            : base(eventAggregator, log)
        {
            SymbolPropertyViewModel = symbolPropertyViewModel;
        }


        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await SymbolPropertyViewModel.ActivateAsync();
            await base.OnActivateAsync(cancellationToken);
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await SymbolPropertyViewModel.DeactivateAsync(true);
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        public Task HandleAsync(EditShapeMessage message, CancellationToken cancellationToken)
        {
            if (message.IsEditable)
            {
                SymbolPropertyViewModel.Model = message.ViewModel;
                SymbolPropertyViewModel.Refresh();
                IsOnEditable = true;
            }
            else
            {
                SymbolPropertyViewModel.Model = new TextSymbolViewModel();
                SymbolPropertyViewModel.Refresh();
                IsOnEditable = false;
            }
            _log.Info($"{SymbolPropertyViewModel.Model.Id}가 PropertyViewModel을 {IsOnEditable} 하였습니다.", _class);


            return Task.CompletedTask;
        }
        #endregion
        #region - Properties -
        public bool IsOnEditable
        {
            get { return SymbolPropertyViewModel.IsEnable; }
            set
            {
                SymbolPropertyViewModel.IsEnable = value;
                NotifyOfPropertyChange(() => IsOnEditable);
            }
        }

        public SymbolPropertyViewModel SymbolPropertyViewModel { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
