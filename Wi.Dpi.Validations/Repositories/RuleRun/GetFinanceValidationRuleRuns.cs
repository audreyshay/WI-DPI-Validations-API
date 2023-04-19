using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public GetFinanceValidationRuleRuns(IWiseMetaService wiseMetaService)
        {
            _wiseMetaService = wiseMetaService;
        }

        public async Task<ValidationRuleRun> GetByIdAsync(string id)
        {
            var request = new ValidationRuleRunRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationRuleRuns(request);
            var source = result.FirstOrDefault();
          
            return source;
        }

        public async Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest rulesRequest)
        {
            var result = await _wiseMetaService.GetValidationRuleRuns(rulesRequest);
            var rules = result;
            return rules;
        }

        }
}
