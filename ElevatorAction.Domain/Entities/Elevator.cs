using ElevatorAction.Domain.Common;
using ElevatorAction.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElevatorAction.Domain.Entities
{
    public class Elevator : Entity
    {
        private const int MAXCAPACITY = 10; // default maximum capacity
        private Floor? currentFloor = new();
        private List<Floor> floors = new();

        public Elevator(int weightLimit = MAXCAPACITY)
        {
            if (weightLimit <= 0 )
            {
                throw new ValidationException(Constants.MaxPersonsError); // Could be handled through FluentValidation
            }
            Id = Guid.NewGuid();
            MaxPersons = weightLimit;
        }

        public int CurrentFloor {
            get
            {
                return currentFloor?.Number ?? 0;
            } 
            set
            {
                currentFloor = floors.FirstOrDefault(x => x.Number == value);
            } 
        
        }
        public int CurrentPersons { get; set; }

        public ElevatorDirection Direction { get; set; }
        public ElevatorState ElevatorState { get; set; } = ElevatorState.Stationary;
        public int MaxPersons { get; protected set; }
        public void AddFloor(Floor floor)
        {
            if (!floors.Any(x => x.Id == floor.Id))
                floors.Add(floor);
        }
    }
}
