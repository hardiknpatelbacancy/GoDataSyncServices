using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using GoDataSyncServices.Models;

namespace GoDataSyncServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncGOController : ControllerBase
    {
        private readonly string _connectionString;

        public SyncGOController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found.");
        }

        [HttpPost]
        public async Task<IActionResult> SyncTables()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    var tenants = await db.QueryAsync<Tenants>("SELECT * FROM [dbo].[Tenants]");
                    return Ok(tenants);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 