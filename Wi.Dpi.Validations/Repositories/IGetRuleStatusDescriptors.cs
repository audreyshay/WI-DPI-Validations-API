using System.Collections.Generic;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;

namespace Wi.Dpi.Validations.Repositories
{
    public interface IGetRuleStatusDescriptors
    {
        Task<IList<RuleStatusDescriptor>> GetAllAsync();

        Task<RuleStatusDescriptor> GetByIdAsync(string id);
    }
}
