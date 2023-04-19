using EdFi.Ods.Common.Security;
using System;

namespace Wi.Dpi.Validations
{
    public static class ApiKeyContextProviderExtensions
    {
        public static bool IsFinance(this Func<IApiKeyContextProvider> apiKeyContextProviderFactory)
            => apiKeyContextProviderFactory().GetApiKeyContext().IsFinance();

        public static bool IsFinance(this ApiKeyContext context) => context.ClaimSetName?.StartsWith("Finance") ?? false;

    }
}
