using EdFi.Common.Configuration;
using EdFi.Ods.Api.Constants;
using EdFi.Ods.Common.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using System.Reflection;
using Wi.Dpi.Validations.Controllers;

namespace Wi.Dpi.Validations
{
    public class ValidationsControllerRouteConvention : IApplicationModelConvention
    {
        private readonly ApiSettings _apiSettings;

        public ValidationsControllerRouteConvention(ApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public void Apply(ApplicationModel application)
        {
            var routePrefix = new AttributeRouteModel { Template = CreateRouteTemplate() };
            var controllers =
                application.Controllers.Where(x => x.ControllerType == typeof(ValidationRuleRunsController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(ValidationRulesController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(ValidationResultsController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(RunStatusDescriptorsController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(RuleStatusDescriptorsController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(SeverityDescriptorsController).GetTypeInfo() ||
                                                   x.ControllerType == typeof(ValidationLogicTypeDescriptorsController).GetTypeInfo());

            foreach (ControllerModel controller in controllers)
            {
                foreach (var selector in controller.Selectors)
                {
                    if (selector.AttributeRouteModel != null)
                    {
                        selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            routePrefix,
                            selector.AttributeRouteModel);
                    }
                }
            }

            string CreateRouteTemplate()
            {
                string template = $"{ValidationsConstants.ValidationsRoutePrefix}/";

                if (_apiSettings.GetApiMode() == ApiMode.YearSpecific)
                {
                    template += RouteConstants.SchoolYearFromRoute;
                }

                return template;
            }
        }
    }
}
