namespace ElevatorAction.Application.Interfaces;

public interface IAsyncDelayer
{
    Task Delay(int milliSeconds, CancellationToken stoppingToken);
}
