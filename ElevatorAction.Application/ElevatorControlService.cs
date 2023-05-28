using ElevatorAction.Application.Common;
using ElevatorAction.Application.Interfaces;
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
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IInputManager _inputManager;
        private List<IElevatorService> _elevatorServices = new();
        private List<Floor> _floors = new();
        private bool _isInitialised = false;
        private Queue<Request> _requestQueue = new();
        public ElevatorControlService(IAsyncDelayer asyncDelayer, IInputManager inputManager)
        {
            _asyncDelayer = asyncDelayer;
            _inputManager = inputManager;
        }

        public event EventHandler<RequestEventArgs>? RequestReceived;

        /// <inheritdoc/>
        public ElevatorDirection GetAvailableDirections(int floor)
        {
            var lowestFloor = _floors.MinBy(x => x.Number)?.Number ?? floor;
            var topFloor = _floors.MaxBy(x => x.Number)?.Number ?? floor;

            if (floor == lowestFloor) // At the bottom
            {
                return ElevatorDirection.Up; // Can only go up
            }
            else if (floor > lowestFloor && floor < topFloor) // Middle
            {
                return ElevatorDirection.Up | ElevatorDirection.Down; // Can go both ways
            }
            else if (floor == topFloor) // Top
            {
                return ElevatorDirection.Down; // Can only go down
            }
            else
            {
                throw new InvalidOperationException(Constants.Operation.DirectionError);
            }
        }

        /// <summary>
        /// Initializes the controller with floors and elevator services
        /// </summary>
        /// <param name="elevatorServices"><see cref="IElevatorService"/>: List of services</param>
        /// <param name="floors"><see cref="Floor"/>: List of floors</param>
        public void Initialize(List<IElevatorService> elevatorServices, List<Floor> floors)
        {
            if (_isInitialised)
            {
                Console.WriteLine(Constants.InitializedAlready);
                return;
            }
            _elevatorServices = elevatorServices;
            _floors = floors;
            _requestQueue = new Queue<Request>();
            _isInitialised = true;
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessElevatorRequestsAsync()
        {
            while (_requestQueue.Count > 0)
            {
                Request request = _requestQueue.Dequeue();

                IElevatorService? elevatorService = GetAvailableElevator(request);

                if (elevatorService != null)
                {
                    await MoveElevatorAsync(elevatorService, request);
                }
                else
                {
                    await EnqueueRequestAsync(request.Floor, request.People, request.Direction);
                }
            }

            // Default is true, failure results in exception
            return true;
        }

        /// <inheritdoc/>
        public async Task RequestElevatorAsync(Request request)
        {
            var elevatorService = GetAvailableElevator(request);

            if (elevatorService is not null)
            {
                await MoveElevatorAsync(elevatorService, request);
            }
            else
            {
                // If there are no elevators, then we will have to queue one
                if (await EnqueueRequestAsync(request.Floor, request.People, request.Direction))
                {
                    await _asyncDelayer.Delay(2000, default);
                    await ProcessElevatorRequestsAsync(); // Take it off the queue
                }
            }
        }

        /// <summary>
        /// Invoke listeners when a request is received
        /// </summary>
        /// <param name="e"><see cref="EventArgs"/> <seealso cref="RequestEventArgs"/></param>
        /// <returns>bool indicating success</returns>
        protected virtual async Task<bool> OnRequestReceivedAsync(RequestEventArgs e)
        {
            if (RequestReceived != null)
            {
                Delegate[] eventHandlers = RequestReceived.GetInvocationList();
                List<Task> handlerTasks = new(eventHandlers.Length);

                foreach (EventHandler<RequestEventArgs> handler in eventHandlers)
                {
                    Task task = Task.Run(() => handler.Invoke(this, e));
                    handlerTasks.Add(task);
                }

                await Task.WhenAll(handlerTasks);
            }

            return true;
        }

        /// <summary>
        /// Enqueue takes care of adding the correct queue in terms of
        /// capacity to the back, ensuring that requests are valid and tended to
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="people"></param>
        /// <param name="direction"></param>
        /// <returns>bool indicating success</returns>
        private async Task<bool> EnqueueRequestAsync(int floor, int people, ElevatorDirection direction)
        {
            int maximumCapacity = _elevatorServices.Max(x => x.GetCapacity());
            if (people > maximumCapacity)
            {
                Console.WriteLine(string.Format(Constants.Operation.NoElevators, people));

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

                return true;
            }
            else
            {
                _requestQueue.Enqueue(new Request(floor, people, direction));
                await OnRequestReceivedAsync(new RequestEventArgs(floor, direction));
                return true;
            }
        }

        /// <summary>
        /// This logic uses closest first to get the next available elevator
        /// </summary>
        /// <param name="request"><see cref="Request"/></param>
        /// <returns><see cref="IElevatorService"/>: Next available service</returns>
        private IElevatorService? GetAvailableElevator(Request request)
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

        /// <summary>
        /// Used to move an elevator after request
        /// </summary>
        /// <param name="elevatorService"><see cref="IElevatorService"/></param>
        /// <param name="request"><see cref="Request"/></param>
        /// <returns>bool indicating success</returns>
        private async Task<bool> MoveElevatorAsync(IElevatorService elevatorService, Request request)
        {
            // Create a cancellation token source
            var cts = new CancellationTokenSource();

            // This handles the elevator itself, moving and then opening doors
            await elevatorService.ProcessRequestAsync(request, cts.Token);

            int destinationFloor = await Task.Run(() => _inputManager.NumberInput($"{Constants.Operation.ElevatorReady} "));

            Task moveTask = elevatorService.MoveToFloorAsync(destinationFloor, request.Direction, cts.Token);

            // Now let's move...
            try
            {
                await moveTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(Constants.Operation.Canceled);
                return false;
            }

            return true;
        }
    }
}