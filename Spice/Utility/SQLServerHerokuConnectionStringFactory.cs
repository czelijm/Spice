using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public class SQLServerHerokuConnectionStringFactory : IConnectionStringFactory
    {

        private string _dbItegratedSecurity;
        private string _dbPersistSecurityInfo;

        public SQLServerHerokuConnectionStringFactory(string dbItegratedSecurity = "False", string dbPersistSecurityInfo = "False")
        {
            _dbItegratedSecurity = dbItegratedSecurity;
            _dbPersistSecurityInfo = dbPersistSecurityInfo;
        }

        public string Build(string url)
        {
            //return BuildForLocalDockerDeveloment();//for development only
            return BuildForProductionDockerDeveloment(url);//for development only
        }

        public string Build() 
        {
            return Build(Environment.GetEnvironmentVariable("DATABASE_URL"));
        }

        public string BuildForLocalDockerDeveloment()
        {
            var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? SD.DBProvier.postgres;
            var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
            var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
            var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

            return $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
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

            return $"Server={hostName},{port};Database={dbName};Integrated Security={_dbItegratedSecurity};Persist Security Info={_dbPersistSecurityInfo};" +
                                $"User ID={userName};Password={password}";
        }
    }
}
