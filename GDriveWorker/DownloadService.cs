using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class DownloadService : BackgroundService
    {
        private readonly ILogger<DownloadService> _logger;
        private readonly ISQLiteDB _sqliteDB;
        private readonly IGDriveLogic _gDriveLogic;
        private readonly IConfiguration _configuration;

        public DownloadService(ILogger<DownloadService> logger, ISQLiteDB sqLiteDB, IGDriveLogic gDriveLogic, IConfiguration configuration)
        {
            _logger = logger;
            _sqliteDB = sqLiteDB;
            _gDriveLogic = gDriveLogic;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _sqliteDB.InsertInformationdRecord($"Running DownloadService", DateTime.Now.ToString());

                Boolean.TryParse(_configuration["AppSettings:RunDownloadService"], out bool runService);

                if (runService)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Running DownloadService at {dateTime}", DateTime.Now);
                    }
                    Task.Run(() => _gDriveLogic.DownloadMediaDirectory("/../media/download"));
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("DownloadService is currently disabled");
                    }
                    _sqliteDB.InsertInformationdRecord("DownloadService is currently disabled", DateTime.Now.ToString());
                }

                double delayHours = Convert.ToDouble(_configuration["AppSettings:DownloadServiceDelayHours"]);
                _sqliteDB.InsertInformationdRecord($"DownloadService will execute again in {delayHours} hours", DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromHours(delayHours), stoppingToken);
            }
        }
    }
}