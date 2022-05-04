using EdFi.Ods.Api.ExceptionHandling;
using EdFi.Ods.Common.Configuration;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Wi.Dpi.Validations.Models;
using Wi.Dpi.Validations.Repositories;
using Wi.Dpi.Validations.Repositories.Rule;

namespace Wi.Dpi.Validations.Controllers
{
    [Description("Find validation rules")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/validationRules")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class ValidationRulesController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private readonly IGetValidationRules _getRules;
        private readonly int _defaultPageLimitSize;


        public ValidationRulesController(ApiSettings apiSettings, IDefaultPageSizeLimitProvider defaultPageSizeLimitProvider, IGetValidationRules getRules = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getRules = getRules;
            _defaultPageLimitSize = defaultPageSizeLimitProvider.GetDefaultPageSizeLimit();
        }


        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ValidationRule>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ValidationRuleRequest rulesRequest)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationRulesController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
            if (rulesRequest.Limit != null)
            {
                if (rulesRequest.Limit <= 0 || rulesRequest.Limit > _defaultPageLimitSize)
                {
                    return BadRequest(ErrorTranslator.GetErrorMessage($"Limit must be omitted or set to a value between 1 and {_defaultPageLimitSize}."));
                }
            }
            else
            {
                rulesRequest.Limit = _defaultPageLimitSize;
            }

            var rules = await _getRules.GetAllAsync(rulesRequest);
            return Ok(rules);
        }

        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ValidationRule), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationRulesController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var rules = await _getRules.GetByIdAsync(id);

            if (rules == null)
            {
                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            return Ok(rules);
        }
    }
}
