using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.RuleRun
{
    public interface IGetCollectionValidationRuleRuns : IGetValidationRuns
    {

    }

    public class GetCollectionValidationRuleRuns : IGetCollectionValidationRuleRuns
    {
        private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
        private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;
        private readonly string selectFromView = @"select Id, RunIdentifier, RunStartDateTime, RunFinishDateTime, RunStatusDescriptor, 
                                                          Host, ValidationEngine, EducationOrganizationId, Discriminator, EdOrgResourceId, [Namespace]
                                                          from [dbo].[RuleRunsForValidationsApi]";
        public GetCollectionValidationRuleRuns(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider,
            Func<IApiKeyContextProvider> apiKeyContextProviderFactory,
            Func<ISchoolYearContextProvider> schoolYearContextProviderFactory)
        {
            _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
            _sqlAccessTokenProvider = sqlAccessTokenProvider;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
        }

        private string AuthorizationClauses()
        {
            var schoolYear = (short)_schoolYearContextProviderFactory().GetSchoolYear();
            var keyContext = _apiKeyContextProviderFactory().GetApiKeyContext();
            var edOrgs = keyContext.EducationOrganizationIds.ToList();

            var yearString = $@"SchoolYear = {schoolYear} and ";

            var orgString = edOrgs.Any()
                ? $"EducationOrganizationId in ({string.Join(", ", edOrgs)}) "
                : "EducationOrganizationId is null ";

            return yearString + orgString;
        }

        private void MapRunFromReader(ValidationRuleRun run, SqlDataReader reader)
        {
            run.Id = reader.GetString("Id");
            run.RunIdentifier = reader.GetString("RunIdentifier");
            run.RunStartDateTime = reader.GetDateTime("RunStartDateTime");
            run.RunFinishDateTime = reader.IsDBNull("RunFinishDateTime") ? (DateTime?)null : reader.GetDateTime("RunFinishDateTime");
            run.RunStatusDescriptor = reader.GetString("RunStatusDescriptor");
            run.Host = reader.IsDBNull("Host") ? null : reader.GetString("Host");
            run.ValidationEngine = reader.IsDBNull("Namespace") ? null : reader.GetString("ValidationEngine");
            run.EducationOrganizationReference = reader.IsDBNull("EducationOrganizationId") ? null :
             new EducationOrganizationReference
             {
                 EducationOrganizationId = reader.GetInt32("EducationOrganizationId"),
                 Discriminator = reader.GetString("Discriminator"),
                 ResourceId = reader.GetGuid("EdOrgResourceId")
             };

        }

        public async Task<ValidationRuleRun> GetByIdAsync(string id)
        {
            var run = new ValidationRuleRun();

            var sql = $@"{selectFromView}
                    where {AuthorizationClauses()} 
                    and Id = @id";

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
                            MapRunFromReader(run, reader);

                        }
                    }
                }
            }
            return run;

        }
        public async Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest runsRequest)
        {
            List<ValidationRuleRun> runs = new List<ValidationRuleRun>();
            List<string> clauses = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (runsRequest.RunIdentifier != null)
            {
                clauses.Add("RunIdentifier = @runIdentifier");
                var runIdentifier = new SqlParameter("@runIdentifier", SqlDbType.VarChar);
                runIdentifier.Value = runsRequest.RunIdentifier;
                parameters.Add(runIdentifier);
            }
            if (runsRequest.Id != null)
            {
                clauses.Add("Id = @id");
                var id = new SqlParameter("@id", SqlDbType.VarChar);
                id.Value = runsRequest.Id;
                parameters.Add(id);
            }

            if (!string.IsNullOrWhiteSpace(runsRequest.Host))
            {
                clauses.Add("Host = @host");
                var host = new SqlParameter("@host", SqlDbType.VarChar);
                host.Value = runsRequest.Host;
                parameters.Add(host);
            }

            if (!string.IsNullOrWhiteSpace(runsRequest.ValidationEngine))
            {
                clauses.Add("ValidationEngine = @validationEngine");
                var validationEngine = new SqlParameter("@validationEngine", SqlDbType.VarChar);
                validationEngine.Value = runsRequest.ValidationEngine;
                parameters.Add(validationEngine);
            }

            var sql =
                $@" {selectFromView}
                    where {AuthorizationClauses()}
                    {(clauses.Any() ? " and " + string.Join(" and ", clauses) : "")}
                    ORDER BY RunIdentifier
                    OFFSET {runsRequest.Offset ?? 0} ROWS
                    FETCH NEXT {runsRequest.Limit} ROWS ONLY;";


            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken();

                await conn.OpenAsync();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            var run = new ValidationRuleRun();
                            MapRunFromReader(run, reader);
                            runs.Add(run);
                        }
                    }
                }
            }

            return runs;

        }



    }
}
