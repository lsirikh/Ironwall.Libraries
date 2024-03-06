using System.Threading.Tasks;
using System.Threading;
using System;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    public interface IObjectShapeViewModel : IShapeSymbolViewModel
    {
        Task AlarmTask(DateTime expTime = default, CancellationTokenSource cancellationTokenSource = default);
        Task MalfunctionTask(DateTime expTime = default, CancellationTokenSource cancellationTokenSource = default);
        Task ExecuteCancel();

        int IdController { get; set; }
        int IdSensor { get; set; }
        string NameArea { get; set; }
        string NameDevice { get; set; }
        int TypeDevice { get; set; }
        bool IsAlarming { get; set; }
    }
}