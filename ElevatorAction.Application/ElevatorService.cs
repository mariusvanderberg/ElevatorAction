using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;
using System.Drawing;

namespace ElevatorAction.Application
{
    public class ElevatorService : IElevatorService
    {
        private readonly Elevator _elevator;

        public ElevatorService()
        {
            _elevator = new Elevator();
        }

        public ElevatorService(Elevator elevator)
        {
            _elevator = elevator;
        }

        public async Task<bool> ProcessRequestAsync(Request request)
        {
            // Process the request here (e.g., open doors, handle passengers, etc.)
            _elevator.ElevatorState = ElevatorState.Moving;
            _elevator.Direction = request.Direction;
            Console.WriteLine($"Elevator {_elevator.Id} on the way to floor {request.Floor}");
            await Task.Delay(2000);
            _elevator.ElevatorState = ElevatorState.Loading; // Could be used to indicate some loading
            _elevator.CurrentFloor = request.Floor;
            _elevator.Direction = default;
            Console.WriteLine($"Elevator {_elevator.Id} arrived at floor {request.Floor}.");
            Console.Write("Doors opening... ");
            await Task.Delay(5000);
            Console.WriteLine("Doors open.");
            Console.WriteLine();
            _elevator.CurrentPersons = request.People;

            // Now the service needs to ask for input...
            // Publish event ?

            return true;
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

        /// <inheritdoc/>
        public async Task MoveToFloor(int floorNumber, ElevatorDirection direction, CancellationToken stoppingToken)
        {
            Console.Write($"Doors closing... ");
            await Task.Delay(2000);
            Console.Write("Doors closed. ");
            await Task.Delay(1000);
            _elevator.ElevatorState = ElevatorState.Moving;
            _elevator.Direction = direction;
            Console.WriteLine($"{_elevator.Id} moving from floor {_elevator.CurrentFloor} to {floorNumber}. ");
            // Some delay...

            //await Task.Delay(10000);

            // Simulating elevator movement
            while (_elevator.CurrentFloor != floorNumber)
            {
                // Check if cancellation has been requested
                if (stoppingToken.IsCancellationRequested)
                {
                    // Handle emergency stop, e.g., stop the elevator immediately
                    Console.WriteLine("Emergency stop requested. Stopping elevator.");
                    MakeEmergencyStop();
                    return;
                }

                // Perform elevator movement logic, e.g., update current floor, move up or down, etc.
                if (direction == ElevatorDirection.Up)
                {
                    _elevator.CurrentFloor++;
                } else
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

            _elevator.CurrentFloor = floorNumber;
            _elevator.ElevatorState = ElevatorState.Stationary;
            _elevator.Direction = default;
            Console.Write($"Doors openng... ");
            await Task.Delay(2000);
            Console.Write("Doors open. ");
            Console.WriteLine($"{_elevator.Id} has arrived on floor {floorNumber}");

            // We don't know how many people got in, so update that from the caller.
            // We also don't know if the elevator was occupied or empty

            // Ding Dong, elevator is ready...

            // Trigger a domain event

            //_elevator.CurrentPersons = 10; // TODO: Update with actual amount of people
            //Thread.Sleep(3);

            // Prompt for selection
            // Let's say they chose to move down to the bottom

            //Thread.Sleep(1);
            //Console.WriteLine($"Doors closed. In transit");
            //_elevator.ElevatorState = ElevatorState.Moving;
            //_elevator.Direction = ElevatorDirection.Down;

            //Thread.Sleep(10);

            // Arrived
            //_elevator.CurrentFloor = -2;
            //_elevator.ElevatorState = ElevatorState.Stationary;
            _elevator.CurrentPersons = 0;
            
            // Someone left their purse and need to go back up... They press floor 7 again
            //_elevator.Direction = default;

            // User makes selection
            // Elevator door closes, elevator starts moving.
            // Update persons
            // Update elevator status to moving
            // Be ready for when someone presses emergency stop...
        }        
    }
}