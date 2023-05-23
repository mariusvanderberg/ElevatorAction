using ElevatorAction.Domain.Entities;

namespace ElevatorAction.Application;

public static class Constants
{
    public const string InitializedAlready = "Init can only happen once";

    public static class Doors
    {
        public const string Closed = "Doors closed.";
        public const string Closing = "Doors closing...";
        public const string Open = "Doors open.";
        public const int OpenClosedDelay = 1000;
        public const int OpenClosingDelay = 2000;
        public const string Opening = "Doors opening...";
    }

    public static class Messages
    {
        public const string ElevatorMovingFormat = "Elevator moving with {0} people. Current floor: {1}";
    }
    public static class Operation
    {
        public const string Canceled = "Move operation canceled.";
        public const string DirectionError = "Cannot determine direction";
        public const string ElevatorArrived = "{0} has arrived on floor {1}";
        public const string ElevatorMoving = "{0} moving from floor {1} to {2}.";
        public const string ElevatorOnRoute = "Elevator {0} is on the way to floor {1} to pick up {2} people";
        public const string ElevatorReady = "Elevator ready and loaded. Which floor would you like to go to?";
        public const string EmergencyStop = "Emergency stop requested. Stopping elevator.";
        public const string Error = "Please provide a number";
        public const string NoElevators = "We currently have no elevators that take {0} people. We will send more elevators, please wait.";
        public const string Movement = "Elevator going {0}";
    }
}
