using System;
using System.Collections.Generic;
using System.Text;
using EdFi.Common.Database;
using Microsoft.Extensions.Configuration;

namespace Wi.Dpi.Domain
{
    public interface ICollectionsOdsConnectionStringProvider: IDatabaseConnectionStringProvider
    {
       
    }

    public class CollectionsOdsConnectionStringProvider: ICollectionsOdsConnectionStringProvider
    {
        private readonly string _odsConnectionString;
        public CollectionsOdsConnectionStringProvider(IConfiguration configuration)
        {
            _odsConnectionString = configuration.GetConnectionString("Collections_Ods");
        }

        public string GetConnectionString()
        {
            return _odsConnectionString;
        }
    }
}
