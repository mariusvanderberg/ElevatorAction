using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests.Elevators
{
    public class FloorTests : BaseTest
    {
        private Random _rand;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            _rand = new Random();
        }

        [Test]
        public void Adding_New_Floor_Should_Have_Id()
        {
            // Arrange
            Floor floor;

            // Act
            floor = new();

            // Assert
            Assert.That(floor.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(17)]
        [TestCase(100)]
        [TestCase(101)]
        public void Adding_Floors_Should_Have_Correct_Ground_Level(int floorCount)
        {
            // Arrange
            int groundFloor = _rand.Next(0, floorCount);
            List<Floor> floors = new();

            // Act
            FloorHelper.Iterate(groundFloor, floorCount, i => floors.Add(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Assert
            Assert.That(floors.Count, Is.EqualTo(floorCount));
            Assert.That(floors.MinBy(x => x.Number)?.Number, Is.EqualTo(groundFloor * -1 + 1));
            Assert.That(floors.MaxBy(x => x.Number)?.Number, Is.EqualTo(floorCount - groundFloor));
        }
    }
}
