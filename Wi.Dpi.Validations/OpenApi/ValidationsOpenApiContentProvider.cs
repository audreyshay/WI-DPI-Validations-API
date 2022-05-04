using EdFi.Ods.Api.Constants;
using EdFi.Ods.Api.Models;
using EdFi.Ods.Api.Providers;
using EdFi.Ods.Common.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wi.Dpi.Validations.OpenApi
{
    public class ValidationsOpenApiContentProvider : IOpenApiContentProvider
    {
      public string RouteName => ValidationsConstants.ValidationsMetadataRouteName;

        public IEnumerable<OpenApiContent> GetOpenApiContent()
        {
            var assembly = GetType()
                .Assembly;

            return assembly
                .GetManifestResourceNames()
                .Where(x => x.EndsWith("Validations.json"))
                .Select(
                    x => new OpenApiContent(OpenApiMetadataSections.Other, ValidationsConstants.FeatureName, new Lazy<string>(() => assembly.ReadResource(x)), $"{ValidationsConstants.FeatureName}/v1", string.Empty));
        }
    }
}
