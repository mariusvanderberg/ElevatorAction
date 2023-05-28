using ElevatorAction.Application;
using ElevatorAction.Application.Common;
using ElevatorAction.Application.Interfaces;
using ElevatorAction.ConsoleUI.Extensions;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Text;
using Constants = ElevatorAction.Application.Constants;

namespace ElevatorAction.ConsoleUI
{
    public class Simulator : ISimulator
    {
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IElevatorControlService _controller;
        private readonly IInputManager _inputManager;
        private readonly int _maximumCapacity;
        private readonly IOutputManager _outputManager;
        private readonly List<Floor> floors = new(); // Building block floor configuration
        private bool _isRunning;

        public Simulator(IInputManager inputManager, IOutputManager outputManager, IElevatorControlService controller, IConfiguration configuration, IAsyncDelayer asyncDelayer)
        {
            _inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
            _outputManager = outputManager ?? throw new ArgumentNullException(nameof(outputManager));
            _controller = controller;
            _maximumCapacity = configuration.GetValue<int>("Elevator:MaximumCapacity");
            _asyncDelayer = asyncDelayer;
        }

        public List<Elevator> Elevators { get; set; } = new();
        /// <inheritdoc/>
        public async Task<bool> Run()
        {
            // Validate elevators
            if (Elevators.Count == 0)
            {
                _isRunning = false;
                return false;
            }
            _isRunning = true;
            List<IElevatorService> services = new();

            // Start the application
            foreach (Elevator elevator in Elevators)
            {
                services.Add(new ElevatorService(elevator, _asyncDelayer));
            }

            // Instantiate main elevator controller service
            _controller.Initialize(services, floors);

            _controller.RequestReceived += ControlService_RequestReceived; // Everytime a request is received

            _outputManager.Clear();
            Console.WriteLine(Constants.Simulator.Welcome);

            await SimulatePerson();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> Start()
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
                            if (!floors.Any())
                            {
                                AddFloors();
                            }
                            if (!Elevators.Any())
                            {
                                AddElevator();
                            }
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
                            Elevators.Clear();
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
                // Run application was chosen
                ReadLine.AutoCompletionHandler = new SimulatorAutoCompletionHandler();
                Console.WriteLine(Constants.Simulator.Running);

                return await Run();
            }

            return true;
        }

        /// <summary>
        /// Feedback for when a request is received
        /// </summary>
        /// <param name="sender">Sender of the request</param>
        /// <param name="e">Eventargs: <see cref="RequestEventArgs"/></param>
        private static void ControlService_RequestReceived(object? sender, RequestEventArgs e)
        {
            Console.WriteLine(string.Format(Constants.Simulator.RequestReceived, e.Floor, e.Direction));
        }

        /// <summary>
        /// Writes application options to output
        /// </summary>
        private static void PrintOptions()
        {
            foreach (ApplicationOptions opt in Enum.GetValues(typeof(ApplicationOptions)))
            {
                Console.WriteLine(string.Format(Constants.Formatting.OptionsFormat, (int)opt, opt, opt.GetEnumDescription()));
            }

            Console.WriteLine(Constants.Messages.Intro);
        }

        /// <summary>
        /// Adds all current floors to the elevator
        /// </summary>
        /// <param name="elevator"></param>
        private void AddAllFloorsToElevator(Elevator elevator)
        {
            for (int i = 0; i < floors.Count; i++)
            {
                elevator.AddFloor(floors[i]);
            }
        }

        /// <summary>
        /// Adds a new elevator
        /// </summary>
        private void AddElevator()
        {
            var newElevator = new Elevator(_maximumCapacity);
            Elevators.Add(newElevator);
            Console.WriteLine(Constants.Simulator.ElevatorAdded);

            AddFloorsToElevator(ref newElevator);
        }

        /// <summary>
        /// Adds a new floor
        /// </summary>
        /// <param name="floorNumber"></param>
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

        /// <summary>
        /// Add multiple floors according to input
        /// </summary>
        private void AddFloors()
        {
            int floorCount, groundFloor;

            // Amount of floors
            floorCount = _inputManager.GreaterThanNumberInput(Constants.Simulator.Levels, 2); // Lowest is two levels
            Console.Write($"{Constants.Messages.ThankYou} ");

            // Which level is the ground floor?
            groundFloor = _inputManager.BetweenNumberInput(Constants.Simulator.GroundLevel, 0, floorCount);

            // Work out floor levels
            FloorHelper.Iterate(groundFloor, floorCount, AddFloor);

            // If elevators have been added, suggest to apply floor changes
            if (Elevators.Count > 0 && _inputManager.YesNoInput(Constants.Simulator.FloorConfigAll))
            {
                for (int i = 0; i < Elevators.Count; i++)
                {
                    AddAllFloorsToElevator(Elevators[i]);
                }
            }
        }

        /// <summary>
        /// Configure new floors for the elevator
        /// </summary>
        /// <param name="elevator">Ref to the Elevator</param>
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
                        while (elevator.GetFloors().Count() < 2)
                        {
                            // Which floors then?
                            Console.WriteLine(string.Format(Constants.Simulator.FloorCount, floors.Count));
                            Console.WriteLine(string.Join(Constants.Messages.Separator, floors.Select(floor => floor.Number)));

                            var floorNumber = _inputManager.BetweenNumberInput(string.Empty, floors.MinBy(x => x.Number)!.Number, floors.MaxBy(x => x.Number)!.Number);
                            elevator.AddFloor(floors[floorNumber]);
                            Console.Write(Constants.Simulator.ElevatorFloorAdded);
                        }
                    }
                    else
                    {
                        AddFloor(floors.Count + 1);
                        elevator.AddFloor(floors.Last());
                    }
                }
            }
            else
            {
                AddFloors();
            }
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
        private async Task<bool> SimulatePerson()
        {
            while (_isRunning)
            {
                int lowest = floors.MinBy(x => x.Number)!.Number, highest = floors.MaxBy(x => x.Number)!.Number;
                // Print out floor config
                Console.WriteLine(string.Format(Constants.Simulator.Selection, floors.Count, lowest, highest));

                int currentFloor = _inputManager.BetweenNumberInput(Constants.Simulator.RequestElevator, lowest, highest);

                int people = _inputManager.BetweenNumberInput(Constants.Simulator.PeopleCount, 1, _maximumCapacity);

                // Just by this information, we should be able to tell which directions are available
                var directions = _controller.GetAvailableDirections(currentFloor);

                // Can go up or down, so choose
                if (directions.HasFlag(ElevatorDirection.Up | ElevatorDirection.Down))
                {
                    directions = _inputManager.DirectionInput(Constants.Input.Direction);
                }

                await _controller.RequestElevatorAsync(new Request(currentFloor, people, directions));
            }

            return true;
        }
    }
}
