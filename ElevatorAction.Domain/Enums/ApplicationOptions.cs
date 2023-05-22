using System.ComponentModel;

namespace ElevatorAction.Domain.Enums;

/// <summary>
/// Runtime options to get the application started
/// </summary>
public enum ApplicationOptions
{
    [Description("Run simulator")]
    Run = 1,
    [Description("Add Elevators")]
    Add = 2,
    [Description("Add Floors")]
    Floors = 3,
    [Description("About Elevator Action")]
    About = 4,
    [Description("Reset elevator configuration")]
    Reset = 5,
    [Description("Exit")]
    Exit = 6
}
