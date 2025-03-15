using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class UploadService : BackgroundService
    {
        private readonly ILogger<UploadService> _logger;
        private ISQLiteDB _sqliteDB;
        private readonly int _delayTime = 15000;
        Random rand = new Random();

        public UploadService(ILogger<UploadService> logger, ISQLiteDB sqLiteDB)
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
                    int num = rand.Next(100);
                    _sqliteDB.InsertUploadRecord($"File_{num}", DateTime.Now.ToString());
                    _logger.LogInformation($"File_{num} Inserted");
                }
                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task InsertFile()
        {
            int num = rand.Next(100);
            _sqliteDB.InsertUploadRecord($"File_{num}", DateTime.Now.ToString());
            _logger.LogInformation($"File_{num} Inserted");
        }
    }
}
