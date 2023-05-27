using ElevatorAction.Application.Interfaces;

namespace ElevatorAction.Application
{
    public class AsyncDelayer : IAsyncDelayer
    {
        public Task Delay(int milliSeconds, CancellationToken stoppingToken)
        {
            return Task.Delay(milliSeconds, stoppingToken);
        }
    }
}
