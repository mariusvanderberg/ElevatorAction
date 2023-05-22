using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;

namespace ElevatorAction.Application
{
    /// <summary>
    /// This is the implementation for <see cref="IElevatorControlService"/>.
    /// This is the orchastration logic for all elevators and floors in the block
    /// </summary>
    public class ElevatorControlService : IElevatorControlService
    {
        private readonly List<IElevatorService> _elevatorServices;
        private readonly List<Floor> _floors;
        private readonly Queue<Request> _requestQueue;

        public ElevatorControlService(List<IElevatorService> elevatorServices, List<Floor> floors)
        {
            _elevatorServices = elevatorServices;
            _floors = floors;
            _requestQueue = new Queue<Request>();
        }

        public void AddRequest(int floor, int people, ElevatorDirection direction)
        {
            int maximumCapacity = _elevatorServices.Max(x => x.GetCapacity());
            if (people > maximumCapacity)
            {
                Console.WriteLine($"We currently have no elevators that take {people} people. We will send more elevators, please wait.");

                var elevatorNeedCount = people / maximumCapacity;
                for (int i = 0; i < elevatorNeedCount; i++)
                {
                    _requestQueue.Enqueue(new Request(floor, maximumCapacity, direction));
                    people -= maximumCapacity;
                }

                // Reuce complexity by placing if statements outside the loop
                if (people % maximumCapacity > 0)
                {
                    // Add the remainder
                    _requestQueue.Enqueue(new Request(floor, people, direction));
                }
            } else
            {
                _requestQueue.Enqueue(new Request(floor, people, direction));
            }
        }

        public void ProcessElevatorRequests()
        {
            while (_requestQueue.Count > 0)
            {
                Request request = _requestQueue.Dequeue();

                IElevatorService elevatorService = GetAvailableElevator(request);

                if (elevatorService != null)
                {
                    elevatorService.ProcessRequest(request);
                }
                else
                {
                    Console.WriteLine("No available elevators to handle the request.");
                }
            }
        }

        public ElevatorDirection GetAvailableDirections(int floor)
        {
            var lowestFloor = _floors.MinBy(x => x.Number)?.Number ?? floor;
            var topFloor = _floors.MaxBy(x => x.Number)?.Number ?? floor;

            if (floor == lowestFloor) // At the bottom
            {
                return ElevatorDirection.Up; // Can only go up
            } else if (floor > lowestFloor && floor < topFloor) // Middle
            {
                return ElevatorDirection.Up | ElevatorDirection.Down; // Can go both ways
            } else if (floor == topFloor) // Top
            {
                return ElevatorDirection.Down; // Can only go down
            } else
            {
                throw new InvalidOperationException("Cannot determine direction");
            }
        }

        public IElevatorService? GetAvailableElevator(Request request)
        {
            // Get all elevators. Exclude out of order elevators
            var eligibleElevators = _elevatorServices.Where(x =>
                x.GetElevatorState() != ElevatorState.OutOfOrder &&
                !x.CapacityReached() && x.HasSpaceFor(request.People)).ToList();

            // First, is there a moving elevator, close by, that has enough capacity
            // and is still going towards the people that requested the elevator?
            var moving = eligibleElevators.Where(x =>
                x.GetElevatorState() == ElevatorState.Moving &&
                x.GetElevatorDirection() == request.Direction &&
                (
                    (request.Direction == ElevatorDirection.Up && x.GetCurrentFloor() < request.Floor) ||
                    (request.Direction == ElevatorDirection.Down && x.GetCurrentFloor() > request.Floor)
                ));

            // Secondly, give me the stationary elevators, and then check if one is perhaps
            // closer. It may be that one is ready on the same floor.
            var stationary = eligibleElevators.Where(x => x.GetElevatorState() == ElevatorState.Stationary);

            // Now, determine which elevator we are going to send
            // We need to remember to update the elevator service status
            // Elevator status
            var elevator = moving.Concat(stationary).MinBy(x => x.GetCurrentFloor());

            // If no elevators are close enough to host, then return the best one
            // Closest and most capacity...


            return elevator;
        }

        public void RequestElevator(Request request)
        {
            var elevator = GetAvailableElevator(request);

            if (elevator is not null)
            {
                elevator.MoveToFloor(request.Floor);
            } else
            {
                // If there is no elevators, then we will have to queue one
                AddRequest(request.Floor, request.People, request.Direction);
            }
        }

        /// <summary>
        /// Checks if the elevator is still on track to stop at the floor requested
        /// </summary>
        /// <param name="elevatorService"><see cref="IElevatorService"/></param>
        /// <param name="direction"><see cref="Domain.Enums.ElevatorDirection"/>: Direction requested bu the user (up or down) form a floor</param>
        /// <param name="floor">Floor which the user is currently on</param>
        /// <returns>bool indicating if the elevator is still going to pass the people</returns>
        private bool GoingPastFloor(ref ElevatorService elevatorService, Domain.Enums.ElevatorDirection direction, int floor)
        {
            if (direction == Domain.Enums.ElevatorDirection.Up)
            {
                return elevatorService.GetCurrentFloor() < floor;
            }
            return elevatorService.GetCurrentFloor() > floor;
        }
    }
}