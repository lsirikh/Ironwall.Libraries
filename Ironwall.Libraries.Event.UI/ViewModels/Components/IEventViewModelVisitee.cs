using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public interface IEventViewModelVisitee
    {
        void Accept(EventVisitor visitor, Framework.Models.Events.ActionEventModel actionModel);
    }
}
