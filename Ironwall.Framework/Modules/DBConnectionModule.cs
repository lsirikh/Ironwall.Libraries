using Autofac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Modules
{
    public class DBConnectionModule : Module
    {
        #region - Overrides -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.Register(ctx =>
                {
                    var dataSource = ((Func<string, string, int, string>)(
                    (pathDatabase, nameDatabase, version) =>
                    {
                        if (!Directory.CreateDirectory(pathDatabase).Exists)
                            throw new DirectoryNotFoundException();
                        return $@"Data Source={Path.Combine(pathDatabase, nameDatabase)}; Version={version};";

                    }))(Path.Combine(Environment.CurrentDirectory, PathDatabase), NameDatabase, Version);

                    return new SQLiteConnection(dataSource);
                })
                .As<SQLiteConnection>()
                .As<IDbConnection>()
                .SingleInstance();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region - Properties -
        public string PathDatabase { get; set; }
        public string NameDatabase { get; set; }
        public int Version { get; set; }
        #endregion

        #region - Attributes -
        #endregion
    }
}
