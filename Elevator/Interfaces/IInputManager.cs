using ElevatorAction.Domain.Enums;

namespace ElevatorAction.ConsoleUI.Interfaces
{
    public interface IInputManager
    {
        /// <summary>
        /// Re-usable simple yes / no prompt
        /// </summary>
        /// <param name="message">Message displayed to the user</param>
        /// <returns><see cref="bool"/> Indication of answer</returns>
        bool YesNoInput(string message, bool appendOptions = true);

        /// <summary>
        /// Prompts the user to input a number
        /// </summary>
        /// <returns></returns>
        int NumberInput(string message);

        /// <summary>
        /// Determine which direction in <see cref="ElevatorDirection"/>
        /// </summary>
        /// <param name="message">Custom message to ask for direction</param>
        /// <returns><see cref="ElevatorDirection"/></returns>
        ElevatorDirection DirectionInput(string message);
    }
}
