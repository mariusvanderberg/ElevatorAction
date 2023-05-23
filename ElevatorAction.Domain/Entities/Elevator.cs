using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Entities
{
    public class Elevator : Entity
    {
        private const int MAXCAPACITY = 10; // default maximum capacity
        private Floor? currentFloor = new();
        private List<Floor> floors = new();

        public Elevator(int weightLimit = MAXCAPACITY)
        {
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
        public List<Floor> Floors { get => floors; set => floors = value; }
        public int MaxPersons { get; protected set; }
        
        public override string ToString()
        {
            return $"Elevator: {Id}\nCapacity: {MaxPersons}\nFloors: {Floors.Count}";
        }
    }
}
