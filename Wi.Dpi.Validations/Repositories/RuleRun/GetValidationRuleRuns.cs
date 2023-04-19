using EdFi.Ods.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.RuleRun
{
    public class GetValidationRuleRuns : IGetValidationRuns
    {
        private readonly IGetCollectionValidationRuleRuns _getCollectionValidationRuleRuns;
        private readonly IGetFinanceValidationRuleRuns _getFinanceValidationRuleRuns;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;

        public GetValidationRuleRuns(IGetCollectionValidationRuleRuns getCollectionValidationRuleRuns, IGetFinanceValidationRuleRuns getFinanceValidationRuleRuns, Func<IApiKeyContextProvider> apiKeyContextProviderFactory)
        {
            _getCollectionValidationRuleRuns = getCollectionValidationRuleRuns;
            _getFinanceValidationRuleRuns = getFinanceValidationRuleRuns;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
        }

        public async Task<ValidationRuleRun> GetByIdAsync(string id)
        {
            var run = _apiKeyContextProviderFactory.IsFinance() ?
                await _getFinanceValidationRuleRuns.GetByIdAsync(id) :
                await _getCollectionValidationRuleRuns.GetByIdAsync(id);
            return run;
        }


        public async Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest runRequest)
        {
            var runs = _apiKeyContextProviderFactory.IsFinance() ?
                await _getFinanceValidationRuleRuns.GetAllAsync(runRequest) :
                await _getCollectionValidationRuleRuns.GetAllAsync(runRequest);
            return runs.ToList();
        }
    }
}
