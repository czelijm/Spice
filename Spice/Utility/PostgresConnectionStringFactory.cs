using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public class PostgresHerokuConnectionStringFactory : IConnectionStringFactory
    {

        private string _dbItegratedSecurity;
        private string _dbPersistSecurityInfo;
        private string _sslMode;
        private string _trustServerCertificate;


        public PostgresHerokuConnectionStringFactory(string dbItegratedSecurity = "False", string dbPersistSecurityInfo = "False", string sslMode = "Disable", string trustServerCertificate = "False")
        {
            _dbItegratedSecurity = dbItegratedSecurity;
            _dbPersistSecurityInfo = dbPersistSecurityInfo;
            _sslMode = sslMode;
            _trustServerCertificate = trustServerCertificate;

        }

        public string Build(string url)
        {
            return BuildForLocalDockerDeveloment(); // only for development or EntityFramework Migration Create
           // return BuildForProductionDockerDeveloment(url); // production
        }
        public string Build()
        {
            return Build(Environment.GetEnvironmentVariable("DATABASE_URL"));
        }

        public string BuildForLocalDockerDeveloment()
        {
            var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
            var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
            var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

            return $"Host={dbServerName};Port={dbPort};Database={dataBase};" +
                $"User ID={dbUser};Password={dbPassword}";
        }
        public string BuildForProductionDockerDeveloment(string url)
        {
            var tmpSplit = url.Split(@"://");
            //var hostName = tmpSplit.First();
            url = tmpSplit.Last();

            tmpSplit = url.Split(@"/");

            url = tmpSplit.First();
            var dbName = tmpSplit.Last();

            tmpSplit = url.Split("@");
            var userName = tmpSplit.First();
            var hostName = tmpSplit.Last();
            tmpSplit = userName.Split(":");
            userName = tmpSplit.First();
            var password = tmpSplit.Last();
            tmpSplit = hostName.Split(":");
            hostName = tmpSplit.First();
            var port = tmpSplit.Last();

            return $"Host={hostName};Port={port};Database={dbName};User ID={userName};Password={password};" +
                //$"Integrated Security={_dbItegratedSecurity};Persist Security Info={_dbPersistSecurityInfo};" +
                $"SSL Mode={_sslMode};Trust Server Certificate={_trustServerCertificate};Pooling=True";
        }

    }
}
