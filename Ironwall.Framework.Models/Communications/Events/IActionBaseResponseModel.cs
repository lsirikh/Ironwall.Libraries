namespace Ironwall.Framework.Models.Communications.Events
{
    interface IActionBaseResponseModel<T>
    {
        T Event { get; set; }
    }
}