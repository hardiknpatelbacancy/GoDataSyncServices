using Microsoft.Extensions.Configuration;

namespace GoDataSyncServices.Services.Configuration
{
    public class ApiConfiguration
    {
        public string BaseUrl { get; }
        public string AuthToken { get; }
        public int PageSize { get; }

        public ApiConfiguration(IConfiguration configuration)
        {
            BaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new Exception("BaseUrl not found in configuration.");
            AuthToken = configuration["ApiSettings:AuthToken"] ?? throw new Exception("AuthToken not found in configuration.");
            PageSize = int.Parse(configuration["ApiSettings:PageSize"] ?? "100");
        }
    }
} 