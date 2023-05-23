namespace ElevatorAction.ConsoleUI.Interfaces
{
    public interface ISimulator
    {
        /// <summary>
        /// Runs the application after everything has been configured
        /// </summary>
        Task Run();

        /// <summary>
        /// Starts the simulator, by allowing configuration first
        /// </summary>
        Task Start();
    }
}
