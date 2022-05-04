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
    /// <summary>
    /// This descriptor describes the severity of a validation rule
    /// </summary>
    [Description("Find severityDescriptors")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/severityDescriptors")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class SeverityDescriptorsController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private IGetSeverityDescriptors _getSeverityDescriptors;

        public SeverityDescriptorsController(ApiSettings apiSettings, IGetSeverityDescriptors getSeverityDescriptors = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getSeverityDescriptors = getSeverityDescriptors;
        }

        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<SeverityDescriptor>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(SeverityDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getSeverityDescriptors.GetAllAsync();
            return Ok(runs);
        }

        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeverityDescriptor), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(SeverityDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getSeverityDescriptors.GetByIdAsync(id);

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
