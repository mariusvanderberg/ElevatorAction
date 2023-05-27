namespace ElevatorAction.ConsoleUI.Interfaces
{
    public interface ISimulator
    {
        /// <summary>
        /// Runs the application after everything has been configured
        /// </summary>
        /// <returns>bool indicating success</returns>
        Task<bool> Run();

        /// <summary>
        /// Starts the simulator, by allowing configuration first. This is
        /// the entry point for the application. Use <see cref="Run"/> to run
        /// the simulator
        /// </summary>
        /// <returns>bool indicating success</returns>
        Task<bool> Start();
    }
}
