using ElevatorAction.Application.Interfaces;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ElevatorAction.Tests
{
    [TestFixture]
    public class BaseTest
    {
        public Mock<IInputManager> InputManagerMock;
        public Mock<IOutputManager> OutputManagerMock;
        public Mock<IAsyncDelayer> TaskDelayMock;
        public Mock<IElevatorControlService> ElevatorControlServiceMock;
        public IConfiguration Configuration;
        public TextReader OriginalConsoleInput;
        public TextWriter OriginalConsoleOutput;
        public StringWriter ConsoleOutputWriter;

        [SetUp]
        public void SetUp()
        {
            // Ensure delays do not happen during tests
            TaskDelayMock.Setup(m => m.Delay(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            InputManagerMock = new Mock<IInputManager>();
            OutputManagerMock = new Mock<IOutputManager>();
            TaskDelayMock = new Mock<IAsyncDelayer>(); // Create a mock of Task.Delay
            ElevatorControlServiceMock = new Mock<IElevatorControlService>();
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            OriginalConsoleInput = Console.In;
            OriginalConsoleOutput = Console.Out;
            ConsoleOutputWriter = new StringWriter();

            Console.SetOut(ConsoleOutputWriter);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            InputManagerMock.Reset();
            OutputManagerMock.Reset();
            TaskDelayMock.Reset();
            ElevatorControlServiceMock.Reset();

            Console.SetIn(OriginalConsoleInput);
            Console.SetOut(OriginalConsoleOutput);
            ConsoleOutputWriter.Dispose();
        }

        //[Test]
        //public async Task Simulator_Run_ShouldInitializeControllerService()
        //{
        //    // Arrange
        //    var elevator1 = new Elevator(10);
        //    var elevator2 = new Elevator(50);
        //    var elevators = new List<Elevator> { elevator1, elevator2 };
        //    var floors = new List<Floor>();
        //    ElevatorControlServiceMock.Setup(s => s.Initialize(It.IsAny<List<IElevatorService>>(), It.IsAny<List<Floor>>()));
        //    var simulator = new ElevatorAction.ConsoleUI.Simulator(InputManagerMock.Object, OutputManagerMock.Object, ElevatorControlServiceMock.Object, Configuration);

        //    // Act
        //    await simulator.Run();

        //    // Assert
        //    ElevatorControlServiceMock.Verify(s => s.Initialize(It.Is<List<IElevatorService>>(services => services.Any(s => s.GetCapacity() == elevator1.MaxPersons) && services.Any(s => s.GetCapacity() == elevator2.MaxPersons)), floors), Times.Once);
        //}

        //[Test]
        //public async Task Simulator_Start_ShouldAddNewElevator()
        //{
        //    // Arrange
        //    var simulator = new ElevatorAction.ConsoleUI.Simulator(InputManagerMock.Object, OutputManagerMock.Object, ElevatorControlServiceMock.Object, Configuration);
        //    InputManagerMock.SetupSequence(m => m.YesNoInput(It.IsAny<string>(), true))
        //        .Returns(true) // Add elevator
        //        .Returns(false); // Exit
        //    InputManagerMock.Setup(m => m.NumberInput(Constants.Simulator.PeopleCount)).Returns(5);

        //    // Act
        //    await simulator.Start();

        //    // Assert
        //    Assert.That(simulator.Elevators.Count, Is.EqualTo(1));
        //    //Assert.AreEqual(10, simulator.Elevators[0].MaximumCapacity);
        //    //Assert.That(simulator.Elevators[0].Floors, Has.Count.EqualTo(5));
        //}

        //[Test]
        //public async Task Simulator_Start_ShouldNotAddNewElevator()
        //{
        //    // Arrange
        //    var simulator = new ElevatorAction.ConsoleUI.Simulator(InputManagerMock.Object, OutputManagerMock.Object, ElevatorControlServiceMock.Object, Configuration);
        //    InputManagerMock.SetupSequence(m => m.YesNoInput(It.IsAny<string>(), true))
        //        .Returns(false); // Exit

        //    // Act
        //    await simulator.Start();

        //    // Assert
        //    Assert.That(simulator.Elevators.Count, Is.EqualTo(0));
        //}

        //// Add more tests for other functionality

        //[Test]
        //public async Task Simulator_Start_ShouldDisplayWelcomeMessage()
        //{
        //    // Arrange
        //    var simulator = new ElevatorAction.ConsoleUI.Simulator(InputManagerMock.Object, OutputManagerMock.Object, ElevatorControlServiceMock.Object, Configuration);
        //    InputManagerMock.Setup(m => m.NumberInput(Constants.Simulator.PeopleCount)).Returns(5);

        //    // Act
        //    await simulator.Start();

        //    // Assert
        //    Assert.IsTrue(ConsoleOutputWriter.ToString().Contains(Constants.Messages.Welcome));
        //}
    }
}
