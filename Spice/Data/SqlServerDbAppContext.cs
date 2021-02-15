using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class SqlServerDbAppContext : ApplicationDbContext
    {
        public SqlServerDbAppContext()
        {   
        }

        public SqlServerDbAppContext(DbContextOptions<SqlServerDbAppContext> options)
            : base(options)
        {
        }

        //Needed for EntityFramework for migrations reasons
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    optionsBuilder.UseSqlServer(
        //        (new SQLServerHerokuConnectionStringFactory()).Build()
        //    );
        //    base.OnConfiguring(optionsBuilder);
        //}
        // EntityFrameworkCore\Add-Migration InitialMigration -StartupProject Spice -Context SqlServerDbAppContext -OutputDir Data/Migrations/SqlServer -Verbose

    }

    //For EntityFramework Migrations
    //public class SqlServerDbAppFactory : IDesignTimeDbContextFactory<SqlServerDbAppContext>
    //{
    //    public SqlServerDbAppContext CreateDbContext(string[] args)
    //    {

    //        var builder = new DbContextOptionsBuilder<SqlServerDbAppContext>();
    //        builder.UseSqlServer(
    //            (new SQLServerHerokuConnectionStringFactory()).Build()
    //        );

    //        return new SqlServerDbAppContext(builder.Options);
    //    }

    //}

}
