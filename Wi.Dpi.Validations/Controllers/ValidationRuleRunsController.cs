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
using Wi.Dpi.Validations.Repositories.RuleRun;

namespace Wi.Dpi.Validations.Controllers
{
    [Description("Find validation runs")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/validationRuns")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class ValidationRuleRunsController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private IGetValidationRuns _getRuns;
        private readonly int _defaultPageLimitSize;

        public ValidationRuleRunsController(ApiSettings apiSettings, IDefaultPageSizeLimitProvider defaultPageSizeLimitProvider, IGetValidationRuns getRuns = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getRuns = getRuns;
            _defaultPageLimitSize = defaultPageSizeLimitProvider.GetDefaultPageSizeLimit();
        }

        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ValidationRuleRun>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ValidationRuleRunRequest runsRequest)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationRuleRunsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
            if (runsRequest.Limit != null)
            {
                if (runsRequest.Limit <= 0 || runsRequest.Limit > _defaultPageLimitSize)
                {
                    return BadRequest(ErrorTranslator.GetErrorMessage($"Limit must be omitted or set to a value between 1 and {_defaultPageLimitSize}."));
                }
            }
            else
            {
                runsRequest.Limit = _defaultPageLimitSize;
            }
            var runs = await _getRuns.GetAllAsync(runsRequest);
            return Ok(runs);
        }

        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ValidationRuleRun), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationRuleRunsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getRuns.GetByIdAsync(id);

            if (runs == null)
            {
                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            return Ok(runs);
        }
    }
}
