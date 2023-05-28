using ElevatorAction.Application;
using ElevatorAction.Application.Common;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using ElevatorAction.Tests.Helpers;

namespace ElevatorAction.Tests.Elevators
{
    public class ElevatorServiceTests : BaseTest
    {
        private Elevator _elevator;
        private ElevatorService _elevatorService;

        [Test]
        [TestCase(7, 3, true)]
        [TestCase(0, 3, true)]
        [TestCase(0, 10, true)]
        [TestCase(10, 10, false)]
        [TestCase(7, 7, false)]
        public void Elevator_With_Enough_Capacity_Should_Return_True(int currentPeople, int additionalPeople, bool expectedOutcome)
        {
            // Arrange / Act
            _elevator.CurrentPersons = currentPeople;

            // Assert
            Assert.That(_elevatorService.HasSpaceFor(additionalPeople), Is.EqualTo(expectedOutcome));
        }

        [Test]
        public void Emergency_Stop_Should_Put_Elevator_Out_Of_ServiceAsync()
        {
            // Arrange/ Act
            _elevatorService.MakeEmergencyStop();

            // Assert
            Assert.That(_elevator.ElevatorState, Is.EqualTo(ElevatorState.OutOfOrder));
        }

        [Test]
        [TestCase(2)]
        [TestCase(7)]
        [TestCase(10)]
        public async Task Going_To_Highest_Level_Should_SucceedAsync(int floorCount)
        {
            // Arrange
            int groundFloor = floorCount / 2;
            int expectedFloor = groundFloor;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            // Act
            await _elevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(7)]
        public async Task Going_To_Lowest_Level_Should_SucceedAsync(int floorCount)
        {
            // Arrange
            int groundFloor = floorCount / 2;
            int expectedFloor = groundFloor * -1 + 1;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            // Act
            await _elevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }

        [Test]
        [TestCase(-10)]
        [TestCase(10)]
        public void Moving_Elevator_To_Wrong_Floor_Should_Throw(int expectedFloor)
        {
            // Arrange
            const int floorCount = 5, groundFloor = 2;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            // Act
            var task = _elevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Up, new CancellationToken());

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await task);
        }

        [Test]
        public async Task Requesting_Elevator_Should_Send_Elevator_To_Floor_SuccessfullyAsync()
        {
            // Arrange
            const int floorCount = 5, groundFloor = 2, expectedFloor = 3, people = 10;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            Request req = new Request(expectedFloor, people, Domain.Enums.ElevatorDirection.Down);

            // Act
            await _elevatorService.ProcessRequestAsync(req, new CancellationToken());

            // Assert
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }

        [Test]
        [TestCase(-10)]
        [TestCase(10)]
        public void Requesting_Elevator_To_Wrong_Floor_Should_Throw(int expectedFloor)
        {
            // Arrange
            const int floorCount = 5, groundFloor = 2, people = 10;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            var req = new Request(expectedFloor, people, Domain.Enums.ElevatorDirection.Down);

            // Act
            var task = _elevatorService.ProcessRequestAsync(req, new CancellationToken());

            // Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await task);
        }

        [SetUp]
        public void SetUp()
        {
            _elevator = new Elevator();
            _elevatorService = new ElevatorService(_elevator, TaskDelayMock.Object);
        }

        [Test]
        public async Task Should_Be_Able_To_Access_Bewlow_Ground_LevelsAsync()
        {
            // Arrange
            const int floorCount = 5, groundFloor = 3, expectedFloor = -2;

            TestHelper.AddFloorsToElevator(groundFloor, floorCount, _elevator);

            // Act
            await _elevatorService.MoveToFloorAsync(expectedFloor, Domain.Enums.ElevatorDirection.Down, new CancellationToken());

            // Assert
            Assert.That(_elevator.CurrentFloor, Is.EqualTo(expectedFloor));
        }
    }
}
