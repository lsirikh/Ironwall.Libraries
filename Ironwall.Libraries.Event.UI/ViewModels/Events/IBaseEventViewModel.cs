using Ironwall.Framework.Models.Events;
using System;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IBaseEventViewModel<T> where T : IBaseEventModel
    {
        string Id { get; set; }
        DateTime DateTime { get; set; }

        void Dispose();
        void OnLoaded(object sender, SizeChangedEventArgs e);
        void UpdateModel(T model);
    }
}