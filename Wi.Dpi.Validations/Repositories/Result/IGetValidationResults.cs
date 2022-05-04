using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories.Result
{
    public interface IGetValidationResults
    {
        Task<IList<ValidationResult>> GetAllAsync(ValidationResultRequest resultRequest);

        Task<ValidationResult> GetByIdAsync(string id);
    }
}
