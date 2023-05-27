using ElevatorAction.Application;
using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests.Elevators
{
    public class ElevatorServiceTests : BaseTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(7)]
        public async Task Going_To_Highest_Level_Should_SucceedAsync(int floorCount)
        {
            // Arrange
            var elevator = new Elevator();
            var ElevatorService = new ElevatorService(elevator, TaskDelayMock.Object);

            int groundFloor = floorCount / 2;
            int expectedFloor = groundFloor;

            FloorHelper.Iterate(groundFloor, floorCount, i => elevator.AddFloor(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Act
            await ElevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(7)]
        public async Task Going_To_Lowest_Level_Should_SucceedAsync(int floorCount)
        {
            // Arrange
            var elevator = new Elevator();
            var ElevatorService = new ElevatorService(elevator, TaskDelayMock.Object);

            int groundFloor = floorCount / 2;
            int expectedFloor = groundFloor * -1 + 1;

            FloorHelper.Iterate(groundFloor, floorCount, i => elevator.AddFloor(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Act
            await ElevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }

        [Test]
        public async Task Should_Be_Able_To_Access_Bewlow_Ground_LevelsAsync()
        {
            // Arrange
            var elevator = new Elevator();
            var ElevatorService = new ElevatorService(elevator, TaskDelayMock.Object);

            const int totalFloors = 5, groundFloor = 3, expectedFloor = -2;

            FloorHelper.Iterate(groundFloor, totalFloors, i => elevator.AddFloor(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Act
            await ElevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }
    }
}
