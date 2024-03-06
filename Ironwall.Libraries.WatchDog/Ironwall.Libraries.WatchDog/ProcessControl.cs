using System;
using System.Diagnostics;
using System.Linq;

namespace Ironwall.Libraries.WatchDog
{
    public class ProcessControl : IProcessControl
    {
        private string _target;
        private string _fileName;
        private string _currentDir;

        public void SetTarget(string name)
        {
            _target = name;
            _fileName = $"{name}.exe";
            Setup();
        }
        
        private static readonly Lazy<ProcessControl> _instance = new Lazy<ProcessControl>(() => new ProcessControl());

        public static ProcessControl Instance { get { return _instance.Value; } }

        public ProcessControl()
        {
        }

        private void Setup()
        {
            pInfo = new ProcessStartInfo();
            _currentDir = System.IO.Directory.GetCurrentDirectory();
            pInfo.UseShellExecute = true;
            pInfo.FileName = _currentDir + '\\'+ _fileName;
            pInfo.WorkingDirectory = _currentDir;
            pInfo.Verb = "runas";
        }

        public bool IsProcessRun()
        {
            Process[] pses = Process.GetProcesses();
            Process[] ps = Process.GetProcessesByName(_target);
            if (ps.Count() > 0)
                return true;
            else
                return false;
        }

        public bool Terminate()
        {
            Process[] ps = Process.GetProcessesByName(_target);
            try
            {
                if (IsProcessRun())
                    ps[0].Kill();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool StartProcess()
        {
            try
            {
                if (!IsProcessRun())
                    Process.Start(pInfo);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        ProcessStartInfo pInfo;
    }
}
