using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class UploadService : BackgroundService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly ISQLiteDB _sqliteDB;
        private readonly IGDriveLogic _gDriveLogic;
        private readonly int _delayTime = 15000;
        private readonly Random rand;

        public UploadService(ILogger<UploadService> logger, ISQLiteDB sqLiteDB, IGDriveLogic gDriveLogic)
        {
            _logger = logger;
            _sqliteDB = sqLiteDB;
            rand = new Random();
            _gDriveLogic = gDriveLogic;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Running UploadService at {dateTime}", DateTime.Now);                   
                }
                Task.Run(() => _gDriveLogic.UploadMediaDirectory("/../media/"));

                await Task.Delay(1000000, stoppingToken);
            }
        }
    }
}
