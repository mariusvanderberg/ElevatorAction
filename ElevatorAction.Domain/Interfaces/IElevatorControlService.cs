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
        void RequestElevator(Request request);

        /// <summary>
        /// Based on the current floor, hich directions can the elevator go
        /// </summary>
        /// <returns>Directions that the elevator can go</returns>
        ElevatorDirection GetAvailableDirections(int floor);
    }
}
