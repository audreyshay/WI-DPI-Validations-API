
using Microsoft.Azure.Services.AppAuthentication;

namespace Wi.Dpi.Domain
{
    public interface ISqlAccessTokenProvider
    {
        string GetAccessToken();
    }

    public class SqlAccessTokenProvider : ISqlAccessTokenProvider
    {
        public bool IsAzure { get; }

        public SqlAccessTokenProvider()
        {
            IsAzure = false;
        }

        public string GetAccessToken() => IsAzure ? (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result : null;
    }
}