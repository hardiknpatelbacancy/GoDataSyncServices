using System.Data;
using System.Data.SqlClient;
using Dapper;
using GoDataSyncServices.Services.Interfaces;

namespace GoDataSyncServices.Services.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new Exception("Connection string not found.");
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return await db.ExecuteAsync(sql, param);
        }
    }
} 