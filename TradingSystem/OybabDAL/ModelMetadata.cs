using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Oybab.DAL
{
    [DbConfigurationType(typeof(SQLiteConfiguration))]
    public partial class tsEntities : DbContext
    {
        
        public tsEntities(string conn)
            : base(conn)
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public tsEntities(string conn, bool LazyLoadingEnabled = true)
            : base(conn)
        {
            this.Configuration.LazyLoadingEnabled = LazyLoadingEnabled;
        }
    }

    internal sealed class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", System.Data.SQLite.SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite.EF6", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));

        }
    }  
}
