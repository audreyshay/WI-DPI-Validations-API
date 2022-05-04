using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public interface IGetValidationLogicTypeDescriptors
    {
        Task<IList<ValidationLogicTypeDescriptor>> GetAllAsync();

        Task<ValidationLogicTypeDescriptor> GetByIdAsync(string id);
    }
}
