using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;

namespace ElevatorAction.Application
{
    public class ElevatorService : IElevatorService
    {
        private readonly Elevator _elevator;

        public ElevatorService(Elevator elevator)
        {
            _elevator = elevator;
        }

        public void ProcessRequest(Request request)
        {
            // Process the request here (e.g., open doors, handle passengers, etc.)
            Console.WriteLine($"Elevator {_elevator.Id} processing request for floor {request.Floor}");
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
        public void MoveToFloor(int floorNumber)
        {
            Console.WriteLine($"{_elevator.CurrentFloor} moving to floor {floorNumber}");

            // Some delay...

            Thread.Sleep(10);
            _elevator.CurrentFloor = floorNumber;
            Console.WriteLine($"{_elevator.Id} has arrived on floor {floorNumber}");

            // Ding Dong, elevator is ready...

            // Trigger a domain event

            _elevator.CurrentPersons = 10; // TODO: Update with actual amount of people
            Thread.Sleep(3);

            // Prompt for selection
            // Let's say they chose to move down to the bottom

            Console.WriteLine($"Doors closing...");
            Thread.Sleep(1);
            Console.WriteLine($"Doors closed. In transit");
            _elevator.ElevatorState = ElevatorState.Moving;
            _elevator.Direction = ElevatorDirection.Down;

            Thread.Sleep(10);

            // Arrived
            _elevator.CurrentFloor = -2;
            _elevator.ElevatorState = ElevatorState.Stationary;
            _elevator.CurrentPersons = 1; // Someone left their purse and need to go back up... They press floor 7 again
            _elevator.Direction = default;

            // User makes selection
            // Elevator door closes, elevator starts moving.
            // Update persons
            // Update elevator status to moving
            // Be ready for when someone presses emergency stop...
        }        
    }
}