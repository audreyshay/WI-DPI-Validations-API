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
    /// This descriptor describes the status of a validation rule run
    /// </summary>
    [Description("Find runStatusDescriptors")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/runStatusDescriptors")]
    [Produces("application/json")]
    [DisplayName("runStatusDescriptors")]
    public class RunStatusDescriptorsController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private IGetRunStatusDescriptors _getRunStatusDescriptors;

        public RunStatusDescriptorsController(ApiSettings apiSettings, IGetRunStatusDescriptors getRunStatusDescriptors = null)
        {
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
            _getRunStatusDescriptors = getRunStatusDescriptors;
        }

        ///<summary>Retrieves specific resources using the resource's property values (using the "Get" pattern).</summary>
        /// <response code="200">The requested resources were successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<RunStatusDescriptor>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(RunStatusDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getRunStatusDescriptors.GetAllAsync();
            return Ok(runs);
        }


        ///<summary>Retrieves a specific resource using the resource's identifier (using the "Get By Id" pattern).</summary>
        /// <param name="id">A resource identifier that uniquely identifies the resource.</param>
        /// <response code="200">The requested resource was successfully retrieved.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RunStatusDescriptor), StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get(string id)
        {
            if (!_isEnabled)
            {
                _logger.Debug($"{nameof(RunStatusDescriptorsController)} was matched to handle the request, but the '{ValidationsConstants.Validations}' feature is disabled.");

                // Not Found
                return new ObjectResult(null)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            var runs = await _getRunStatusDescriptors.GetByIdAsync(id);

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
