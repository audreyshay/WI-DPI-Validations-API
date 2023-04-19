using EdFi.Ods.Common.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Domain;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Rule
{
    public interface IGetCollectionValidationRules : IGetValidationRules
    {

    }

    public class GetCollectionValidationRules : IGetCollectionValidationRules
    {
        private readonly ICollectionsOdsConnectionStringProvider _odsDatabaseConnectionStringProvider;
        private readonly ISqlAccessTokenProvider _sqlAccessTokenProvider;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;
        
        private readonly string selectFromView = @"select RuleIdentifier, RuleSource, RuleId, HelpUrl, ShortDescription, Description, RuleStatusDescriptor, 
                                                 Category, SeverityDescriptor,
                                                 ExternalRuleId, ValidationLogicTypeDescriptor, ValidationLogic
                                                 from [dbo].[RulesForValidationsApi] ";

        public GetCollectionValidationRules(ICollectionsOdsConnectionStringProvider odsDatabaseConnectionStringProvider, ISqlAccessTokenProvider sqlAccessTokenProvider,
            Func<ISchoolYearContextProvider> schoolYearContextProviderFactory)
        {
            _odsDatabaseConnectionStringProvider = odsDatabaseConnectionStringProvider;
            _sqlAccessTokenProvider = sqlAccessTokenProvider;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
        }

        private string YearClause() {
            var schoolYear = (short)_schoolYearContextProviderFactory().GetSchoolYear();
            return $" where ((StartSchoolYear is null or StartSchoolYear <= {schoolYear}) and (EndSchoolYear is null or EndSchoolYear >= {schoolYear}))";
        }
        private void MapRuleFromReader(ValidationRule rule, SqlDataReader reader)
        {
            rule.Id = reader.GetString("RuleId");
            rule.RuleIdentifier = reader.GetString("RuleIdentifier");
            rule.RuleSource = reader.GetString("RuleSource");
            rule.HelpUrl = reader.IsDBNull("HelpUrl") ? null : reader.GetString("HelpUrl");
            rule.ShortDescription = reader.IsDBNull("ShortDescription") ? null : reader.GetString("ShortDescription");
            rule.Description = reader.IsDBNull("Description") ? null : reader.GetString("Description");
            rule.RuleStatusDescriptor = reader.GetString("RuleStatusDescriptor");
            rule.Category = reader.IsDBNull("Category") ? null : reader.GetString("Category");
            rule.SeverityDescriptor = reader.GetString("SeverityDescriptor");
            rule.ExternalRuleId = reader.IsDBNull("ExternalRuleId") ? null : reader.GetString("ExternalRuleId");
            rule.ValidationLogicTypeDescriptor = reader.IsDBNull("ValidationLogicTypeDescriptor") ? null : reader.GetString("ValidationLogicTypeDescriptor");
            rule.ValidationLogic = reader.IsDBNull("ValidationLogic") ? null : reader.GetString("ValidationLogic");

        }
        public async Task<ValidationRule> GetByIdAsync(string id)
        {
            var rule = new ValidationRule();
            var sql = $@"{selectFromView} {YearClause()}
                    and RuleId = @id";

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
                            MapRuleFromReader(rule, reader);
                        }
                    }
                }
            }
            return rule;

        }
        public async Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest)
        {
            List<ValidationRule> rules = new List<ValidationRule>();
            List<string> clauses = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(rulesRequest.Id))
            {
                clauses.Add("RuleId = @id");
                var id = new SqlParameter("@id", SqlDbType.VarChar);
                id.Value = rulesRequest.Id;
                parameters.Add(id);
            }
            if (!string.IsNullOrWhiteSpace(rulesRequest.RuleIdentifier))
            {
                clauses.Add("RuleIdentifier = @ruleIdentifier");
                var ruleIdentifier = new SqlParameter("@ruleIdentifier", SqlDbType.VarChar);
                ruleIdentifier.Value = rulesRequest.RuleIdentifier;
                parameters.Add(ruleIdentifier);
            }
            if (!string.IsNullOrWhiteSpace(rulesRequest.RuleSource))
            {
                clauses.Add("RuleSource = @ruleSource");
                var ruleSource = new SqlParameter("@ruleSource", SqlDbType.VarChar);
                ruleSource.Value = rulesRequest.RuleSource;
                parameters.Add(ruleSource);
            }

            if (!string.IsNullOrWhiteSpace(rulesRequest.RuleStatus))
            {
                clauses.Add("RuleStatus = @status");
                var status = new SqlParameter("@status", SqlDbType.VarChar);
                status.Value = rulesRequest.RuleStatus;
                parameters.Add(status);
            }

            if (!string.IsNullOrWhiteSpace(rulesRequest.SeverityDescriptor))
            {
                clauses.Add("SeverityDescriptor = @severity");
                var severity = new SqlParameter("@severity", SqlDbType.VarChar);
                severity.Value = rulesRequest.SeverityDescriptor;
                parameters.Add(severity);

            }

            if (!string.IsNullOrWhiteSpace(rulesRequest.Category))
            {
                clauses.Add("Category like @category");
                var category = new SqlParameter("@category", SqlDbType.VarChar);
                category.Value = "%" + rulesRequest.Category + "%";
                parameters.Add(category);

            }

            if (!string.IsNullOrWhiteSpace(rulesRequest.ExternalRuleId))
            {
                clauses.Add("ExternalRuleId = @type");
                var type = new SqlParameter("@type", SqlDbType.VarChar);
                type.Value = rulesRequest.ExternalRuleId;
                parameters.Add(type);
            }

            var sql =
                $@"{selectFromView}  {YearClause()} {(clauses.Any() ? ("and " + string.Join(" and ", clauses)) : "")}
                    ORDER BY RuleIdentifier
                    OFFSET {rulesRequest.Offset ?? 0} ROWS
                    FETCH NEXT {rulesRequest.Limit} ROWS ONLY;";


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
                            var rule = new ValidationRule();
                            MapRuleFromReader(rule, reader);
                            rules.Add(rule);
                        }
                    }
                }
            }

            return rules;

        }
    }
}
