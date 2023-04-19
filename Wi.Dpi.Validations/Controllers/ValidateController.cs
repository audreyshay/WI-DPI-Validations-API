using EdFi.Common;
using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Api.Services.Controllers.LocalEducationAgencies.EdFi;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.Context;
using EdFi.Ods.Common.Security;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Wi.Dpi.Validations.Controllers
{
    [Description("Triggers Wisconsin DPI L2 Validations")]
    [Authorize]
    [ApiController]
    [Route("ed-fi-xvalidations/validate")]
    [Produces("application/json")]
    [DisplayName("validations")]
    public class ValidateController : ControllerBase
    {
        private ILog _logger;
        protected ILog Logger => _logger ??= LogManager.GetLogger(GetType());
        private readonly bool _isEnabled;
        private readonly Func<IApiKeyContextProvider> _apiKeyContextProviderFactoryFactory;
        private readonly ISchoolYearContextProvider _schoolYearContextProvider;
        private readonly ValidationSettings _validationSettings;
        private readonly IEducationOrganizationReferenceProvider _educationOrganizationReferenceProvider;
        private static readonly HttpClient Client = new HttpClient();

        public ValidateController(ApiSettings apiSettings, Func<IApiKeyContextProvider> apiKeyContextProviderFactory, ISchoolYearContextProvider schoolYearContextProvider, ValidationSettings validationSettings, LocalEducationAgenciesController leaController,
            IEducationOrganizationReferenceProvider educationOrganizationReferenceProvider)
        {
            _apiKeyContextProviderFactoryFactory = apiKeyContextProviderFactory;
            _schoolYearContextProvider = schoolYearContextProvider;
            _validationSettings = validationSettings;
            _educationOrganizationReferenceProvider = educationOrganizationReferenceProvider;
            _isEnabled = apiSettings.IsFeatureEnabled(ValidationsConstants.Validations.GetConfigKeyName());
        }

        ///<summary>Trigger Wisconsin DPI L2 Validations for the associated district(s) and school year.</summary>
        /// <response code="200">The L2 Validations was successfully triggered.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public virtual async Task<IActionResult> Get()
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

            var schoolYear = _schoolYearContextProvider.GetSchoolYear();
            var apiKeyContext = _apiKeyContextProviderFactoryFactory().GetApiKeyContext();
            var leas = apiKeyContext.EducationOrganizationIds;
            var priority = 100;
            var order = 1; 

            Dictionary<int,string> leaUrls = apiKeyContext.IsFinance()
                ? leas.Distinct().ToDictionary(lea => lea, lea => $"{_validationSettings.FinanceUrl}JobQueue?JobName=L2Validation.Request&EducationOrganizationId={lea}&Priority={priority}&Order={order}&QueuedBy={apiKeyContext.ApiKey}&SchoolYear={schoolYear}")
                : leas.Distinct().ToDictionary(lea => lea, lea => $"{_validationSettings.CollectionsUrl}JobQueue?JobName=L2Validation.Request&LocalEducationAgencyId={lea}&Priority={priority}&Order={order}&QueuedBy={apiKeyContext.ApiKey}&SchoolYear={schoolYear}");


            var results = new List<ValidateResult>();

            foreach (var leaId in leaUrls.Keys)
            {
                var url = leaUrls[leaId];
                var edOrgRef = await _educationOrganizationReferenceProvider.GetEducationOrganizationReferences(leaId);
                var result = new ValidateResult { EducationOrganizationReference = edOrgRef };
                results.Add(result);

                using (var response = await Client.GetAsync(url))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var msg = $"Error triggering validations - StatusCode: {response.StatusCode}";
                        _logger.Error(msg);
                        result.ErrorTriggeringValidation = true;
                        result.Message = msg;
                        continue;
                    }

                    using (var content = response.Content)
                    {
                        string json = await content.ReadAsStringAsync();
                        var jobQueueResults = JsonConvert.DeserializeObject<JobQueueResults>(json);

                        if (jobQueueResults?.Data == null)
                        {
                            var msgResultsWereNull = $"Error triggering validations - JobQueue Results were null";
                            _logger.Error(msgResultsWereNull);
                            result.ErrorTriggeringValidation = true;
                            result.Message = msgResultsWereNull;
                            continue;
                        }

                        if (jobQueueResults.Data.SimilarAlreadyExists)
                        {
                            result.ValidationAlreadyTriggered = true;
                            result.Message = "Validation was not triggered because it was already in the job queue";

                            continue;
                        }

                        
                        result.TriggeredValidation = true;
                        result.Message = "Validation triggered successfully";
                    }
                }
            }

            return Ok(results);
        }

    }

    public class ValidateResult
    {
        public EducationOrganizationReference EducationOrganizationReference { get; set; }
        public bool TriggeredValidation { get; set; }
        public bool ValidationAlreadyTriggered { get; set; }
        public bool ErrorTriggeringValidation { get; set; }
        public string Message { get; set; }
    }

    public class JobQueueResults
    {
        public bool Success { get; set; }
        public string ResponseText { get; set; }
        public JobQueueResult Data { get; set; }
    }

    public class JobQueueResult
    {
        public bool ExactAlreadyExists { get; set; }
        public bool SimilarAlreadyExists { get; set; }
        public int Id { get; set; }
        public int JobQueueStatusId { get; set; }
        public int Priority { get; set; }
        public int Order { get; set; }
        public string QueuedBy { get; set; }
        public System.DateTime QueuedTime { get; set; }

        public int JobId { get; set; }
        public string JobName { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public bool? IsProcessing { get; set; }
        public System.DateTime? LastExecutionStart { get; set; }
        public System.DateTime? LastExecutionComplete { get; set; }
        public int? LastExecutionDuration { get; set; }
        public string LastExecutionMessage { get; set; }
        public string State { get; set; }
        public string ExecutedBy { get; set; }
        public string StoppedBy { get; set; }
        public short? SchoolYear { get; set; }
    }

    public interface IEducationOrganizationReferenceProvider
    {
        Task<EducationOrganizationReference> GetEducationOrganizationReferences(int educationOrganizationId);
    }

    public class EducationOrganizationReferenceProvider : IEducationOrganizationReferenceProvider
    {
        private const string Sql = @"SELECT DISTINCT EducationOrganizationId, Id as ResourceId, Discriminator FROM edfi.EducationOrganization";

        private readonly ISessionFactory _sessionFactory;

        public EducationOrganizationReferenceProvider(ISessionFactory sessionFactory)
        {
            _sessionFactory = Preconditions.ThrowIfNull(sessionFactory, nameof(sessionFactory));
        }

        public async Task<EducationOrganizationReference> GetEducationOrganizationReferences(int educationOrganizationId)
        {
            using (var session = _sessionFactory.OpenStatelessSession())
            {
                return await session.CreateSQLQuery(Sql + " WHERE EducationOrganizationId = :edOrgId;")
                    .SetParameter("edOrgId", educationOrganizationId)
                    .SetResultTransformer(Transformers.AliasToBean<EducationOrganizationReference>())
                    .UniqueResultAsync<EducationOrganizationReference>();
            }
        }

   
    }
}
