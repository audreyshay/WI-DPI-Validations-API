using EdFi.Ods.Common.Security;
using log4net;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;
using Wi.Dpi.Validations.Repositories.Result;

namespace Wi.Dpi.Validations.Repositories.Rule
{
    public class GetValidationRules : IGetValidationRules
    {
        private readonly IGetCollectionValidationRules _getCollectionValidationRules;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILog _logger = LogManager.GetLogger(typeof(GetValidationRules));

        public GetValidationRules(IGetCollectionValidationRules getCollectionValidationRules, Func<IApiKeyContextProvider> apiKeyContextProviderFactory, IWebHostEnvironment webHostEnvironment, TelemetryClient telemetryClient)
        {
            _getCollectionValidationRules = getCollectionValidationRules;
            _apiKeyContextProviderFactory = apiKeyContextProviderFactory;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ValidationRule> GetByIdAsync(string id)
        {
            var rule = await _getCollectionValidationRules.GetByIdAsync(id);
            return rule;
        }

        public async Task<IList<ValidationRule>> GetAllAsync(ValidationRuleRequest rulesRequest)
        {
            var rules = await _getCollectionValidationRules.GetAllAsync(rulesRequest);
            return rules.ToList();

        }
    }
}
