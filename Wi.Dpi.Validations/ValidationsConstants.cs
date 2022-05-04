using EdFi.Ods.Common.Constants;
using EdFi.Ods.Common.Conventions;

namespace Wi.Dpi.Validations
{
    public class ValidationsConstants
    {
        public const string FeatureName = "Validations";
        public const string FeatureVersion = "1";

        public static readonly string ValidationsMetadataRouteName = EdFiConventions.GetOpenApiMetadataRouteName(FeatureName);

        public static readonly string ValidationsRoutePrefix = $"validations/v{FeatureVersion}";

        public static readonly ApiFeature Validations = new ApiFeature("validations", "Validations");
    }
}
