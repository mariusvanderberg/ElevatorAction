using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;
using System.Drawing;

namespace ElevatorAction.Application
{
    public class ElevatorService : IElevatorService
    {
        private readonly Elevator _elevator;

        public ElevatorService(Elevator elevator)
        {
            _elevator = elevator;
        }

        /// <inheritdoc/>
        public bool CapacityReached()
        {
            return _elevator.CurrentPersons >= _elevator.MaxPersons; // Could be that people overload, or that it's 10 cats and 1 human
        }

        /// <inheritdoc/>
        public int GetCapacity()
        {
            return _elevator.MaxPersons;
        }

        /// <inheritdoc/>
        public int GetCurrentFloor()
        {
            return _elevator.CurrentFloor;
        }

        /// <inheritdoc/>
        public ElevatorDirection? GetElevatorDirection()
        {
            return _elevator.Direction;
        }

        /// <inheritdoc/>
        public ElevatorState GetElevatorState()
        {
            return _elevator.ElevatorState;
        }

        /// <inheritdoc/>
        public int GetNumberOfPeople()
        {
            return _elevator.CurrentPersons;
        }

        /// <inheritdoc/>
        public bool HasSpaceFor(int people)
        {
            return _elevator.CurrentPersons + people <= _elevator.MaxPersons;
        }

        /// <inheritdoc/>
        public void MakeEmergencyStop()
        {
            // Update status
            _elevator.ElevatorState = ElevatorState.OutOfOrder;
        }

        /// <summary>
        /// Calculate actual direction of elevator based on the current floor
        /// </summary>
        /// <param name="destinationFloor">Requested floor</param>
        /// <returns><see cref="ElevatorDirection"/></returns>
        private ElevatorDirection DetermineDirection(int destinationFloor)
        {
            if (_elevator.CurrentFloor > destinationFloor)
                return ElevatorDirection.Down;

            return ElevatorDirection.Up;
        }

        /// <inheritdoc/>
        public async Task<bool> MoveToFloor(int floorNumber, ElevatorDirection direction, CancellationToken stoppingToken)
        {
            Console.Write($"Doors closing... ");
            await Task.Delay(2000);
            Console.WriteLine("Doors closed. ");
            await Task.Delay(1000);

            Console.WriteLine($"{_elevator.Id} moving from floor {_elevator.CurrentFloor} to {floorNumber}. ");
            await SimulateMovementAsync(floorNumber, DetermineDirection(floorNumber), stoppingToken); // Moving up or down

            // Now we have arrived
            _elevator.CurrentFloor = floorNumber;
            _elevator.ElevatorState = ElevatorState.Stationary;
            _elevator.Direction = default;
            Console.Write($"Doors openng... ");
            await Task.Delay(2000);
            Console.Write("Doors open. ");
            Console.WriteLine($"{_elevator.Id} has arrived on floor {floorNumber}");

            // Offload all people
            _elevator.CurrentPersons = 0;

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessRequestAsync(Request request, CancellationToken stoppingToken)
        {
            // Process the request here (e.g., open doors, handle passengers, etc.)
            Console.WriteLine($"Elevator {_elevator.Id} is on the way to floor {request.Floor} to pick up {request.People} people");

            // No need to simulate movement if floor is same
            if (_elevator.CurrentFloor != request.Floor)
            {
                await SimulateMovementAsync(request.Floor, DetermineDirection(request.Floor), stoppingToken);
            }

            Console.WriteLine($"Elevator {_elevator.Id} arrived at floor {request.Floor}.");
            _elevator.ElevatorState = ElevatorState.Loading; // Could be used to indicate some loading
            _elevator.CurrentFloor = request.Floor;
            _elevator.Direction = default;
            Console.Write("Doors opening... ");
            await Task.Delay(1000);
            Console.WriteLine("Doors open.");
            Console.WriteLine();
            _elevator.CurrentPersons = request.People;

            return true;
        }

        /// <summary>
        /// This will simulate and actual elevator moving. We can enhance it by animating it
        /// </summary>
        /// <param name="floorNumber">Requested floor</param>
        /// <param name="direction"><see cref="ElevatorDirection"/>: Movement</param>
        /// <param name="stoppingToken"><see cref="CancellationToken"/>: In case of emergency stop</param>
        /// <returns>bool indicating success</returns>
        private async Task<bool> SimulateMovementAsync(int floorNumber, ElevatorDirection direction, CancellationToken stoppingToken)
        {
            _elevator.ElevatorState = ElevatorState.Moving;
            _elevator.Direction = direction;

            // Simulating elevator movement
            while (_elevator.CurrentFloor != floorNumber)
            {
                // Check if cancellation has been requested
                if (stoppingToken.IsCancellationRequested)
                {
                    // Handle emergency stop, e.g., stop the elevator immediately
                    Console.WriteLine("Emergency stop requested. Stopping elevator.");
                    MakeEmergencyStop();
                    return false;
                }

                // Perform elevator movement logic, e.g., update current floor, move up or down, etc.
                if (direction == ElevatorDirection.Up)
                {
                    _elevator.CurrentFloor++;
                }
                else
                {
                    _elevator.CurrentFloor--;
                }

                // Simulate delay between floors. This is a Maglev elevator, super fast!
                await Task.Delay(TimeSpan.FromMilliseconds(500), stoppingToken);

                // Print current floor
                Console.Write($"Current floor: {_elevator.CurrentFloor}");
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            Console.WriteLine();

            return true;
        }
    }
}