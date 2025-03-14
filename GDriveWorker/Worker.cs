namespace GDriveWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly int _delayTime;

        public Worker(ILogger<Worker> logger, int milliSeconds)
        {
            _logger = logger;
            _delayTime = milliSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("{delayTime} Worker running at: {time}", _delayTime, DateTimeOffset.Now);
                }
                await Task.Delay(_delayTime, stoppingToken);
            }
        }
    }
}
