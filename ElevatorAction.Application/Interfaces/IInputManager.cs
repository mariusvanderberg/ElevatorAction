using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Application.Interfaces
{
    public interface IInputManager
    {
        /// <summary>
        /// Prompts the user to input a number between two numbers
        /// provided
        /// </summary>
        /// <param name="message">Message to display to output</param>
        /// <param name="lowest">Lowest acceptable number</param>
        /// <param name="highest">Greatest acceptable number</param>
        /// <returns>User provided number</returns>
        int BetweenNumberInput(string message, int lowest, int highest);

        /// <summary>
        /// Determine which direction in <see cref="ElevatorDirection"/>
        /// </summary>
        /// <param name="message">Custom message to ask for direction</param>
        /// <returns><see cref="ElevatorDirection"/></returns>
        ElevatorDirection DirectionInput(string message);

        /// <summary>
        /// Prompts the user to input a number greater than the number
        /// provided
        /// </summary>
        /// <param name="message">Message to display to output</param>
        /// <param name="lowest">Lowest acceptable number</param>
        /// <returns>User provided number</returns>
        int GreaterThanNumberInput(string message, int lowest = 0);

        /// <summary>
        /// Prompts the user to input a number
        /// </summary>
        /// <param name="message">Message to display to output</param>
        /// <returns>User provided number</returns>
        int NumberInput(string message);
        /// <summary>
        /// Re-usable simple yes / no prompt
        /// </summary>
        /// <param name="message">Message displayed to the user</param>
        /// <returns><see cref="bool"/> Indication of answer</returns>
        bool YesNoInput(string message, bool appendOptions = true);
    }
}
