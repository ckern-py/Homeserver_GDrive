using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class DBMaintenance : BackgroundService
    {
        private readonly ILogger<DBMaintenance> _logger;
        private readonly ISQLiteDB _sqliteDB;

        public DBMaintenance(ILogger<DBMaintenance> logger, ISQLiteDB sqLiteDB)
        {
            _logger = logger;
            _sqliteDB = sqLiteDB;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int delCount = _sqliteDB.DeleteOldRecords();
                if (_logger.IsEnabled(LogLevel.Information))
                {                    
                    _logger.LogInformation("Removed {delCount} Records at {datetime}", delCount, DateTime.Now);
                }
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
