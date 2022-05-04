using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EdFi.Ods.Common.Security;
using Wi.Dpi.Validations.Models;
using Wi.Dpi.Validations.Repositories.Result;

namespace Wi.Dpi.Validations.Repositories.RuleRun
{
    public class GetValidationRuleRuns : IGetValidationRuns
    {
        private readonly IGetCollectionValidationRuleRuns _getCollectionValidationRuleRuns;
      private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;

        public GetValidationRuleRuns(IGetCollectionValidationRuleRuns getCollectionValidationRuleRuns, Func<IApiKeyContextProvider> apiKeyContextProviderFactory)
        {
            _getCollectionValidationRuleRuns = getCollectionValidationRuleRuns;
             _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
        }

        public async Task<ValidationRuleRun> GetByIdAsync(string id)
        {
            var run = await _getCollectionValidationRuleRuns.GetByIdAsync(id);
            return run;
        }


        public async Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest runRequest)
        {
            var runs = await _getCollectionValidationRuleRuns.GetAllAsync(runRequest);
            return runs.ToList();
        }
    }
}
