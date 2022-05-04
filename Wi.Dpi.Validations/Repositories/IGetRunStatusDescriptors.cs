using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public interface IGetRunStatusDescriptors
    {
        Task<IList<RunStatusDescriptor>> GetAllAsync();

        Task<RunStatusDescriptor> GetByIdAsync(string id);
    }
}
