using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;
using WISEmeta.Api.Client;

namespace Wi.Dpi.Validations.Repositories.Result
{
    public interface IGetFinanceValidationResults : IGetValidationResults
    {

    }

    public class GetFinanceValidationResults : IGetFinanceValidationResults
    {
        private readonly IWiseMetaService _wiseMetaService;

        public GetFinanceValidationResults(IWiseMetaService wiseMetaService)
        {
            _wiseMetaService = wiseMetaService;
        }

        public async Task<ValidationResult> GetByIdAsync(string id)
        {
            var request = new ValidationResultRequest { Id = id, Offset = 0, Limit = 1 };
            var result = await _wiseMetaService.GetValidationResults(request);
            var source = result.FirstOrDefault();
            return source;
        }

        public async Task<IList<ValidationResult>> GetAllAsync(ValidationResultRequest resultRequest)
        {
            var result = await _wiseMetaService.GetValidationResults(resultRequest);
            var results = result;
            return results;
        }

     

     }
}
