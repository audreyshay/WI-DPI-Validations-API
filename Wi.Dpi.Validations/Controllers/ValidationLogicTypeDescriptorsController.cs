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

namespace Wi.Dpi.Validations.Controllers
{
    [Description("Find validationLogicTypeDescriptors")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/validationLogicTypeDescriptors")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class ValidationLogicTypeDescriptorsController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private IGetValidationLogicTypeDescriptors _getValidationLogicTypeDescriptors;

        public ValidationLogicTypeDescriptorsController(ApiSettings apiSettings, IGetValidationLogicTypeDescriptors getValidationLogicTypeDescriptors = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getValidationLogicTypeDescriptors = getValidationLogicTypeDescriptors;
        }

        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ValidationLogicTypeDescriptor>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationLogicTypeDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getValidationLogicTypeDescriptors.GetAllAsync();
            return Ok(runs);
        }

        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ValidationLogicTypeDescriptor), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(ValidationLogicTypeDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getValidationLogicTypeDescriptors.GetByIdAsync(id);

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
