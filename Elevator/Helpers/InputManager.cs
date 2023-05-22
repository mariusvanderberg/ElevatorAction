using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Enums;

namespace ElevatorAction.ConsoleUI.Helpers
{
    public class InputManager : IInputManager
    {
        /// <inheritdoc/>
        public ElevatorDirection DirectionInput(string message = Constants.Input.Direction)
        {
            ElevatorDirection myOpt;

            // Keep reading input until a valid selection is made
            while (!Enum.TryParse(ReadLine.Read(message, ElevatorDirection.Up.ToString()), ignoreCase: true, out myOpt))
            {
                Console.WriteLine(Constants.Messages.Error);
            }

            return myOpt;
        }

        /// <inheritdoc/>
        public int NumberInput(string message)
        {
            int val;
            var input = ReadLine.Read(message);

            while (!int.TryParse(input, out val))
            {
                input = ReadLine.Read(message);
            }

            return val;
        }

        /// <inheritdoc/>
        public bool YesNoInput(string message, bool appendOptions = true)
        {
            string[] options = new string[2] { "Y", "N" };

            // TODO: Add to constants as format
            if (appendOptions)
                message = $"{message} Enter {options[0]} for yes and {options[1]} for no.";

            var input = ReadLine.Read(message, options[0]);

            while (!options.Contains(input))
            {
                input = ReadLine.Read(message, options[0]);
            }

            return input == options[0];
        }
    }
}
