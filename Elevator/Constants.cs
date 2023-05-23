﻿namespace ElevatorAction.ConsoleUI;

public static class Constants
{
    public const string OptionsFormat = "{0}) {1} - {2}";

    public static class Input
    {
        public const string Direction = "Would you like to go up or down? 1 for up and 2 for down";
        public const string Number = "Please provide a number";
        public static readonly string[] YesNoOptions = { "Y", "N" };
    }

    public static class Messages
    {
        public const string Error = "Invalid input, please try again.";
        public const string Exit = "Thanks for using Elevator Action! Goodbye.";
        public const string InputPrompt = "Selection: ";
        public const string Intro = "\nHint: Press tab to cycle through options.";
        public const string Separator = ", ";
        public const string ThankYou = "Thank you.";
        public const string Welcome = "Welcome, please select an option below:";
        public const string YesNoAppend = "{0} Enter {1} for yes and {2} for no.";
    }

    public static class Simulator
    {
        public const string About = "Elevator Action was inspired by the old TV Game, elevator action.\nThis is more elevator than action, but hope you enjoy the clean architecture though.";
        public const string ElevatorAdded = "New elevator has been added.";
        public const string ExistingFloor = "Would you like to use one of the existing {0} floor(s) or add a new one?";
        public const string FloorConfigAll = "Would you like to add current floor configuration to all elevators?";
        public const string FloorConfigById = "Would you like to add current floor configuration to elevator {0}?";
        public const string FloorCount = "We have {0} floors. Please provide a floor number to add from the options below:";
        public const string GroundLevel = "Which floor number is the ground floor? i.e which floor is 0?";
        public const string GroundLevelName = "Ground";
        public const string Levels = "In total, including basement levels, how many floors does these elevators serve?";
        public const string PeopleCount = "Thank you. How many people requires the elevator?";
        public const string RequestElevator = "To request an elevator, please enter your curent floor number: ";
        public const string RequestReceived = "New elevator request received for floor {0}, direction {1}";
        public const string Reset = "Values have been reset.";
        public const string Running = "Running simulator";
        public const string Selection = "You have {0} floors from {1} to {2}";
        public const string Welcome = "Welcome to Elevator action! Pres ctrl + c to close the application.";
    }
}
