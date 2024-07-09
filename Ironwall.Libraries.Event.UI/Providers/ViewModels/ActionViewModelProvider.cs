
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Providers;

namespace Ironwall.Libraries.Event.UI.Providers.ViewModels
{
    public sealed class ActionViewModelProvider : WrapperActionViewModelProvider<IActionEventModel, ActionEventViewModel>
    {
        #region - Ctors -
        public ActionViewModelProvider(ActionEventProvider provider) : base(provider)
        {
            ClassName = nameof(ActionViewModelProvider);
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
        #endregion
        #region - Attributes -
        #endregion
    }
}
