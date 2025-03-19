using GDriveWorker.Domain;

namespace GDriveWorker
{
    public class UploadService : BackgroundService
    {
        private readonly ILogger<UploadService> _logger;
        private readonly ISQLiteDB _sqliteDB;
        private readonly IGDriveLogic _gDriveLogic;
        private readonly IConfiguration _configuration;

        public UploadService(ILogger<UploadService> logger, ISQLiteDB sqLiteDB, IGDriveLogic gDriveLogic, IConfiguration configuration)
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
                _sqliteDB.InsertInformationdRecord($"Running UploadService", DateTime.Now.ToString());
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Running UploadService at {dateTime}", DateTime.Now);
                }
                Task.Run(() => _gDriveLogic.UploadMediaDirectory("/../media/"));

                double delayHours = Convert.ToDouble(_configuration["AppSettings:UploadServiceDelayHours"]);
                _sqliteDB.InsertInformationdRecord($"UploadService will execute again in {delayHours} hours", DateTime.Now.ToString());
                await Task.Delay(TimeSpan.FromHours(delayHours), stoppingToken);
            }
        }
    }
}
