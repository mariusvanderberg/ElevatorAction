using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace ElevatorAction.Application
{
    /// <summary>
    /// This is the implementation for <see cref="IElevatorControlService"/>.
    /// This is the orchastration logic for all elevators and floors in the block
    /// </summary>
    public class ElevatorControlService : IElevatorControlService
    {
        private List<IElevatorService> _elevatorServices = new();
        private List<Floor> _floors = new();
        private Queue<Request> _requestQueue = new();
        private bool initialised = false;
        private readonly IInputManager _inputManager;

        public event EventHandler<RequestEventArgs>? RequestReceived;
        public event EventHandler<RequestEventArgs>? ElevatorArrived;

        public ElevatorControlService(IInputManager inputManager)
        {
            _inputManager = inputManager;
        }

        public void Init(List<IElevatorService> elevatorServices, List<Floor> floors)
        {
            if (initialised)
            {
                Console.WriteLine("Init can only happen once");
                return;
            }
            _elevatorServices = elevatorServices;
            _floors = floors;
            _requestQueue = new Queue<Request>();
            initialised = true;
        }

        protected virtual async Task OnRequestReceivedAsync(RequestEventArgs e)
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
        }

        protected virtual async Task<RequestEventArgs> OnElevatorArrivedAsync(RequestEventArgs e)
        {
            if (ElevatorArrived != null)
            {
                Delegate[] eventHandlers = ElevatorArrived.GetInvocationList();
                List<Task> handlerTasks = new(eventHandlers.Length);

                foreach (EventHandler<RequestEventArgs> handler in eventHandlers)
                {
                    Task task = Task.Run(() => handler.Invoke(this, e));
                    handlerTasks.Add(task);
                }

                await Task.WhenAll(handlerTasks);
            }

            return e;
        }

        /// <summary>
        /// Enqueue takes care of adding the correct queue in terms of
        /// capacity to the back, ensuring that requests are valid and tended to
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="people"></param>
        /// <param name="direction"></param>
        /// <returns>bool indicating success</returns>
        public async Task<bool> EnqueueRequestAsync(int floor, int people, ElevatorDirection direction)
        {
            int maximumCapacity = _elevatorServices.Max(x => x.GetCapacity());
            if (people > maximumCapacity)
            {
                Console.WriteLine($"We currently have no elevators that take {people} people. We will send more elevators, please wait.");

                var elevatorNeedCount = people / maximumCapacity;
                for (int i = 0; i < elevatorNeedCount; i++)
                {
                    _requestQueue.Enqueue(new Request(floor, maximumCapacity, direction));
                    //await OnRequestReceivedAsync(new RequestEventArgs(floor, direction));
                    people -= maximumCapacity;
                }

                // Reuce complexity by placing if statements outside the loop
                if (people % maximumCapacity > 0)
                {
                    // Add the remainder
                    _requestQueue.Enqueue(new Request(floor, people, direction));
                    //await OnRequestReceivedAsync(new RequestEventArgs(floor, direction));
                }

                return true;
            } else
            {
                _requestQueue.Enqueue(new Request(floor, people, direction));
                await OnRequestReceivedAsync(new RequestEventArgs(floor, direction));
                //await ProcessElevatorRequestsAsync(); // This will take the front of the queue until an elevator is available
                return true;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessElevatorRequestsAsync()
        {
            while (_requestQueue.Count > 0) // Hopefully loops through the queue until everyone is happy
            {
                Request request = _requestQueue.Dequeue();

                IElevatorService? elevatorService = GetAvailableElevator(request);

                if (elevatorService != null)
                {
                    await MoveElevatorAsync(elevatorService, request);
                    //return true;
                }
                else
                {
                    //Console.WriteLine("No available elevators to handle the request. Queueing again");
                    await EnqueueRequestAsync(request.Floor, request.People, request.Direction);
                    //await Task.Delay(5000);
                    //return await ProcessElevatorRequestsAsync(); // recursive to ensure people are tended to
                }
            }

            // Default is true, failure results in exception
            return true;
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

        private async Task MoveElevatorAsync(IElevatorService elevatorService, Request request)
        {
            // Create a cancellation token source
            CancellationTokenSource cts = new CancellationTokenSource();

            // This handles the elevator itself, moving and then opening doors
            await elevatorService.ProcessRequestAsync(request, cts.Token);

            // We are now on the floor, people are loaded into the elevator, we need to ask where to
            var evt = await OnElevatorArrivedAsync(new RequestEventArgs(request.Floor, request.Direction));

            int destinationFloor = await Task.Run(() => _inputManager.NumberInput($"Elevator ready and loaded. Which floor would you like to go to?"));

            Task moveTask = elevatorService.MoveToFloor(destinationFloor, request.Direction, cts.Token);

            // Now let's move...
            try
            {
                await moveTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Move operation canceled.");
            }
        }

        public async Task RequestElevatorAsync(Request request)
        {
            var elevatorService = GetAvailableElevator(request);

            if (elevatorService is not null)
            {
                await MoveElevatorAsync(elevatorService, request);

            } else
            {
                // If there are no elevators, then we will have to queue one
                if (await EnqueueRequestAsync(request.Floor, request.People, request.Direction))
                {
                    await Task.Delay(2000);
                    await ProcessElevatorRequestsAsync(); // Take it off the queue
                }
            }
        }

        ///// <summary>
        ///// Checks if the elevator is still on track to stop at the floor requested
        ///// </summary>
        ///// <param name="elevatorService"><see cref="IElevatorService"/></param>
        ///// <param name="direction"><see cref="Domain.Enums.ElevatorDirection"/>: Direction requested bu the user (up or down) form a floor</param>
        ///// <param name="floor">Floor which the user is currently on</param>
        ///// <returns>bool indicating if the elevator is still going to pass the people</returns>
        //private bool GoingPastFloor(ref ElevatorService elevatorService, Domain.Enums.ElevatorDirection direction, int floor)
        //{
        //    if (direction == Domain.Enums.ElevatorDirection.Up)
        //    {
        //        return elevatorService.GetCurrentFloor() < floor;
        //    }
        //    return elevatorService.GetCurrentFloor() > floor;
        //}

        public ElevatorDirection GetAvailableFloors(int floor)
        {
            throw new NotImplementedException();
        }
    }

    public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e) where TEventArgs : EventArgs;
}