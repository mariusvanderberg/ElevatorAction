using ElevatorAction.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ElevatorAction.Application.Workers;

public class ElevatorBackgroundService : BackgroundService
{
    private readonly IElevatorControlService _elevatorControlService;
    private readonly IConfiguration _configuration;

    public ElevatorBackgroundService(IElevatorControlService elevatorControlService, IConfiguration configuration)
    {
        _elevatorControlService = elevatorControlService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!int.TryParse(_configuration["BackgroundQueueProcessor:PollingInSeconds"], out var pollingInSeconds))
        {
            pollingInSeconds = 20;
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            // Perform background tasks here
            // Example: Check for elevator requests and process them

            // Call the elevator control service method to process requests
            await _elevatorControlService.ProcessElevatorRequestsAsync();

            // Add any additional background tasks or logic

            // Delay for a certain interval before the next iteration
            // 20 seconds is a long delay, but seems apropraite to handle all requests
            // since we are very limited with a console. This can be reduced significantly
            // if an api is used.
            await Task.Delay(TimeSpan.FromSeconds(pollingInSeconds), stoppingToken);
        }
    }
}