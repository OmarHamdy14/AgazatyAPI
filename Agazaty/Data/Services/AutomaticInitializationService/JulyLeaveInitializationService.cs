using Agazaty.Data.Services.Interfaces;

namespace Agazaty.Data.Services.AutomaticInitializationService
{
    public class JulyLeaveInitializationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public JulyLeaveInitializationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow.Month == 7 && DateTime.UtcNow.Day == 1) // Run on July 1st
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var leaveService = scope.ServiceProvider.GetRequiredService<IAccountService>();
                        await leaveService.InitalizeLeavesCountOfUser();
                    }
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Check once a day
            }
        }
    }
}