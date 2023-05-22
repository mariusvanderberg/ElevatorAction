namespace ElevatorAction.ConsoleUI;

public static class Constants
{
    public static class Messages
    {
        public const string Error = "Invalid input, please try again.";
        public const string Welcome = "Welcome, please select an option below:";
        public const string Exit = "Thanks for using Elevator Action! Goodbye.";
        public const string InputPrompt = "Selection: ";
        public const string Intro = "\nHint: Press tab to cycle through options.";
    }
    public const string OptionsFormat = "{0}) {1} - {2}";
    public static class Input
    {
        public const string Number = "Please provide a number";
        public const string Direction = "Would you like to go up or down? 1 for up and 2 for down";
    }
}
