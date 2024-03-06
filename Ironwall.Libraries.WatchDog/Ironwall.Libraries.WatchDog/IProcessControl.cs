namespace Ironwall.Libraries.WatchDog
{
    public interface IProcessControl
    {
        bool IsProcessRun();
        bool Terminate();
        bool StartProcess();
    }
}