using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.RuleRun
{
    public interface IGetValidationRuns
    {
        Task<IList<ValidationRuleRun>> GetAllAsync(ValidationRuleRunRequest runRequest);

        Task<ValidationRuleRun> GetByIdAsync(string id);
    }
}
