using Ironwall.Framework.Models;
using System.Windows;

namespace Ironwall.Framework.ViewModels
{
    public interface IBaseCustomViewModel<T> :ISelectableBaseViewModel where T : IBaseModel
    {
        int Id { get; set; }
        void Dispose();
        void OnLoaded(object sender, SizeChangedEventArgs e);
        void UpdateModel(T model);
        T Model { get;}
    }
}