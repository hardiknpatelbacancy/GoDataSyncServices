using System.Threading.Tasks;

namespace GoDataSyncServices.Services.Interfaces
{
    public interface ISyncService
    {
        Task<SyncResult> SyncAsync(string tenantId = null, string companyId = null);
    }

    public class SyncResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalRecordsSynced { get; set; }
        public string Error { get; set; }
    }
} 