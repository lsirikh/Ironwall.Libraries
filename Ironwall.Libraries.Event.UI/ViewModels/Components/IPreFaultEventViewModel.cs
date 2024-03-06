using Ironwall.Framework.Models.Events;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public interface IPreFaultEventViewModel : IExEventTimerViewModel<IMalfunctionEventModel>
    {
        int CutFirstEnd { get; set; }
        int CutFirstStart { get; set; }
        int CutSecondEnd { get; set; }
        int CutSecondStart { get; set; }
        int Reason { get; set; }
        string Type { get; set; }

        void Accept(EventVisitor visitor, ActionEventModel actionModel);
        void OnClickButtonActionDetails(object sender, RoutedEventArgs e);
        Task SendAction(string content = null, string idUser = null);
    }
}