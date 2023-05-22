using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Interfaces
{
    /// <summary>
    /// Service to control elevator operations
    /// </summary>
    public interface IElevatorService
    {
        /// <summary>
        /// Process an elevator request
        /// </summary>
        /// <param name="request"><see cref="Request"/></param>
        public void ProcessRequest(Request request);

        /// <summary>
        /// Informs whether the elevator is full or not
        /// </summary>
        /// <returns>bool indicating if the elevator is full or not</returns>
        bool CapacityReached();

        /// <summary>
        /// Returns the people capacity of the current elevator
        /// </summary>
        /// <returns>Elevator capacity</returns>
        int GetCapacity();
        /// <summary>
        /// Gets the floor that an elevator is on
        /// </summary>
        /// <returns></returns>
        int GetCurrentFloor();

        /// <summary>
        /// Returns the current direction of the elevator if it is moving.
        /// </summary>
        /// <remarks>This will return null if the elevator is not moving.
        /// </remarks>
        /// <returns><see cref="ElevatorDirection"/></returns>
        ElevatorDirection? GetElevatorDirection();

        /// <summary>
        /// Elevator is stationary, moving, out of order, etc.
        /// </summary>
        /// <returns><see cref="ElevatorState"/></returns>
        ElevatorState GetElevatorState();

        /// <summary>
        /// Get the current number of people in the elevator
        /// </summary>
        /// <returns>Integer representing people</returns>
        int GetNumberOfPeople();

        /// <summary>
        /// Will be able to tell if there's space for the people requesting a
        /// lift
        /// </summary>
        /// <param name="people">Number of people that needs the elevator</param>
        /// <returns>bool indicaing of the elevator can host the people</returns>
        bool HasSpaceFor(int people);

        /// <summary>
        /// Stops the elevator immediately
        /// </summary>
        void MakeEmergencyStop();
        /// <summary>
        /// Move elevator to specific floor
        /// </summary>
        /// <param name="floorNumber">Floor to move to</param>
        void MoveToFloor(int floorNumber);
    }
}
