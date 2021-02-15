using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class PostgresDbAppContext : ApplicationDbContext
    {
        public PostgresDbAppContext()
        {
        }
        public PostgresDbAppContext(DbContextOptions<PostgresDbAppContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql(
        //        (new PostgresHerokuConnectionStringFactory("False", "False", "Require", "True")).Build()
        //    );
        //    base.OnConfiguring(optionsBuilder);
        //}

        // EntityFrameworkCore\Add-Migration InitialMigration -StartupProject Spice -Context PostgresDbAppContext -OutputDir Migrations/Postgres

    }
    //public class PostgresDbAppFactory : IDesignTimeDbContextFactory<PostgresDbAppContext>
    //{
    //    public PostgresDbAppContext CreateDbContext(string[] args)
    //    {
    //        var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
    //        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    //        var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
    //        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
    //        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
    //        var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
    //        var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

    //        var builder = new DbContextOptionsBuilder<PostgresDbAppContext>();
    //        builder.UseNpgsql(
    //            $"Host={dbServerName};Port={dbPort};Database={dataBase};" +
    //            $"User ID={dbUser};Password={dbPassword}"
    //        );

    //        return new PostgresDbAppContext(builder.Options);
    //    }

    //}




}
