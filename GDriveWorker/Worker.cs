using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ISQLiteDB _sqliteDB;
        private readonly int _delayTime;
        Random rand = new Random();

        public Worker(ILogger<Worker> logger, int milliSeconds, ISQLiteDB sqLiteDB)
        {
            _logger = logger;
            _delayTime = milliSeconds;
            _sqliteDB = sqLiteDB;            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    int num = rand.Next(100);
                    _logger.LogInformation("{delayTime} Worker running at: {time}", _delayTime, DateTimeOffset.Now);
                    _sqliteDB.InsertUploadRecord($"File_{num}", DateTime.Now.ToString());
                    _logger.LogInformation($"File_{num} Inserted");
                    int delRec = _sqliteDB.DeleteOldRecords();
                    _logger.LogInformation($"Removed {delRec} Records");
                }
                await Task.Delay(_delayTime, stoppingToken);
            }
        }

        private async Task<int> KeepDBClean()
        {
            return _sqliteDB.DeleteOldRecords();
        }
    }
}
