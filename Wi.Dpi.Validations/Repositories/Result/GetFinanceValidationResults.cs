using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using EdFi.Security.DataAccess.Repositories;
using Wi.Dpi.Validations.Models;
using WISEmeta.Api.Client;

namespace Wi.Dpi.Validations.Repositories.Result
{
    public interface IGetFinanceValidationResults : IGetValidationResults
    {

    }

    public class GetFinanceValidationResults : IGetFinanceValidationResults
    {
        private readonly IWiseMetaService _wiseMetaService;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;

        public GetFinanceValidationResults(IWiseMetaService wiseMetaService,
            Func<IApiKeyContextProvider> apiKeyContextProviderFactory,
            Func<ISchoolYearContextProvider> schoolYearContextProviderFactory)
        {
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
            _wiseMetaService = wiseMetaService;
        }

        public async Task<ValidationResult> GetByIdAsync(string id)
        {
            var request = new WISEmeta.Api.Domain.Finance.ValidationResultRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationResults(request);
            var source = result.Data.FirstOrDefault();
            var rule = source != null ? MapResultFromWiseMeta(source) : new ValidationResult();
            return rule;
        }

        public async Task<IList<ValidationResult>> GetAllAsync(ValidationResultRequest resultRequest)
        {
            var request = MapRequestToWiseMeta(resultRequest);
            var result = await _wiseMetaService.GetValidationResults(request);
            var results = result.Data.Select(MapResultFromWiseMeta).ToList();
            return results;
        }

        private ValidationResult MapResultFromWiseMeta(WISEmeta.Api.Domain.Finance.ValidationResult source)
        {
            var target = new ValidationResult
            {
                Id = source.Id,
                ResultIdentifier = source.ResultIdentifier,
                ValidationRuleReference = new ValidationRuleReference
                {
                    Id = source?.ValidationRuleReference?.Id,
                    RuleIdentifier = source?.ValidationRuleReference?.RuleIdentifier,
                    RuleSource = source?.ValidationRuleReference?.RuleSource
                },
                ValidationRuleRunReference = new ValidationRuleRunReference
                {
                    Id = source?.ValidationRuleRunReference?.Id,
                    RunIdentifier = source?.ValidationRuleRunReference?.RunIdentifier
                },
                ResourceId = source.ResourceId,
                ResourceType = source.ResourceType,
                EducationOrganizationReference = new EducationOrganizationReference
                {
                    EducationOrganizationId = source.EducationOrganizationReference.EducationOrganizationId,
                    ResourceId = Guid.TryParse(source.EducationOrganizationReference.ResourceId, out var guid) ? guid : new Guid(),
                    Discriminator = source.EducationOrganizationReference.Discriminator
                },
                Namespace = source.Namespace,
                AdditionalContext = source.AdditionalContext.Select(c => new AdditionalContext
                {
                    IdentificationCode = c.Name,
                    CustomizationValue = c.Value
                }).ToArray()
            };

            return target;
        }

        private WISEmeta.Api.Domain.Finance.ValidationResultRequest MapRequestToWiseMeta(ValidationResultRequest source)
        {
            var target = new WISEmeta.Api.Domain.Finance.ValidationResultRequest
            {
                Offset = source.Offset,
                Limit = source.Limit,
                Id = source.Id,
                ResultIdentifier = source.ResultIdentifier,
                RunIdentifier = source.RunIdentifier,
                RuleIdentifier = source.RuleIdentifier,
                RuleSource = source.RuleSource,
                EducationOrganizationId = source.EducationOrganizationId,
                StudentUniqueId = source.StudentUniqueId,
                StaffUniqueId = source.StaffUniqueId,
                Namespace = source.Namespace,
                ResourceId = source.ResourceId,
                ResourceType = source.ResourceType,
                SchoolYear = _schoolYearContextProviderFactory().GetSchoolYear(),
                ClaimsOrganizations = _apiKeyContextProviderFactory().GetApiKeyContext().EducationOrganizationIds.ToList()
        };

            return target;
        }
    }
}
