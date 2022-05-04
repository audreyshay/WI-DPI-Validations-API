using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.Common.Context;
using Wi.Dpi.Validations.Models;
using WISEmeta.Api.Client;

namespace Wi.Dpi.Validations.Repositories.Rule
{
    public interface IGetFinanceValidationRules : IGetValidationRules
    {

    }

    public class GetFinanceValidationRules : IGetFinanceValidationRules
    {
        private readonly IWiseMetaService _wiseMetaService;
        private readonly Func<ISchoolYearContextProvider> _schoolYearContextProviderFactory;

        public GetFinanceValidationRules(IWiseMetaService wiseMetaService, Func<ISchoolYearContextProvider> schoolYearContextProviderFactory)
        {
            _wiseMetaService = wiseMetaService;
            _schoolYearContextProviderFactory = schoolYearContextProviderFactory;
        }

        public async Task<ValidationRule> GetByIdAsync(string id)
        {
            var request = new WISEmeta.Api.Domain.Finance.ValidationRuleRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationRules(request);
            var source = result.Data.FirstOrDefault();
            var rule = source != null ? MapRuleFromWiseMeta(source) : new ValidationRule();

            return rule;
        }

        public async Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest)
        {
            var request = MapRequestToWiseMeta(rulesRequest);
            var result = await _wiseMetaService.GetValidationRules(request);
            var rules = result.Data.Select(MapRuleFromWiseMeta).ToList();
            return rules;
        }

        private ValidationRule MapRuleFromWiseMeta(WISEmeta.Api.Domain.Finance.ValidationRule source)
        {
            var target = new ValidationRule
            {
                Id = source.Id,
                RuleIdentifier = source.RuleIdentifier,
                RuleSource = source.RuleSource,
                HelpUrl = source.HelpUrl,
                ShortDescription = source.ShortDescription,
                Description = source.Description,
                RuleStatusDescriptor = source.RuleStatusDescriptor,
                Category = source.Category,
                SeverityDescriptor = source.SeverityDescriptor,
                ExternalRuleId = source.ExternalRuleId,
                ValidationLogicTypeDescriptor = source.ValidationLogicTypeDescriptor,
                ValidationLogic = source.ValidationLogic
            };

            return target;
        }

        private WISEmeta.Api.Domain.Finance.ValidationRuleRequest MapRequestToWiseMeta(ValidationRuleRequest source)
        {
            var target = new WISEmeta.Api.Domain.Finance.ValidationRuleRequest
            {
                Offset = source.Offset,
                Limit = source.Limit,
                Id = source.Id,
                RuleIdentifier = source.RuleIdentifier,
                RuleSource = source.RuleSource,
                RuleStatusDescriptor = source.RuleStatus,
                Category = source.Category,
                SeverityDescriptor = source.SeverityDescriptor,
                ExternalRuleId = source.ExternalRuleId,
                SchoolYear = _schoolYearContextProviderFactory().GetSchoolYear()
            };

            return target;
        }
    }
}
