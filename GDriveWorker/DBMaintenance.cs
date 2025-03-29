using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class DBMaintenance : BackgroundService
    {
        private readonly ILogger<DBMaintenance> _logger;
        private readonly ISQLiteDB _sqliteDB;
        private readonly IConfiguration _configuration;

        public DBMaintenance(ILogger<DBMaintenance> logger, ISQLiteDB sqLiteDB, IConfiguration configuration)
        {
            _logger = logger;
            _sqliteDB = sqLiteDB;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Running DBMaintenance at {dateTime}", DateTime.Now);
                }
                _sqliteDB.InsertInformationdRecord($"Running DBMaintenance", DateTime.Now.ToString());

                int delUploadCount = _sqliteDB.DeleteOldFileUploadsRecords();
                _sqliteDB.InsertInformationdRecord($"Removed {delUploadCount} records from [FileUploads]", DateTime.Now.ToString());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delUploadCount} records from [FileUploads] at {datetime}", delUploadCount, DateTime.Now);
                }

                int delErrorCount = _sqliteDB.DeleteOldErrorsRecords();
                _sqliteDB.InsertInformationdRecord($"Removed {delErrorCount} records from [Errors]", DateTime.Now.ToString());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delErrorCount} records from [Errors] at {datetime}", delErrorCount, DateTime.Now);
                }

                int delInfoCount = _sqliteDB.DeleteOldInformationRecords();
                _sqliteDB.InsertInformationdRecord($"Removed {delInfoCount} records from [Information]", DateTime.Now.ToString());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Removed {delInfoCount} records from [Information] at {datetime}", delInfoCount, DateTime.Now);
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("DBMaintenance finished {dateTime}", DateTime.Now);
                }
                _sqliteDB.InsertInformationdRecord($"DBMaintenance finished", DateTime.Now.ToString());

                double delayHours = Convert.ToDouble(_configuration["AppSettings:DBMaintenanceDelayHours"]);
                _sqliteDB.InsertInformationdRecord($"DBMaintenance will execute again in {delayHours} hours", DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromHours(delayHours), stoppingToken);
            }
        }
    }
}