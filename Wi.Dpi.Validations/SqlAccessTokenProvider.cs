using Microsoft.Azure.Services.AppAuthentication;

namespace Wi.Dpi.Domain
{
    public interface ISqlAccessTokenProvider
    {
        string GetAccessToken(string connectionString);
    }

    public class SqlAccessTokenProvider : ISqlAccessTokenProvider
    {
        public bool IsAzure { get; }

        public SqlAccessTokenProvider()
        {
            IsAzure = false;
        }

        public string GetAccessToken(string connectionString) => connectionString.Contains("database.windows.net") ? (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result : null;

    }
}
