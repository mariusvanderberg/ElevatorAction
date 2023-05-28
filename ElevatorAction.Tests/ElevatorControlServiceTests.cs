using ElevatorAction.Application;
using ElevatorAction.Application.Common;
using ElevatorAction.Application.Interfaces;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Tests.Setup;
using Moq;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests
{
    public class ElevatorControlServiceTests : BaseTest
    {
        private IElevatorControlService controller;

        public List<Elevator> Elevators { get; set; } = new();

        [Test]
        [TestCase(0, ElevatorDirection.Up | ElevatorDirection.Down, true)]
        [TestCase(-2, ElevatorDirection.Down, false)]
        [TestCase(-2, ElevatorDirection.Up, true)]
        [TestCase(7, ElevatorDirection.Up, false)]
        [TestCase(7, ElevatorDirection.Down, true)]
        [TestCase(7, ElevatorDirection.Up | ElevatorDirection.Down, false)]
        public void GetAvailableDirections_Should_Return_Correct_ElevatorDirection(int floor, ElevatorDirection expectedFlag, bool expectedOutcome)
        {
            // Arrange / Act
            var dir = controller.GetAvailableDirections(floor);

            // Assert
            Assert.That(dir.HasFlag(expectedFlag), Is.EqualTo(expectedOutcome));
        }

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            controller = Resolve<IElevatorControlService>();

            List<IElevatorService> services = new();
            List<Floor> floors = new();

            // Add 10 foors, of which 3 is ground: -2 to 7
            FloorHelper.Iterate(3, 10, i => floors.Add(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Add 3 elevators
            for (int i = 0; i < 3; i++)
            {
                Elevators.Add(new Elevator(10));
                for (int j = 0; j < floors.Count; j++)
                {
                    Elevators[i].AddFloor(floors[j]);
                }
                services.Add(new ElevatorService(Elevators[i], TaskDelayMock.Object));
            }

            controller.Initialize(services, floors);
        }

        [Test]
        public async Task Requesting_Elevator_Should_Return_Closet_Elevator()
        {
            // Arrange
            int firstRequestFloor = -2, secondRequestFloor = 7, poeple = 10;
            var direction = ElevatorDirection.Up;

            // Act
            await controller.RequestElevatorAsync(new Request(firstRequestFloor, poeple, direction));
            await controller.RequestElevatorAsync(new Request(secondRequestFloor, poeple, direction));

            // Assert
            Assert.That(Elevators.Count(x => x.CurrentFloor == secondRequestFloor), Is.EqualTo(2));
        }

        [Test]
        public async Task Requesting_More_People_Than_Elevator_Capacity_Should_Queue_More_ElevatorsAsync()
        {
            // Arrange
            int firstRequestFloor = 7, poeple = 100;
            var direction = ElevatorDirection.Up;

            var mock = new Mock<IInputManager>();
            mock.Setup(m => m.NumberInput(It.IsAny<string>())).Returns(7);

            ReplaceServiceWithMock(mock);

            // Act
            await controller.RequestElevatorAsync(new Request(firstRequestFloor, poeple, direction));

            // Assert
            Assert.That(Elevators.All(x => x.CurrentFloor == firstRequestFloor), Is.True);
        }

        [Test]
        public void Requesting_Multiple_Elevators_Should_Return_Closet_Elevator()
        {
            // Arrange
            int firstRequestFloor = -2, secondRequestFloor = 7, poeple = 10;
            var direction = ElevatorDirection.Up;

            // Act
            _ = controller.RequestElevatorAsync(new Request(firstRequestFloor, poeple, direction));
            _ = controller.RequestElevatorAsync(new Request(firstRequestFloor, poeple, direction));

            // No check which elevator is still on ground
            var stationaryElevator = Elevators.FirstOrDefault(x => x.CurrentFloor != firstRequestFloor);

            _ = controller.RequestElevatorAsync(new Request(secondRequestFloor, poeple, direction));

            // Assert
            Assert.That(stationaryElevator?.CurrentFloor, Is.EqualTo(secondRequestFloor));
        }
    }
}
