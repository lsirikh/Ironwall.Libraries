﻿using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Common.Models
{
    public class LogModel
    {
        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override string ToString()
        {
            if (Code != null)
                return $"[{CreatedTime}][{Level}][{Code}]{Message}";
            else
                return $"[{CreatedTime}][{Level}]{Message}";

        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void Info(string msg, string code = default)
        {
            Message = msg;
            Code = code;
            Level = EnumLogType.INFO;
        }

        public void Warning(string msg, string code = default)
        {
            Message = msg;
            Code = code;
            Level = EnumLogType.WARNING;
        }

        public void Error(string msg, string code = default)
        {
            Message = msg;
            Code = code;
            Level = EnumLogType.ERROR;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Code { get; set; } = "000";

        public string CreatedTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");

        public EnumLogType Level { get; set; }

        public string Message { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
    
}
