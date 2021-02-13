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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
            var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
            var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

            optionsBuilder.UseSqlServer(
                $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
                $"User ID={dbUser};Password={dbPassword}"
            );
            base.OnConfiguring(optionsBuilder);
        }
        // EntityFrameworkCore\Add-Migration InitialMigration -StartupProject Spice -Context SqlServerDbAppContext -OutputDir Data/Migrations/SqlServer -Verbose

    }

    //For EntityFramework Migrations
    public class SqlServerDbAppFactory : IDesignTimeDbContextFactory<SqlServerDbAppContext>
    {
        public SqlServerDbAppContext CreateDbContext(string[] args)
        {
            var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
            var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
            var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

            var builder = new DbContextOptionsBuilder<SqlServerDbAppContext>();
            builder.UseSqlServer(
                $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
                $"User ID={dbUser};Password={dbPassword}"
            );

            return new SqlServerDbAppContext(builder.Options);
        }

    }

}
