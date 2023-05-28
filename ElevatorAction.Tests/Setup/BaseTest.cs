using ElevatorAction.Application.Interfaces;
using ElevatorAction.ConsoleUI.Interfaces;
using ElevatorAction.Domain.Interfaces;
using Moq;

namespace ElevatorAction.Tests
{
    [TestFixture]
    public class BaseTest : TestSetup
    {
        public StringWriter ConsoleOutputWriter;
        public Mock<IInputManager> InputManagerMock;
        public TextReader OriginalConsoleInput;
        public TextWriter OriginalConsoleOutput;
        public Mock<IOutputManager> OutputManagerMock;
        public Mock<IAsyncDelayer> TaskDelayMock;
        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            InputManagerMock = new Mock<IInputManager>();
            OutputManagerMock = new Mock<IOutputManager>();
            TaskDelayMock = new Mock<IAsyncDelayer>(); // Create a mock of Task.Delay

            // Ensure delays do not happen during tests
            TaskDelayMock.Setup(m => m.Delay(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            OriginalConsoleInput = Console.In;
            OriginalConsoleOutput = Console.Out;
            ConsoleOutputWriter = new StringWriter();

            Console.SetOut(ConsoleOutputWriter);
        }

        [OneTimeTearDown]
        public new void OneTimeTearDown()
        {
            InputManagerMock.Reset();
            OutputManagerMock.Reset();
            TaskDelayMock.Reset();

            Console.SetIn(OriginalConsoleInput);
            Console.SetOut(OriginalConsoleOutput);
            ConsoleOutputWriter.Dispose();
        }
    }
}
