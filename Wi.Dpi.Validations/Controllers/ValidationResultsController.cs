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
using Wi.Dpi.Validations.Repositories.Result;

namespace Wi.Dpi.Validations.Controllers
{
    [Description("Find validationResults")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/validationResults")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class ValidationResultsController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private IGetValidationResults _getResults;
        private readonly int _defaultPageLimitSize;

        public ValidationResultsController(ApiSettings apiSettings, IDefaultPageSizeLimitProvider defaultPageSizeLimitProvider, IGetValidationResults getResults = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getResults = getResults;
            _defaultPageLimitSize = defaultPageSizeLimitProvider.GetDefaultPageSizeLimit();
        }

        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ValidationResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ValidationResultRequest resultRequest)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationResultsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }
            if (resultRequest.Limit != null)
            {
                if (resultRequest.Limit <= 0 || resultRequest.Limit > _defaultPageLimitSize)
                {
                    return BadRequest(ErrorTranslator.GetErrorMessage($"Limit must be omitted or set to a value between 1 and {_defaultPageLimitSize}."));
                }
            }
            else
            {
                resultRequest.Limit = _defaultPageLimitSize;
            }
            var results = await _getResults.GetAllAsync(resultRequest);
            return Ok(results);
        }

        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationResultsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var results = await _getResults.GetByIdAsync(id);

            if (results == null)
            {
                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            return Ok(results);
        }
    }
}
