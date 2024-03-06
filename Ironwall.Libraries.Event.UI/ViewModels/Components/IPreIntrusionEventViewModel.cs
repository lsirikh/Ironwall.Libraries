using Ironwall.Framework.Models.Events;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public interface IPreIntrusionEventViewModel : IExEventTimerViewModel<IDetectionEventModel>
    {
        int Result { get; set; }
        string Type { get; set; }

        void Accept(EventVisitor visitor, ActionEventModel actionModel);
        Task SendAction(string content = null, string idUser = null);
    }
}