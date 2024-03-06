namespace Ironwall.Libraries.Tcp.Common.Events
{
    public class BaseResponseEvent
    {
        public BaseResponseEvent()
        {

        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}