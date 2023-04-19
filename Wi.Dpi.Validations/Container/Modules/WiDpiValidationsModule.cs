using Autofac;
using EdFi.Ods.Api.Providers;
using EdFi.Ods.Api.Routing;
using EdFi.Ods.Common.Configuration;
using EdFi.Ods.Common.Container;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Wi.Dpi.Validations.Controllers;
using Wi.Dpi.Validations.OpenApi;
using Wi.Dpi.Validations.Repositories;
using Wi.Dpi.Validations.Repositories.Result;
using Wi.Dpi.Validations.Repositories.Rule;
using Wi.Dpi.Validations.Repositories.RuleRun;

namespace Wi.Dpi.Validations.Container.Modules
{
    public class WiDpiValidationsModule : ConditionalModule
    {
        public WiDpiValidationsModule(ApiSettings apiSettings)
            : base(apiSettings, nameof(WiDpiValidationsModule)) { }

        public override bool IsSelected() => IsFeatureEnabled(ValidationsConstants.Validations);

        public override void ApplyConfigurationSpecificRegistrations(ContainerBuilder builder)
        {
            builder.RegisterType<ValidationsOpenApiContentProvider>()
                .As<IOpenApiContentProvider>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ValidationsOpenApiMetadataRouteInformation>()
                .As<IOpenApiMetadataRouteInformation>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ValidationsControllerRouteConvention>()
                .As<IApplicationModelConvention>()
                .SingleInstance();

            builder.RegisterType<GetValidationRules>()
                .As<IGetValidationRules>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetCollectionValidationRules>()
                .As<IGetCollectionValidationRules>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetFinanceValidationRules>()
                .As<IGetFinanceValidationRules>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetValidationResults>()
                .As<IGetValidationResults>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetFinanceValidationResults>()
                .As<IGetFinanceValidationResults>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetCollectionValidationResults>()
                .As<IGetCollectionValidationResults>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetValidationRuleRuns>()
                .As<IGetValidationRuns>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetFinanceValidationRuleRuns>()
                .As<IGetFinanceValidationRuleRuns>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetCollectionValidationRuleRuns>()
                .As<IGetCollectionValidationRuleRuns>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetRuleStatusDescriptors>()
                .As<IGetRuleStatusDescriptors>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetRunStatusDescriptors>()
                .As<IGetRunStatusDescriptors>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetSeverityDescriptors>()
                .As<IGetSeverityDescriptors>()
                .InstancePerLifetimeScope();

            builder.RegisterType<GetValidationLogicTypeDescriptors>()
                .As<IGetValidationLogicTypeDescriptors>()
                .InstancePerLifetimeScope();

            builder.RegisterType<EducationOrganizationReferenceProvider>()
                .As<IEducationOrganizationReferenceProvider>()
                .InstancePerLifetimeScope();

           
        }
    }
}
