using ElevatorAction.ConsoleUI.Helpers;
using ElevatorAction.Domain.Entities;
using ElevatorAction.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using static ElevatorAction.Application.Constants;

namespace ElevatorAction.Tests.Elevators
{
    public class ElevatorTests : BaseTest
    {
        private Elevator _elevator;
        private Random _rand;

        [Test]
        public void Adding_Elevators_Should_Have_Correct_Capacity()
        {
            // Arrange
            int defaultCapacity = 20;

            // Act
            var elevator2 = new Elevator(defaultCapacity);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_elevator.MaxPersons, Is.EqualTo(Configuration.GetValue<int>("Elevator:MaximumCapacity")));
                Assert.That(elevator2.MaxPersons, Is.EqualTo(defaultCapacity));
            });
        }

        [Test]
        [TestCase(0)]
        [TestCase(-10)]
        public void Adding_Elevators_With_Invalid_Capacity_Should_Throw(int capacity) =>
            // Arrange / Act / Assert
            Assert.Throws<ValidationException>(() => _elevator = new Elevator(capacity));

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(111)]
        public void Adding_Floors_To_Elevator_Should_Succeed(int floorCount)
        {
            // Arrange
            int groundFloor = _rand.Next(0, floorCount);

            FloorHelper.Iterate(groundFloor, floorCount, i => _elevator.AddFloor(new Floor
            {
                FriendlyName = i == 0 ? Simulator.GroundLevelName : i.ToString(),
                Name = i.ToString(),
                Number = i
            }));

            // Act
            var currentFloor = _elevator.CurrentFloor;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(currentFloor, Is.EqualTo(0));
                Assert.That(_elevator.CurrentPersons, Is.EqualTo(0));
                Assert.That(_elevator.ElevatorState, Is.EqualTo(ElevatorState.Stationary));
            });
        }

        [Test]
        public void Elevator_Should_Start_At_Ground_Floor()
        {
            // Arrange
            var elevator = new Elevator();

            // Act
            var currentFloor = elevator.CurrentFloor;

            // Assert
            Assert.That(currentFloor, Is.EqualTo(0));
        }

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            _rand = new Random();
        }

        [SetUp]
        public void SetUp()
        {
            _elevator = new Elevator();
        }
    }
}
