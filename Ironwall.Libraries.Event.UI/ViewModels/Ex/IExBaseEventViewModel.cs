using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IExBaseEventViewModel<T> where T : IBaseEventModel
    {
        int Id { get; set; }
        DateTime DateTime { get; set; }

        void Dispose();
        void OnLoaded(object sender, SizeChangedEventArgs e);
        void UpdateModel(T model);
    }
}
