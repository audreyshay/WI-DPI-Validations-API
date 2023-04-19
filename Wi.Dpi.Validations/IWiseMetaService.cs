using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace WISEmeta.Api.Client
{
    public interface IWiseMetaService
    {
        Task<List<ValidationResult>> GetValidationResults(ValidationResultRequest request);
        Task<List<ValidationRule>> GetValidationRules(ValidationRuleRequest request);
        Task<List<ValidationRuleRun>> GetValidationRuleRuns(ValidationRuleRunRequest request);
    }
    
}

