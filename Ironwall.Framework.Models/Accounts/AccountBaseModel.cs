using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Accounts
{
    public abstract class AccountBaseModel 
        : IAccountBaseModel
    {
        public AccountBaseModel()
        {

        }

        public AccountBaseModel(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
