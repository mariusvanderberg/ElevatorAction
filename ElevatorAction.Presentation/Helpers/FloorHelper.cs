namespace ElevatorAction.ConsoleUI.Helpers;

public static class FloorHelper
{
    /// <summary>
    /// Takes in the ground floor, floour count, and works out the levels
    /// by correct iteration. It provides a <see cref="Action"/> to use as
    /// desired
    /// </summary>
    /// <param name="ground">Ground floor</param>
    /// <param name="count">Floor count, i.e. 10 floors</param>
    /// <param name="action">Action to follow after each iteration.
    /// Wrap this in a delagte to add to the iteration</param>
    public static void Iterate(int ground, int count, Action<int> action)
    {
        for (var i = ground * -1 + 1; i < count - ground + 1; i++)
        {
            action(i);
        }
    }
}
