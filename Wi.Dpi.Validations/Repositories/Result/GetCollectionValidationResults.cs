using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Api.Common.Models.Resources.Staff.EdFi;
using EdFi.Ods.Api.Common.Models.Resources.Student.EdFi;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using EdFi.Security.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Result
{
    public interface IGetCollectionValidationResults : IGetValidationResults
    {

    }

    public class GetCollectionValidationResults : IGetCollectionValidationResults
    {
        private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
        private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;
        private readonly ISecurityRepository _securityRepository;

        private readonly string selectFromView = @"select ResultIdentifier, Id,  RunIdentifier, RunId, RuleIdentifier, RuleSource, RuleId, ResourceId, ResourceType, 
                                                 EducationOrganizationId, Discriminator, EdOrgResourceId, StudentUniqueId, StudentResourceId,
                                                 StaffUniqueId, StaffResourceId, Namespace, Acknowledged
                                                 from [dbo].[MessagesForValidationsApi] ";

        public GetCollectionValidationResults(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider,
            Func<IApiKeyContextProvider> apiKeyContextProviderFactory,
            Func<ISchoolYearContextProvider> schoolYearContextProviderFactory,
            ISecurityRepository securityRepository)
        {
            _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
            _sqlAccessTokenProvider = sqlAccessTokenProvider;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
            _securityRepository = securityRepository;
        }
        private string AuthorizationClauses()
        {
            var schoolYear = (short)_schoolYearContextProviderFactory().GetSchoolYear();
            var keyContext = _apiKeyContextProviderFactory().GetApiKeyContext();
            var edOrgs = keyContext.EducationOrganizationIds.ToList();
            var resourceClaims = _securityRepository.GetClaimsForClaimSet(keyContext.ClaimSetName);
            var resourceTypes = resourceClaims.Select(c => c.ResourceClaim.ResourceClaimId).Distinct().ToList();

            var authString = $@"SchoolYear = {schoolYear}
                and ResourceClaimId in ({string.Join(", ", resourceTypes)})";

            var orgString = edOrgs.Any()
                ? $" and (LocalEducationAgencyId in ({string.Join(", ", edOrgs)}) or SchoolId in ({string.Join(", ", edOrgs)})) "
                : "";

            //TODO Namespace auth?
            return authString + orgString;
        }

        private void MapResultFromReader(ValidationResult result, SqlDataReader reader)
        {
            result.Id = reader.GetString("Id");
            result.ResultIdentifier = reader.GetString("ResultIdentifier");

            result.ValidationRuleRunReference = new ValidationRuleRunReference()
            {
                RunIdentifier = reader.GetString("RunIdentifier"),
                Id = reader.GetString("RunId")
            };

            result.ValidationRuleReference = new ValidationRuleReference()
            {
                RuleIdentifier = reader.GetString("RuleIdentifier"),
                RuleSource = reader.GetString("RuleSource"),
                Id = reader.GetString("RuleId")
            };
            result.ResourceId = reader.IsDBNull("ResourceId") ? null : reader.GetGuid("ResourceId").ToString().Replace("-","").ToLower();
            result.ResourceType = reader.IsDBNull("ResourceType") ? null : reader.GetString("ResourceType");
            result.EducationOrganizationReference = reader.IsDBNull("EducationOrganizationId") ? null : 
                new EducationOrganizationReference
                {
                    EducationOrganizationId = reader.GetInt32("EducationOrganizationId"), 
                    Discriminator = reader.GetString("Discriminator"),
                    ResourceId = reader.GetGuid("EdOrgResourceId")
                };
            result.StudentReference = reader.IsDBNull("StudentUniqueId") ? null : 
                new StudentReference { StudentUniqueId = reader.GetString("StudentUniqueId"),
                    ResourceId = reader.GetGuid("StudentResourceId")
                };
            result.StaffReference = reader.IsDBNull("StaffUniqueId") ? null :
                new StaffReference
                {
                    StaffUniqueId = reader.GetString("StaffUniqueId"),
                    ResourceId = reader.GetGuid("StaffResourceId")
                };

            result.Namespace = reader.IsDBNull("Namespace") ? null : reader.GetString("Namespace");

            if (!reader.IsDBNull("Acknowledged"))
            {
                result.AdditionalContext = new AdditionalContext[] { new AdditionalContext()
                {
                    IdentificationCode = "Acknowledged",
                    CustomizationValue = reader.GetBoolean("Acknowledged").ToString()
                } };
            }
        }

        public async Task<IList<ValidationResult>> GetAllAsync(ValidationResultRequest resultRequest)
        {
            List<ValidationResult> rules = new List<ValidationResult>();
            List<string> clauses = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(resultRequest.Id))
            {
                clauses.Add("Id = @id");
                var id = new SqlParameter("@id", SqlDbType.VarChar);
                id.Value = resultRequest.Id;
                parameters.Add(id);
            }

            if (!string.IsNullOrWhiteSpace(resultRequest.ResultIdentifier))
            {
                clauses.Add("ResultIdentifier = @resultIdentifier");
                var resultIdentifier = new SqlParameter("@resultIdentifier", SqlDbType.VarChar);
                resultIdentifier.Value = resultRequest.ResultIdentifier;
                parameters.Add(resultIdentifier);
            }

            if (!string.IsNullOrWhiteSpace(resultRequest.RuleIdentifier))
            {
                clauses.Add("RuleIdentifier = @ruleid");
                var ruleid = new SqlParameter("@ruleid", SqlDbType.VarChar);
                ruleid.Value = resultRequest.RuleIdentifier;
                parameters.Add(ruleid);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.RuleSource))
            {
                clauses.Add("RuleSource = @ruleSource");
                var ruleSource = new SqlParameter("@ruleSource", SqlDbType.VarChar);
                ruleSource.Value = resultRequest.RuleSource;
                parameters.Add(ruleSource);
            }
            if (resultRequest.RunIdentifier != null)
            {
                clauses.Add("RunIdentifier = @runid");
                var runid = new SqlParameter("@runid", SqlDbType.VarChar);
                runid.Value = resultRequest.RunIdentifier;
                parameters.Add(runid);
            }
            if (resultRequest.EducationOrganizationId != null)
            {
                clauses.Add("EducationOrganizationId = @educationOrganizationId");
                var educationOrganizationId = new SqlParameter("@educationOrganizationId", SqlDbType.Int);
                educationOrganizationId.Value = resultRequest.EducationOrganizationId;
                parameters.Add(educationOrganizationId);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.Namespace))
            {
                clauses.Add("[Namespace] = @namespace");
                var namesp = new SqlParameter("@namespace", SqlDbType.VarChar);
                namesp.Value = resultRequest.Namespace;
                parameters.Add(namesp);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.ResourceType))
            {
                clauses.Add("ResourceType = @type");
                var type = new SqlParameter("@type", SqlDbType.VarChar);
                type.Value = resultRequest.ResourceType;
                parameters.Add(type);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.ResourceId))
            {
                clauses.Add("ResourceId = @resId");
                var resId = new SqlParameter("@resId", SqlDbType.VarChar);
                resId.Value = resultRequest.ResourceId;
                parameters.Add(resId);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.StudentUniqueId))
            {
                clauses.Add("StudentUniqueId = @studentUniqueId");
                var studentUniqueId = new SqlParameter("@studentUniqueId", SqlDbType.VarChar);
                studentUniqueId.Value = resultRequest.StudentUniqueId;
                parameters.Add(studentUniqueId);
            }
            if (!string.IsNullOrWhiteSpace(resultRequest.StaffUniqueId))
            {
                clauses.Add("StaffUniqueId = @staffUniqueId");
                var staffUniqueId = new SqlParameter("@staffUniqueId", SqlDbType.VarChar);
                staffUniqueId.Value = resultRequest.StaffUniqueId;
                parameters.Add(staffUniqueId);
            }

            var sql =
                $@" {selectFromView}
                    where {AuthorizationClauses()}
                    {(clauses.Any() ? " and " + string.Join(" and ", clauses) : "")}
                    ORDER BY ResultIdentifier
                    OFFSET {resultRequest.Offset ?? 0} ROWS
                    FETCH NEXT {resultRequest.Limit} ROWS ONLY;";

            using (var conn = new SqlConnection(_odsDatabaseConnectionStringProvider.GetConnectionString()))
            {
                conn.AccessToken = _sqlAccessTokenProvider.GetAccessToken(_odsDatabaseConnectionStringProvider.GetConnectionString()); ;

                await conn.OpenAsync();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                    {
                        while (await reader.ReadAsync())
                        {
                            var rule = new ValidationResult();
                            MapResultFromReader(rule, reader);
                            rules.Add(rule);
                        }
                    }
                }
            }

            return rules;

        }


        public async Task<ValidationResult> GetByIdAsync(string id)
        {
            var rule = new ValidationResult();

            var sql = $@" {selectFromView}
                            where {AuthorizationClauses()} and
                            Id = @id";

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

                            MapResultFromReader(rule, reader);

                        }
                    }
                }
            }
            return rule;

        }


    }
}
