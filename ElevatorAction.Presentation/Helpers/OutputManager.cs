using ElevatorAction.Application.Interfaces;

namespace ElevatorAction.ConsoleUI.Helpers
{
    public class OutputManager : IOutputManager
    {
        /// <inheritdoc/>
        public void Clear()
        {
            Console.Clear();
        }
    }
}
