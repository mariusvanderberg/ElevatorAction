using ElevatorAction.Application;
using ElevatorAction.Application.Interfaces;
using ElevatorAction.Domain.Common;
using ElevatorAction.Domain.Enums;
using Constants = ElevatorAction.Application.Constants;

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
            Console.Write(message);

            string? input = Console.ReadLine();

            while (!int.TryParse(input, out val))
            {
                input = ReadLine.Read(message);
            }

            return val;
        }

        /// <inheritdoc/>
        public int GreaterThanNumberInput(string message, int lowest = 0)
        {
            int val = NumberInput(string.Format(Constants.Formatting.GreaterThanEqualsFormat, message, lowest));

            while (val < lowest)
            {
                Console.WriteLine(Constants.Messages.Error);
                val = NumberInput(string.Format(Constants.Formatting.GreaterThanEqualsFormat, message, lowest));
            }

            return val;
        }

        /// <inheritdoc/>
        public int BetweenNumberInput(string message, int lowest = 0, int highest = 0)
        {
            int val = NumberInput(string.Format(Constants.Formatting.BetweenFormat, message, lowest, highest));

            while (val < lowest || val > highest)
            {
                Console.WriteLine(Constants.Messages.Error);
                val = NumberInput(string.Format(Constants.Formatting.BetweenFormat, message, lowest, highest));
            }

            return val;
        }

        /// <inheritdoc/>
        public bool YesNoInput(string message, bool appendOptions = true)
        {
            if (appendOptions)
                message = string.Format(Constants.Messages.YesNoAppend, message, Constants.Input.YesNoOptions[0], Constants.Input.YesNoOptions[1]);

            var input = ReadLine.Read(message, Constants.Input.YesNoOptions[0]);

            while (!Constants.Input.YesNoOptions.Contains(input))
            {
                input = ReadLine.Read(message, Constants.Input.YesNoOptions[0]);
            }

            return input == Constants.Input.YesNoOptions[0];
        }
    }
}
