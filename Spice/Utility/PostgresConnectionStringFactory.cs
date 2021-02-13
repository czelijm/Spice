using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public class PostgresHerokuConnectionStringFactory : IConnectionStringFactory
    {

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

            return $"Host={hostName};Port={port};Database={dbName};User ID={userName};Password={password}";
        }


    }
}
