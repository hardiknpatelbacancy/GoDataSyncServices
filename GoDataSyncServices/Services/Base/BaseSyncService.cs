using System.Net.Http.Headers;

namespace GoDataSyncServices.Services.Base
{
    public abstract class BaseSyncService : ISyncService
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly IConfiguration _configuration;
        protected readonly ILogger _logger;
        protected readonly string _connectionString;
        protected readonly string _authToken;

        protected BaseSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new Exception("Connection string not found.");
            _authToken = configuration["ApiSettings:AuthToken"] ?? 
                throw new Exception("AuthToken not found in configuration.");
        }

        public abstract Task<SyncResult> SyncAsync(string tenantId = null, string companyId = null);

        protected HttpClient CreateHttpClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        protected SyncResult CreateSuccessResult(string message, int totalRecordsSynced)
        {
            return new SyncResult
            {
                Success = true,
                Message = message,
                TotalRecordsSynced = totalRecordsSynced
            };
        }

        protected SyncResult CreateErrorResult(string error)
        {
            return new SyncResult
            {
                Success = false,
                Error = error
            };
        }
    }
} 