using System;
using System.Threading;
using System.Threading.Tasks;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

public class ElevatorBackgroundService : BackgroundService
{
    private readonly IElevatorControlService _elevatorControlService;

    public ElevatorBackgroundService(IElevatorControlService elevatorControlService)
    {
        _elevatorControlService = elevatorControlService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Perform background tasks here
            // Example: Check for elevator requests and process them

            // Call the elevator control service method to process requests
            //await _elevatorControlService.ProcessElevatorRequestsAsync();

            // Add any additional background tasks or logic

            // Delay for a certain interval before the next iteration
            // 10 seconds is a long delay, but seems apropraite to handle all requests
            // since we are very limited with a console. This can be reduced significantly
            // if an api is used.
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        }
    }
}