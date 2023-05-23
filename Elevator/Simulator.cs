using ElevatorAction.Application;
using ElevatorAction.ConsoleUI.Extensions;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ElevatorAction.ConsoleUI
{
    internal class Simulator : ISimulator
    {
        private readonly IElevatorControlService _controller;
        private readonly IInputManager _inputManager;
        private readonly int _maximumCapacity;
        private readonly IOutputManager _outputManager;
        List<Button> buttons = new();
        List<Elevator> elevators = new(); // Used for storing elevators
        List<Floor> floors = new(); // Building block floor configuration
                                    // Buttons (default and extended) per elevator and floor
        bool quickStart = false;

        public Simulator(IInputManager inputManager, IOutputManager outputManager, IElevatorControlService controller, IConfiguration configuration)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _outputManager = outputManager ?? throw new ArgumentNullException(nameof(outputManager));
            _controller = controller;
            _maximumCapacity = configuration.GetValue<int>("Elevator:MaximumCapacity");
        }

        /// <summary>
        /// This will simulate the elevator action
        /// </summary>
        public async Task Run()
        {
            List<IElevatorService> services = new List<IElevatorService>();
            // Start the application
            foreach (Elevator elevator in elevators)
            {
                services.Add(new ElevatorService(elevator));
            }

            // Instantiate main elevator controller service
            _controller.Init(services, floors);

            _controller.RequestReceived += ControlService_RequestReceived; // Everytime a request is received

            _outputManager.Clear();
            Console.WriteLine("Welcome to Elevator action! Pres ctrl + c to close the application.");

            await SimulatePerson();

            //_outputManager.Clear();
        }

        /// <summary>
        /// This simulates a person at a controller needing assistance
        /// </summary>
        public async Task SimulatePerson()
        {
            while (true)
            {
                // Print out floor config
                Console.WriteLine($"You have {floors.Count} floors from {floors.MinBy(x => x.Number)?.Number} to {floors.MaxBy(x => x.Number)?.Number}");

                int currentFloor = quickStart ? 2 : _inputManager.NumberInput("To request an elevator, please enter your curent floor number: ");

                int people = quickStart ? 7 : _inputManager.NumberInput("Thank you. How many people requires the elevator?");

                // Just by this information, we should be able to tell which directions are available
                var directions = _controller.GetAvailableDirections(currentFloor);

                // Can go up or down, so choose
                if (directions.HasFlag(ElevatorDirection.Up | ElevatorDirection.Down))
                {
                    directions = quickStart ? ElevatorDirection.Up : _inputManager.DirectionInput(Constants.Input.Direction);
                }

                await _controller.RequestElevatorAsync(new Request(currentFloor, people, directions));
            }
        }

        /// <summary>
        /// Overload to simulate a request when the input is known. Can be used from another user interface
        /// </summary>
        /// <remarks>Pre-validation required on direction</remarks>
        /// <param name="controller"><see cref="IElevatorControlService"/></param>
        /// <param name="request"><see cref="Request"/></param>
        public void SimulatePerson(Request request)
        {
            while (true)
            {
                // Print out floor config
                Console.WriteLine($"You have {floors.Count} floors from {floors.MinBy(x => x.Number)?.Number} to {floors.MaxBy(x => x.Number)?.Number}");

                _controller.RequestElevatorAsync(request);
            }
        }

        public async Task Start()
        {
            Console.OutputEncoding = Encoding.UTF8; // make sure we can print out UTF8 characters
            Console.WriteLine(Constants.Messages.Welcome);

            PrintOptions();

            ReadLine.HistoryEnabled = true;
            ReadLine.AutoCompletionHandler = new ConsoleApplicationAutoCompletionHandler();

            ApplicationOptions myOpt;

            // Keep reading input until a valid selection is made
            do
            {
                if (Enum.TryParse(ReadLine.Read(Constants.Messages.InputPrompt, ApplicationOptions.Run.ToString()), ignoreCase: true, out myOpt))
                {
                    switch (myOpt)
                    {
                        case ApplicationOptions.Run:
                            // We will attempt to Run the simulator
                            break;
                        case ApplicationOptions.Add:
                            AddElevator();
                            break;
                        case ApplicationOptions.Floors:
                            AddFloors();
                            break;
                        case ApplicationOptions.About:
                            Console.WriteLine("Elevator Action was inspired by the old TV Game, elevator action.");
                            Console.WriteLine("This is more elevator than action, but hope you enjoy the clean architecture though.");
                            Console.WriteLine();
                            PrintOptions();
                            break;
                        case ApplicationOptions.Reset:
                            elevators.Clear();
                            floors.Clear();
                            buttons.Clear();
                            RefreshScreen();
                            Console.WriteLine("Values have been reset.");
                            break;
                        case ApplicationOptions.Exit:
                            // Exit application
                            break;
                        default:
                            Console.WriteLine(Constants.Messages.Error);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine(Constants.Messages.Error);
                }
            } while (myOpt != ApplicationOptions.Exit && myOpt != ApplicationOptions.Run);


            if (myOpt == ApplicationOptions.Exit)
            {
                _outputManager.Clear();
                Console.WriteLine(Constants.Messages.Exit);
            }
            else
            {
                ReadLine.AutoCompletionHandler = new SimulatorAutoCompletionHandler();
                Console.WriteLine("Running simulator");

                TempSetup(); // TODO: REMOVE!!

                if (!floors.Any())
                {
                    AddFloors();
                }
                if (!elevators.Any())
                {
                    elevators.Add(new Elevator(_maximumCapacity));
                }

                await Run();
            }
        }

        private static void ControlService_RequestReceived(object? sender, RequestEventArgs e)
        {
            Console.WriteLine($"New elevator request received for floor {e.Floor}, direction {e.Direction}");
        }

        private void AddAllFloorsToElevator(Elevator elevator)
        {
            for (int i = 0; i < floors.Count; i++)
            {
                if (!elevator.Floors.Any(x => x.Id == floors[i].Id))
                    elevator.Floors.Add(floors[i]);
            }
        }

        private void AddElevator()
        {
            var newElevator = new Elevator(_maximumCapacity);
            elevators.Add(newElevator);
            Console.WriteLine("New elevator has been added.");

            AddFloorsToElevator(ref newElevator);
        }

        private void AddFloor(int floorNumber)
        {
            Floor floor = new()
            {
                FriendlyName = floorNumber == 0 ? "Ground" : floorNumber.ToString(),
                Name = floorNumber.ToString(),
                Number = floorNumber
            };

            floors.Add(floor);
        }

        private void AddFloors()
        {
            int floorCount, groundFloor;

            // Amount of floors
            floorCount = _inputManager.NumberInput("In total, including basement levels, how many floors does these elevators serve?");
            Console.Write("Thank you. ");

            // Which level is the ground floor?
            groundFloor = _inputManager.NumberInput("Which floor number is the ground floor? i.e which floor is 0?");

            for (var i = groundFloor * -1 + 1; i < floorCount - groundFloor + 1; i++)
            {
                AddFloor(i);
            }

            // If elevators have been added, suggest to apply floor changes
            if (elevators.Count > 0)
            {
                if (_inputManager.YesNoInput($"Would you like to add current floor configuration to all elevators?"))
                {
                    for (int i = 0; i < elevators.Count; i++)
                    {
                        AddAllFloorsToElevator(elevators[i]);
                    }
                }
            }
        }

        private void AddFloorsToElevator(ref Elevator elevator)
        {
            if (floors.Count > 0)
            {
                if (_inputManager.YesNoInput($"Would you like to add current floor configuration to elevator {elevator.Id}?"))
                {
                    AddAllFloorsToElevator(elevator);
                }
                else
                {
                    // Prompt if they would like to use existing floors or add more
                    if (_inputManager.YesNoInput($"Would you like to use one of the existing {floors.Count} floor(s) or add a new one?"))
                    {
                        // Which floors then?
                        Console.WriteLine($"We have {floors.Count} floors. Please provide a floor number to add from the options below:");
                        for (int i = 0; i < floors.Count; i++)
                        {
                            if (i > 0)
                            {
                                Console.Write(", ");
                            }
                            Console.Write(floors[i].Number);
                        }
                    }
                    else
                    {
                        AddFloor(floors.Count + 1);
                    }
                }
            }
            else
            {
                //// Amount of floors
                //do
                //{
                //    Console.WriteLine("Would you like to add the same floors to this elevator?");
                //}
                //while (!int.TryParse(Console.ReadLine(), out floorCount));
            }
        }

        private void PrintOptions()
        {
            // Application options written to output
            foreach (ApplicationOptions opt in Enum.GetValues(typeof(ApplicationOptions)))
            {
                Console.WriteLine(string.Format(Constants.OptionsFormat, (int)opt, opt, opt.GetEnumDescription()));
            }

            Console.WriteLine(Constants.Messages.Intro);
        }

        /// <summary>
        /// This will clear the console and print options again
        /// </summary>
        private void RefreshScreen()
        {
            _outputManager.Clear();
            PrintOptions();
        }

        void TempSetup()
        {
            for (int i = 0; i < 3; i++)
            {
                AddElevator();
            }
            for (int i = 0; i < 10; i++)
            {
                AddFloor(i);
            }
            foreach(var elevator in elevators)
            {
                AddAllFloorsToElevator(elevator);
            }
        }
    }
}
