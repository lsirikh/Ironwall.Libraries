using Ironwall.Framework.Models.Accounts;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Symbols
{
    public class SymbolDataLoadRequestModel
        : UserSessionBaseRequestModel, ISymbolDataLoadRequestModel
    {
        public SymbolDataLoadRequestModel()
        {
            Command = (int)EnumCmdType.SYMBOL_DATA_LOAD_REQUEST;
        }

        public SymbolDataLoadRequestModel(ILoginSessionModel model)
            : base(model)
        {
            Command = (int)EnumCmdType.SYMBOL_DATA_LOAD_REQUEST;
        }
    }
}
