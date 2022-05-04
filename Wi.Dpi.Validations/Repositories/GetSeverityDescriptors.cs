using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public class GetSeverityDescriptors : IGetSeverityDescriptors
    {
        private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
        private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;

        public GetSeverityDescriptors(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider)
        {
            _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
            _sqlAccessTokenProvider = sqlAccessTokenProvider;
        }
        private readonly string selectFromView = @"Select Id, CodeValue, [Description], [Namespace], RunStatusDescriptorId, ShortDescription
                                                 from [dbo].[SeverityDescriptorsForValidationsApi] ";
        private void MapResultFromReader(SeverityDescriptor result, SqlDataReader reader)
        {
            result.Id = reader.GetString("Id");
            result.CodeValue = reader.GetString("CodeValue");
            result.Description = reader.GetString("Description");
            result.Namespace = reader.GetString("Namespace");
            result.SeverityDescriptorId = reader.GetInt32("RunStatusDescriptorId");
            result.ShortDescription = reader.GetString("ShortDescription");

        }


        public async Task<IList<SeverityDescriptor>> GetAllAsync()
        {

            var descriptors = new List<SeverityDescriptor>();

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
                            var descriptor = new SeverityDescriptor();
                            MapResultFromReader(descriptor, reader);
                            descriptors.Add(descriptor);
                        }
                    }
                }
            }

            return descriptors;

        }


        public async Task<SeverityDescriptor> GetByIdAsync(string id)
        {
            var descriptor = new SeverityDescriptor();

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
