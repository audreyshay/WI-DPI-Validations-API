using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using Wi.Dpi.Validations.Models;
using WISEmeta.Api.Client;

namespace Wi.Dpi.Validations.Repositories.RuleRun
{
    public interface IGetFinanceValidationRuleRuns : IGetValidationRuns
    {

    }

    public class GetFinanceValidationRuleRuns : IGetFinanceValidationRuleRuns
    {
        private readonly IWiseMetaService _wiseMetaService;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;

        public GetFinanceValidationRuleRuns(IWiseMetaService wiseMetaService, Func<IApiKeyContextProvider> apiKeyContextProviderFactory, Func<ISchoolYearContextProvider> schoolYearContextProviderFactory)
        {
            _wiseMetaService = wiseMetaService;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
        }

        public async Task<ValidationRuleRun> GetByIdAsync(string id)
        {
            var request = new WISEmeta.Api.Domain.Finance.ValidationRuleRunRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationRuleRuns(request);
            var source = result.Data.FirstOrDefault();
            var rule = source != null ? MapRuleRunFromWiseMeta(source) : new ValidationRuleRun();

            return rule;
        }

        public async Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest rulesRequest)
        {
            var request = MapRequestToWiseMeta(rulesRequest);
            var result = await _wiseMetaService.GetValidationRuleRuns(request);
            var rules = result.Data.Select(MapRuleRunFromWiseMeta).ToList();
            return rules;
        }

        private ValidationRuleRun MapRuleRunFromWiseMeta(WISEmeta.Api.Domain.Finance.ValidationRuleRun source)
        {
            var target = new ValidationRuleRun
            {
                Id = source.Id,
                Host = source.Host,
                RunFinishDateTime = source.RunFinishDateTime,
                RunIdentifier = source.RunIdentifier,
                RunStartDateTime = source.RunStartDateTime,
                RunStatusDescriptor = source.RunStatusDescriptor,
                ValidationEngine = source.ValidationEngine
            };

            return target;
        }

        private WISEmeta.Api.Domain.Finance.ValidationRuleRunRequest MapRequestToWiseMeta(ValidationRuleRunRequest source)
        {
            var target = new WISEmeta.Api.Domain.Finance.ValidationRuleRunRequest
            {
                Offset = source.Offset,
                Limit = source.Limit,
                Id = source.Id,
                Host = source.Host,
                RunIdentifier = source.RunIdentifier,
                ValidationEngine = source.ValidationEngine,
                SchoolYear = _schoolYearContextProviderFactory().GetSchoolYear(),
                ClaimsOrganizations = _apiKeyContextProviderFactory().GetApiKeyContext().EducationOrganizationIds.ToList()
            };

            return target;
        }
    }
}
