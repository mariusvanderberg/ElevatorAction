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
        private List<Elevator> elevators = new(); // Used for storing elevators
        private List<Floor> floors = new(); // Building block floor configuration
        bool quickStart = false;

        public Simulator(IInputManager inputManager, IOutputManager outputManager, IElevatorControlService controller, IConfiguration configuration)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _outputManager = outputManager ?? throw new ArgumentNullException(nameof(outputManager));
            _controller = controller;
            _maximumCapacity = configuration.GetValue<int>("Elevator:MaximumCapacity");
        }

        /// <inheritdoc/>
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
            Console.WriteLine(Constants.Simulator.Welcome);

            await SimulatePerson();
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
                            Console.WriteLine(Constants.Simulator.About);
                            Console.WriteLine();
                            PrintOptions();
                            break;
                        case ApplicationOptions.Reset:
                            elevators.Clear();
                            floors.Clear();
                            RefreshScreen();
                            Console.WriteLine(Constants.Simulator.Reset);
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
                Console.WriteLine(Constants.Simulator.Running);

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
            Console.WriteLine(string.Format(Constants.Simulator.RequestReceived, e.Floor, e.Direction));
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
            Console.WriteLine(Constants.Simulator.ElevatorAdded);

            AddFloorsToElevator(ref newElevator);
        }

        private void AddFloor(int floorNumber)
        {
            Floor floor = new()
            {
                FriendlyName = floorNumber == 0 ? Constants.Simulator.GroundLevelName : floorNumber.ToString(),
                Name = floorNumber.ToString(),
                Number = floorNumber
            };

            floors.Add(floor);
        }

        private void AddFloors()
        {
            int floorCount, groundFloor;

            // Amount of floors
            floorCount = _inputManager.NumberInput(Constants.Simulator.Levels);
            Console.Write($"{Constants.Messages.ThankYou} ");

            // Which level is the ground floor?
            groundFloor = _inputManager.NumberInput(Constants.Simulator.GroundLevel);

            for (var i = groundFloor * -1 + 1; i < floorCount - groundFloor + 1; i++)
            {
                AddFloor(i);
            }

            // If elevators have been added, suggest to apply floor changes
            if (elevators.Count > 0)
            {
                if (_inputManager.YesNoInput(Constants.Simulator.FloorConfigAll))
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
                if (_inputManager.YesNoInput(string.Format(Constants.Simulator.FloorConfigById, elevator.Id)))
                {
                    AddAllFloorsToElevator(elevator);
                }
                else
                {
                    // Prompt if they would like to use existing floors or add more
                    if (_inputManager.YesNoInput(string.Format(Constants.Simulator.ExistingFloor, floors.Count)))
                    {
                        // Which floors then?
                        Console.WriteLine(string.Format(Constants.Simulator.FloorCount, floors.Count));
                        for (int i = 0; i < floors.Count; i++)
                        {
                            if (i > 0)
                            {
                                Console.Write(Constants.Messages.Separator);
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
                // TODO: Add this logic
                //// Amount of floors
                //do
                //{
                //    Console.WriteLine("Would you like to add the same floors to this elevator?");
                //}
                //while (!int.TryParse(Console.ReadLine(), out floorCount));
            }
        }

        /// <summary>
        /// Writes application options to output
        /// </summary>
        private void PrintOptions()
        {
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

        /// <summary>
        /// This simulates a person at a controller needing assistance
        /// </summary>
        private async Task SimulatePerson()
        {
            while (true)
            {
                // Print out floor config
                Console.WriteLine(string.Format(Constants.Simulator.Selection, floors.Count, floors.MinBy(x => x.Number)?.Number, floors.MaxBy(x => x.Number)?.Number));

                int currentFloor = quickStart ? 2 : _inputManager.NumberInput(Constants.Simulator.RequestElevator);

                int people = quickStart ? 7 : _inputManager.NumberInput(Constants.Simulator.PeopleCount);

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
        /// Easy setup. NB!! REMOVE before prod
        /// </summary>
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
