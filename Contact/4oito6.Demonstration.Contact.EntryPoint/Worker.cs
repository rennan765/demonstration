using _4oito6.Demonstration.Contact.Application.Interfaces;

namespace _4oito6.Demonstration.Contact.EntryPoint
{
    public class Worker : BackgroundService
    {
        private IContactAppServices _appServices;

        public Worker(IContactAppServices appServices)
        {
            _appServices = appServices ?? throw new ArgumentNullException(nameof(appServices));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _appServices.MaintainInformationByQueueAsync().ConfigureAwait(false);
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