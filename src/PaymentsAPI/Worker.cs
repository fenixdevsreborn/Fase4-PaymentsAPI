namespace PaymentsAPI
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentsAPI Worker iniciado.");

            try
            {
                // Mant�m o processo vivo enquanto o host estiver rodando
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Shutdown gracioso
            }
            finally
            {
                _logger.LogInformation("PaymentsAPI Worker finalizado.");
            }
        }
    }
}
