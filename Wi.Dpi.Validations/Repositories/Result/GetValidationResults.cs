using EdFi.Ods.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Result
{
    public class GetValidationResults : IGetValidationResults
    {
        private readonly IGetCollectionValidationResults _getCollectionValidationResults;
        private readonly IGetFinanceValidationResults _getFinanceValidationResults;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;

        public GetValidationResults(IGetCollectionValidationResults getCollectionValidationResults, IGetFinanceValidationResults getFinanceValidationResults, Func<IApiKeyContextProvider> apiKeyContextProviderFactory)
        {
            _getCollectionValidationResults = getCollectionValidationResults;
            _getFinanceValidationResults = getFinanceValidationResults;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
        }

        public async Task<ValidationResult> GetByIdAsync(string id)
        {
            var result = _apiKeyContextProviderFactory.IsFinance() ?
                await _getFinanceValidationResults.GetByIdAsync(id) :
                await _getCollectionValidationResults.GetByIdAsync(id);
            return result;
        }

        public async Task<IList<ValidationResult>> GetAllAsync(ValidationResultRequest resultRequest)
        {
            var result = _apiKeyContextProviderFactory.IsFinance() ? 
                await _getFinanceValidationResults.GetAllAsync(resultRequest) :
                await _getCollectionValidationResults.GetAllAsync(resultRequest);
            return result.ToList();
        }
    }


}
