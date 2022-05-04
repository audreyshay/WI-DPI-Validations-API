using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Rule
{
    public interface IGetValidationRules
    {
        Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest);

        Task<ValidationRule> GetByIdAsync(string id);
    }
}
