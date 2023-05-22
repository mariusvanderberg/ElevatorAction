using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Entities
{
    public class Elevator : Entity
    {
        private const int MAXCAPACITY = 10;
        private Floor? currentFloor = new();
        private List<Floor> floors = new();

        public Elevator()
        {
            Id = Guid.NewGuid();
        }

        public int CurrentFloor { get => currentFloor?.Number ?? 0; set => floors.FirstOrDefault(x => x.Number == value); }
        //public int CurrentFloor { get; set; }
        public int CurrentPersons { get; set; }

        public ElevatorState ElevatorState { get; set; } = ElevatorState.Stationary;
        public ElevatorDirection Direction { get; set; }
        public List<Floor> Floors { get => floors; set => floors = value; }
        public int MaxPersons { get => MAXCAPACITY; }
        //public void AddFloor(Floor floor)
        //{
        //    if (!floors.Contains(floor))
        //    {
        //        floors.Add(floor);
        //    }
        //}

        //public void SetCurrentFloor(Floor floor)
        //{
        //    if (floors.Any(x => x.Id == floor.Id))
        //    {
        //        currentFloor = floors.FirstOrDefault(x => x.Id == floor.Id);
        //    } else
        //    {
        //        // Log a warning
        //    }
        //}

        public override string ToString()
        {
            return $"Elevator: {Id}\nCapacity: {MaxPersons}\nFloors: {Floors.Count}";
        }
    }
}
