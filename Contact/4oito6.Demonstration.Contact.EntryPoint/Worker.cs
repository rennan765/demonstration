using _4oito6.Demonstration.Contact.Application.Interfaces;

namespace _4oito6.Demonstration.Contact.EntryPoint
{
    public class Worker : BackgroundService
    {
        private IContactAppServices _appServices;
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger, IContactAppServices appServices)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTime.UtcNow}");
                await _appServices.MaintainInformationByQueueAsync().ConfigureAwait(false);
                await Task.Delay((int)TimeSpan.FromMinutes(5).TotalMilliseconds, stoppingToken);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _appServices?.Dispose();
            _appServices = null;
            GC.SuppressFinalize(this);
        }
    }
}