namespace Ironwall.Libraries.Base.Services
{
    public interface ILogService
    {
        public void Error(string msg, bool debug = true);
        public void Info(string msg, bool debug = true);
        public void Warning(string msg, bool debug = true);
    }
}