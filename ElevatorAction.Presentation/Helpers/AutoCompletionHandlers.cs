using ElevatorAction.Domain.Enums;

namespace ElevatorAction.ConsoleUI.Helpers;

/// <summary>
/// Auto-complete handler for elevators - provides a richer experience for the console
/// </summary>
internal class ConsoleApplicationAutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ', '.', '/', '|' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Auto complete known values
            return Enum.GetNames(typeof(ApplicationOptions)).Where(c => c.StartsWith(text)).ToArray();
        }
        if (text.StartsWith(""))
            return Enum.GetNames(typeof(ApplicationOptions)); // All
        return Array.Empty<string>();
    }
}

/// <summary>
/// Auto-complete handler for simulator - provides a richer experience for the console
/// </summary>
internal class SimulatorAutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ', '.', '/', '|' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Auto complete known values
            return Enum.GetNames(typeof(SimulatorOptions)).Where(c => c.StartsWith(text)).ToArray();
        }
        if (text.StartsWith(""))
            return Enum.GetNames(typeof(SimulatorOptions)); // All
        return Array.Empty<string>();
    }
}