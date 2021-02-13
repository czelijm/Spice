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

        public SQLServerHerokuConnectionStringFactory(string dbItegratedSecurity="False",string dbPersistSecurityInfo="False")
        {
            _dbItegratedSecurity = dbItegratedSecurity;
            _dbPersistSecurityInfo = dbPersistSecurityInfo;
        }

        public string Build(string url)
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
