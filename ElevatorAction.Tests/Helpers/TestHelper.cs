using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests.Helpers;

/// <summary>
/// Any help necessary to enrich test experience should
/// be defined here
/// </summary>
internal static class TestHelper
{
    /// <summary>
    /// Uses <see cref="FloorHelper"/> to add floors to elevator
    /// </summary>
    /// <param name="groundFloor">Specified ground floor</param>
    /// <param name="floorCount">Specified floor count</param>
    /// <param name="elevator">Elevator to add new floors to</param>
    public static void AddFloorsToElevator(int groundFloor, int floorCount, Elevator elevator) => FloorHelper.Iterate(groundFloor, floorCount, i => elevator.AddFloor(new Floor
    {
        FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
        Name = i.ToString(),
        Number = i
    }));
}
