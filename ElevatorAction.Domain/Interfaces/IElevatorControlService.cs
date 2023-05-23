﻿using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Interfaces
{
    public interface IElevatorControlService
    {
        public event EventHandler<RequestEventArgs> RequestReceived;

        /// <summary>
        /// Based on the current floor, hich directions can the elevator go
        /// </summary>
        /// <param name="floor">Current floor</param>
        /// <returns>Directions that the elevator can go</returns>
        ElevatorDirection GetAvailableDirections(int floor);

        void Init(List<IElevatorService> elevatorServices, List<Floor> floors);

        /// <summary>
        /// This needs to be hooked up to a poller
        /// </summary>
        /// <returns>bool indicating success</returns>
        Task<bool> ProcessElevatorRequestsAsync();

        /// <summary>
        /// Someone pressed a button, so now we need to process the request
        /// </summary>
        /// <param name="request"><see cref="Request"/></param>
        Task RequestElevatorAsync(Request request);
    }
}
