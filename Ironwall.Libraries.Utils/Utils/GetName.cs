using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Utils
{
    public static class GetName
    {
        public static string Find(object sender = default)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;

            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Name;

            return prevFuncName + " - " + prevClassName; ;

        }
    }
}
