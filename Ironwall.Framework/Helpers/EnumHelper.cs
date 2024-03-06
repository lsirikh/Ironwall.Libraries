using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Helpers
{
    public static class EnumHelper
    {

        public static int GetEventType(int type)
        {

            int returnValue = 0;

            switch (type)
            {
                case (int)EnumCmdType.LOGIN_REQUEST:
                case (int)EnumCmdType.LOGIN_REQUEST_FORCE:
                case (int)EnumCmdType.LOGIN_RESPONSE:
                    {
                    }
                    break;
                case (int)EnumCmdType.LOGOUT_REQUEST:
                case (int)EnumCmdType.LOGOUT_REQUEST_FORCE_LOGIN:
                case (int)EnumCmdType.LOGOUT_REQUEST_TIMEOUT:
                case (int)EnumCmdType.LOGOUT_RESPONSE:
                    {

                    }
                    break;
                case (int)EnumCmdType.SESSION_REFRESH_REQUEST:
                case (int)EnumCmdType.SESSION_REFRESH_RESPONSE:
                    {

                    }
                    break;
                case (int)EnumCmdType.USER_ACCOUNT_ADD_REQUEST:
                case (int)EnumCmdType.USER_ACCOUNT_ADD_RESPONSE:
                    {

                    }
                    break;

                case (int)EnumCmdType.USER_ACCOUNT_EDIT_REQUEST:
                case (int)EnumCmdType.USER_ACCOUNT_EDIT_RESPONSE:
                    {

                    }
                    break;
                case (int)EnumCmdType.USER_ACCOUNT_DELETE_REQUEST:
                case (int)EnumCmdType.USER_ACCOUNT_DELETE_RESPONSE:
                    {

                    }
                    break;
                case (int)EnumCmdType.USER_ACCOUNT_INFO_REQUEST:
                case (int)EnumCmdType.USER_ACCOUNT_INFO_RESPONSE:
                    {
                    }
                    break;
                
                case (int)EnumCmdType.EVENT_DETECTION_REQUEST:
                case (int)EnumCmdType.EVENT_DETECTION_RESPONSE:
                    {
                        returnValue = (int)EnumEventType.Intrusion;
                    }
                    break;
                
                case (int)EnumCmdType.EVENT_MALFUNCTION_REQUEST:
                case (int)EnumCmdType.EVENT_MALFUNCTION_RESPONSE:
                    {
                        returnValue = (int)EnumEventType.Fault;
                    }
                    break;
                case (int)EnumCmdType.EVENT_ACTION_REQUEST:
                case (int)EnumCmdType.EVENT_ACTION_RESPONSE:
                    {
                        returnValue = (int)EnumEventType.Action;
                    }
                    break;
                case (int)EnumCmdType.EVENT_CONNECTION_REQUEST:
                case (int)EnumCmdType.EVENT_CONNECTION_RESPONSE:
                    {
                        returnValue = (int)EnumEventType.Connection;
                    }
                    break;
                
                default:
                    break;
            }

            return returnValue;
        }
    }
}
