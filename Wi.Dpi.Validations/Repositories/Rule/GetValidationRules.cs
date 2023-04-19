using EdFi.Ods.Common.Security;
using log4net;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Rule
{
    public class GetValidationRules : IGetValidationRules
    {
        private readonly IGetCollectionValidationRules _getCollectionValidationRules;
        private readonly IGetFinanceValidationRules _getFinanceValidationRules;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;

        public GetValidationRules(IGetCollectionValidationRules getCollectionValidationRules, IGetFinanceValidationRules getFinanceValidationRules, Func<IApiKeyContextProvider> apiKeyContextProviderFactory)
        {
            _getCollectionValidationRules = getCollectionValidationRules;
            _getFinanceValidationRules = getFinanceValidationRules;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
        }

        public async Task<ValidationRule> GetByIdAsync(string id)
        {
            var rule = _apiKeyContextProviderFactory.IsFinance() ?
                await _getFinanceValidationRules.GetByIdAsync(id) :
                await _getCollectionValidationRules.GetByIdAsync(id);
            return rule;
        }

        public async Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest)
        {
            var rules = _apiKeyContextProviderFactory.IsFinance() ?
                await _getFinanceValidationRules.GetAllAsync(rulesRequest) :
                await _getCollectionValidationRules.GetAllAsync(rulesRequest);
            return rules.ToList();

        }
    }
}
