﻿using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Accounts
{
    public class LogoutResponseModel : ResponseModel, ILogoutResponseModel
    {
        public LogoutResponseModel()
        {
            Command = (int)EnumCmdType.LOGOUT_RESPONSE;
        }

        public LogoutResponseModel(bool success, string msg)
            : base(success, msg)
        {
            Command = (int)EnumCmdType.LOGOUT_RESPONSE;
        }

        //public void Insert(bool success, string msg)
        //{
        //    Success = success;
        //    Message = msg;
        //}

        //public void Insert(int cmd, bool success, string msg)
        //{
        //    Command = cmd;
        //    Success = success;
        //    Message = msg;
        //}
    }
}
