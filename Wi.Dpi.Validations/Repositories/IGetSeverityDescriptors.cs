using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public interface IGetSeverityDescriptors
    {
        Task<IList<SeverityDescriptor>> GetAllAsync();

        Task<SeverityDescriptor> GetByIdAsync(string id);
    }
}
