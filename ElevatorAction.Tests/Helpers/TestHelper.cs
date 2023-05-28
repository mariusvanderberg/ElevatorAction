using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests.Helpers;

internal static class TestHelper
{
    public static void AddFloorsToElevator(int groundFloor, int floorCount, Elevator elevator) => FloorHelper.Iterate(groundFloor, floorCount, i => elevator.AddFloor(new Floor
    {
        FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
        Name = i.ToString(),
        Number = i
    }));
}
