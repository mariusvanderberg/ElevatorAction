using ElevatorAction.Application.Interfaces;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ElevatorAction.Tests
{
    [TestFixture]
    public class BaseTest : TestSetup
    {
        public Mock<IInputManager> InputManagerMock;
        public Mock<IOutputManager> OutputManagerMock;
        public Mock<IAsyncDelayer> TaskDelayMock;
        public Mock<IElevatorControlService> ElevatorControlServiceMock;
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
        public new void OneTimeSetUp()
        {
            InputManagerMock = new Mock<IInputManager>();
            OutputManagerMock = new Mock<IOutputManager>();
            TaskDelayMock = new Mock<IAsyncDelayer>(); // Create a mock of Task.Delay
            ElevatorControlServiceMock = new Mock<IElevatorControlService>();

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
    }
}
