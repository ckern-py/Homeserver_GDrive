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
                int delUploadCount = _sqliteDB.DeleteOldFileUploadsRecords();
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delUploadCount} records from [FileUploads] at {datetime}", delUploadCount, DateTime.Now);
                }

                int delInfoCount = _sqliteDB.DeleteOldInformationRecords();
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delInfoCount} records from [Information] at {datetime}", delInfoCount, DateTime.Now);
                }

                int delErrorCount = _sqliteDB.DeleteOldErrorsRecords();
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delErrorCount} records from [Errors] at {datetime}", delErrorCount, DateTime.Now);
                }

                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}