using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Interfaces
{
    public interface IElevatorControlService
    {
        /// <summary>
        /// Someone pressed a button, so now we need to process the request
        /// </summary>
        /// <param name="request"><see cref="Request"/></param>
        Task RequestElevatorAsync(Request request);

        void Init(List<IElevatorService> elevatorServices, List<Floor> floors);

        /// <summary>
        /// Based on the current floor, hich directions can the elevator go
        /// </summary>
        /// <param name="floor">Current floor</param>
        /// <returns>Directions that the elevator can go</returns>
        ElevatorDirection GetAvailableDirections(int floor);

        /// <summary>
        /// Floors that the elevator can visit
        /// </summary>
        /// <param name="floor">Current floor</param>
        /// <returns>A list of available floors</returns>
        ElevatorDirection GetAvailableFloors(int floor);

        /// <summary>
        /// This needs to be hooked up to a poller
        /// </summary>
        /// <returns>bool indicating success</returns>
        Task <bool> ProcessElevatorRequestsAsync();

        public event EventHandler<RequestEventArgs> RequestReceived;

        public event EventHandler<RequestEventArgs> ElevatorArrived;
    }
}
