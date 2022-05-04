using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{

    public class GetRunStatusDescriptors : IGetRunStatusDescriptors
    {
         private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
         private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;

        public GetRunStatusDescriptors(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider)
         {
             _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
             _sqlAccessTokenProvider = sqlAccessTokenProvider;
         }
     
        private readonly string selectFromView = @"Select Id, CodeValue, [Description], [Namespace], RunStatusDescriptorId, ShortDescription
                                                 from [dbo].[RunStatusDescriptorsForValidationsApi] ";

        private void MapResultFromReader(RunStatusDescriptor result, SqlDataReader reader)
        {
            result.Id = reader.GetString("Id");
            result.CodeValue = reader.GetString("CodeValue");
            result.Description = reader.GetString("Description");
            result.Namespace = reader.GetString("Namespace");
            result.RunStatusDescriptorId = reader.GetInt32("RunStatusDescriptorId");
            result.ShortDescription = reader.GetString("ShortDescription");
            
        }


        public async Task<IList<RunStatusDescriptor>> GetAllAsync()
        {
            var descriptors = new List<RunStatusDescriptor>();

            var sql =
                $@" {selectFromView}
                    ORDER BY RunStatusDescriptorId;";

            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken();

                await conn.OpenAsync();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            var descriptor = new RunStatusDescriptor();
                            MapResultFromReader(descriptor, reader);
                            descriptors.Add(descriptor);
                        }
                    }
                }
            }

            return descriptors;
        }

        public async Task<RunStatusDescriptor> GetByIdAsync(string id)
        {
            var descriptor = new RunStatusDescriptor();

            var sql = $@" {selectFromView}
                            where Id = @id";

            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken(); ;

                await conn.OpenAsync();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        if (await reader.ReadAsync())
                        {

                            MapResultFromReader(descriptor, reader);

                        }
                    }
                }
            }
            return descriptor;
        }

       
    }
}
