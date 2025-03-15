using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class DBMaintenance : BackgroundService
    {
        private readonly ILogger<DBMaintenance> _logger;
        private ISQLiteDB _sqliteDB;

        public DBMaintenance(ILogger<DBMaintenance> logger, ISQLiteDB sqLiteDB)
        {
            _logger = logger;
            _sqliteDB = sqLiteDB;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    int delCount = _sqliteDB.DeleteOldRecords();
                    _logger.LogInformation($"Removed {delCount} Records");
                }
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
