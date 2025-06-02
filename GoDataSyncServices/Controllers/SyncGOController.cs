using Microsoft.AspNetCore.Mvc;
using GoDataSyncServices.Services;
using GoDataSyncServices.Services.Configuration;

namespace GoDataSyncServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncGOController : ControllerBase
    {
        private readonly ILogger<SyncGOController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiConfiguration _apiConfig;
        private readonly ILoggerFactory _loggerFactory;

        public SyncGOController(
            ILogger<SyncGOController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _apiConfig = new ApiConfiguration(configuration);
            _loggerFactory = loggerFactory;
        }

        [HttpPost("tenants")]
        public async Task<IActionResult> SyncTenants()
        {
            var serviceLogger = _loggerFactory.CreateLogger<TenantSyncService>();
            var service = new TenantSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync();
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("companies")]
        public async Task<IActionResult> SyncCompanies()
        {
            var serviceLogger = _loggerFactory.CreateLogger<CompanySyncService>();
            var service = new CompanySyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync();
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("projects")]
        public async Task<IActionResult> SyncProjects([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
            {
                return BadRequest("Tenants ID and Companies ID are required");
            }

            var serviceLogger = _loggerFactory.CreateLogger<ProjectSyncService>();
            var service = new ProjectSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync(tenants_id, companies_id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("tasks")]
        public async Task<IActionResult> SyncTasks([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
            {
                return BadRequest("Tenants ID and Companies ID are required");
            }

            var serviceLogger = _loggerFactory.CreateLogger<TaskSyncService>();
            var service = new TaskSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync(tenants_id, companies_id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("locations")]
        public async Task<IActionResult> SyncLocations([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
            {
                return BadRequest("Tenants ID and Companies ID are required");
            }

            var serviceLogger = _loggerFactory.CreateLogger<LocationSyncService>();
            var service = new LocationSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync(tenants_id, companies_id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("workflows")]
        public async Task<IActionResult> SyncWorkflows([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
            {
                return BadRequest("Tenants ID and Companies ID are required");
            }

            var serviceLogger = _loggerFactory.CreateLogger<WorkflowSyncService>();
            var service = new WorkflowSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync(tenants_id, companies_id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }

        [HttpPost("clients")]
        public async Task<IActionResult> SyncClients([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
            {
                return BadRequest("Tenants ID and Companies ID are required");
            }

            var serviceLogger = _loggerFactory.CreateLogger<ClientSyncService>();
            var service = new ClientSyncService(_httpClientFactory, _configuration, serviceLogger, _apiConfig);
            var result = await service.SyncAsync(tenants_id, companies_id);
            return result.Success ? Ok(result) : StatusCode(500, result);
        }
    }
}

  