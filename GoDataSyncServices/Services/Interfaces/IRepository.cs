
namespace GoDataSyncServices.Services.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<int> ExecuteAsync(string sql, object param = null);
    }
} 