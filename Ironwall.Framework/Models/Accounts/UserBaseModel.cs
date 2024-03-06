using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Accounts
{
    public abstract class UserBaseModel 
        : AccountBaseModel
        , IUserBaseModel
    {
        public UserBaseModel()
        {

        }

        public UserBaseModel(IUserBaseModel model)
            : base(model.Id)
        {
            IdUser = model.IdUser;
            Password = model.Password;
            Level = model.Level;
            Name = model.Name;
            Used = model.Used;
        }

        public UserBaseModel(int id, string idUser, string pass, int level, string name, bool used)
            : base(id) 
        {
            IdUser = idUser;
            Password = pass;
            Level = level;
            Name = name;
            Used = used;
        }

        public string IdUser { get; set; }
        public string Password { get; set; }
        public int Level { get; set; } = 2;
        public string Name { get; set; }
        public bool Used { get; set; }
    }
}
