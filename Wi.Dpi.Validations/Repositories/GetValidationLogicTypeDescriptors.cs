using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public class GetValidationLogicTypeDescriptors : IGetValidationLogicTypeDescriptors
    {
        private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
        private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;

        public GetValidationLogicTypeDescriptors(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider)
        {
            _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
            _sqlAccessTokenProvider = sqlAccessTokenProvider;
        }

        private readonly string selectFromView = @"Select Id, CodeValue, [Description], [Namespace], RunStatusDescriptorId, ShortDescription
                                                 from [dbo].[ValidationLogicTypeDescriptorsForValidationsApi] ";
        private void MapResultFromReader(ValidationLogicTypeDescriptor result, SqlDataReader reader)
        {
            result.Id = reader.GetString("Id");
            result.CodeValue = reader.GetString("CodeValue");
            result.Description = reader.GetString("Description");
            result.Namespace = reader.GetString("Namespace");
            result.ValidationLogicTypeDescriptorId = reader.GetInt32("RunStatusDescriptorId");
            result.ShortDescription = reader.GetString("ShortDescription");

        }


        public async Task<IList<ValidationLogicTypeDescriptor>> GetAllAsync()
        {

            var descriptors = new List<ValidationLogicTypeDescriptor>();

            var sql =
                $@" {selectFromView}
                    ORDER BY RunStatusDescriptorId;";

            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken(_odsDatabaseConnectionStringProvider.GetConnectionString()); ;

                await conn.OpenAsync();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            var descriptor = new ValidationLogicTypeDescriptor();
                            MapResultFromReader(descriptor, reader);
                            descriptors.Add(descriptor);
                        }
                    }
                }
            }

            return descriptors;


        }


        public async Task<ValidationLogicTypeDescriptor> GetByIdAsync(string id)
        {
            var descriptor = new ValidationLogicTypeDescriptor();

            var sql = $@" {selectFromView}
                            where Id = @id";

            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken(_odsDatabaseConnectionStringProvider.GetConnectionString()); ;

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
