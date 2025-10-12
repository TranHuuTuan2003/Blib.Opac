
using KMS.Web.Services;

namespace KMS.Web
{
    public class SyncBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer = new Timer(_ => { }, null, Timeout.Infinite, Timeout.Infinite);
        private readonly ILogger<SyncBackgroundService> _logger;
        private readonly Services.PLog.IService _plogService;
        // private readonly ViewCountManager _viewCountManager;

        public SyncBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<SyncBackgroundService> logger,
            Services.PLog.IService plogService
            // ViewCountManager viewCountManager
            )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _plogService = plogService;
            // _viewCountManager = viewCountManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calculate the time until 23:00
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, 9, 11, 0);

            if (now > scheduledTime)
            {
                // If it's already past 23:00 today, schedule it for the next day
                scheduledTime = scheduledTime.AddDays(1);
            }

            // Calculate the initial delay
            TimeSpan initialDelay = scheduledTime - now;

            // Set up the timer to run the background task every 24 hours
            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async Task RunAsyncJobs()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // await SyncLogToCore();
                    // await SyncViewCount();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in background job: {error}", ex.Message);
            }
        }

        private void DoWork(object? state)
        {
            _ = RunAsyncJobs();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer when the application is stopped
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of resources, if any
            _timer?.Dispose();
        }

        // public async Task SyncLogToCore()
        // {
        //     await _plogService.SyncLogToCoreAsync();
        // }

        // public async Task SyncViewCount()
        // {
        //     await _viewCountManager.SyncData();
        // }
    }
}