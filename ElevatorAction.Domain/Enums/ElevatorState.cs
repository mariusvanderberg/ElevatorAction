namespace ElevatorAction.Domain.Enums;

public enum ElevatorState
{
    Stationary = 1, // Just idle
    Moving = 2, // On the move
    OutOfOrder = 3, // Broken
    Loading = 4 // Loading people off or on, so not stationary
}