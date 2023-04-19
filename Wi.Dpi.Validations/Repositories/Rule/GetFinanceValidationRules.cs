using EdFi.Ods.Common.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public GetFinanceValidationRules(IWiseMetaService wiseMetaService)
        {
            _wiseMetaService = wiseMetaService;
        }

        public async Task<ValidationRule> GetByIdAsync(string id)
        {
            var request = new ValidationRuleRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationRules(request);
            var source = result.FirstOrDefault();
            return source;
        }

        public async Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest)
        {
            var result = await _wiseMetaService.GetValidationRules(rulesRequest);
            var rules = result;
            return rules;
        }

   
    }
}
