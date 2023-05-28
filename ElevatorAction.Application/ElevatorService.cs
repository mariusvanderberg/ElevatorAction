using ElevatorAction.Application.Common;
using ElevatorAction.Application.Interfaces;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Application
{
    public class ElevatorService : IElevatorService
    {
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly Elevator _elevator;
        public ElevatorService(Elevator elevator, IAsyncDelayer asyncDelayer)
        {
            _elevator = elevator;
            _asyncDelayer = asyncDelayer;
        }

        /// <inheritdoc/>
        public bool CapacityReached()
        {
            return _elevator.CurrentPersons >= _elevator.MaxPersons; // Could be that people overload, or that it's 10 cats and 1 human
        }

        /// <inheritdoc/>
        public IEnumerable<Floor> GetAvailableFloors()
        {
            return _elevator.GetFloors();
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
        public bool HasFloor(int floorNumber)
        {
            return _elevator.GetFloors().Any(x => x.Number == floorNumber);
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
        public async Task<bool> MoveToFloorAsync(int floorNumber, ElevatorDirection direction, CancellationToken stoppingToken)
        {
            if (!HasFloor(floorNumber))
            {
                throw new ArgumentOutOfRangeException(nameof(floorNumber), Constants.Operation.InvalidFloor);
            }
            await SimulateDoorsClosingAsync(stoppingToken);

            Console.WriteLine($"{string.Format(Constants.Operation.ElevatorMoving, _elevator.Id, _elevator.CurrentFloor, floorNumber)} ");
            await SimulateMovementAsync(floorNumber, DetermineDirection(floorNumber), stoppingToken); // Moving up or down

            // Now we have arrived
            _elevator.CurrentFloor = floorNumber;
            _elevator.ElevatorState = ElevatorState.Stationary;
            _elevator.Direction = default;
            await SimulateDoorsOpeningAsync(stoppingToken);
            Console.WriteLine(string.Format(Constants.Operation.ElevatorArrived, _elevator.Id, floorNumber));

            // Offload all people
            _elevator.CurrentPersons = 0;

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessRequestAsync(Request request, CancellationToken stoppingToken)
        {
            if (!HasFloor(request.Floor))
            {
                throw new ArgumentOutOfRangeException(nameof(request), Constants.Operation.InvalidFloor);
            }
            // Process the request here (e.g., open doors, handle passengers, etc.)
            Console.WriteLine(string.Format(Constants.Operation.ElevatorOnRoute, _elevator.Id, request.Floor, request.People));

            // No need to simulate movement if floor is same
            if (_elevator.CurrentFloor != request.Floor)
            {
                await SimulateMovementAsync(request.Floor, DetermineDirection(request.Floor), stoppingToken);
            }

            Console.WriteLine(string.Format(Constants.Operation.ElevatorArrived, _elevator.Id, request.Floor));
            _elevator.ElevatorState = ElevatorState.Loading; // Could be used to indicate some loading
            _elevator.CurrentFloor = request.Floor;
            _elevator.Direction = default;
            await SimulateDoorsOpeningAsync(stoppingToken);
            _elevator.CurrentPersons = request.People;

            return true;
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

        /// <summary>
        /// Simulates elevator doors closing
        /// </summary>
        /// <param name="stoppingToken"><see cref="CancellationToken"/></param>
        /// <returns>bool indicating success</returns>
        private async Task<bool> SimulateDoorsClosingAsync(CancellationToken stoppingToken)
        {
            Console.Write(Constants.Doors.Closing);
            await _asyncDelayer.Delay(Constants.Doors.OpenClosingDelay, stoppingToken);
            Console.WriteLine($"{Constants.Doors.Closed} ");
            await _asyncDelayer.Delay(Constants.Doors.OpenClosedDelay, stoppingToken);

            return true;
        }

        /// <summary>
        /// Simulates elevator doors opening
        /// </summary>
        /// <param name="stoppingToken"><see cref="CancellationToken"/></param>
        /// <returns>bool indicating success</returns>
        private async Task<bool> SimulateDoorsOpeningAsync(CancellationToken stoppingToken)
        {
            Console.Write($"{Constants.Doors.Opening} ");
            await _asyncDelayer.Delay(Constants.Doors.OpenClosingDelay, stoppingToken);
            Console.Write($"{Constants.Doors.Open} ");
            Console.WriteLine();

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

            Console.WriteLine(string.Format(Constants.Operation.Movement, direction.ToString().ToLower()));

            int floor = _elevator.CurrentFloor;

            // Simulating elevator movement
            while (floor != floorNumber)
            {
                // Check if cancellation has been requested
                if (stoppingToken.IsCancellationRequested)
                {
                    // Handle emergency stop, e.g., stop the elevator immediately
                    Console.WriteLine(Constants.Operation.EmergencyStop);
                    MakeEmergencyStop();
                    return false;
                }

                // Perform elevator movement logic, e.g., update current floor, move up or down, etc.
                if (direction == ElevatorDirection.Up)
                    floor++;
                else
                    floor--;

                // Some floors are not supported by the elevator, so update when the correct floor has been reached
                _elevator.CurrentFloor = floor;

                // Simulate delay between floors. This is a Maglev elevator, super fast!
                await _asyncDelayer.Delay(500, stoppingToken);

                // Clears input
                Console.Write("\r" + new string(' ', floor.ToString().Length) + "\r");

                // Print current floor
                Console.Write(string.Format(Constants.Formatting.ElevatorMovingFormat, _elevator.CurrentPersons, floor));
            }
            Console.WriteLine();

            return true;
        }
    }
}