namespace ElevatorAction.Domain.Entities
{
    public class Floor : Entity
    {
        private List<Elevator> elevators = new List<Elevator>();
        public int Number { get; set; } = default;
        public string Name { get; set; } = "G";
        public string FriendlyName { get; set; } = "Ground floor";

        public Floor()
        {
            Id = Guid.NewGuid();
        }

        // Setup
        public void AddElevator(Elevator newElevator)
        {
            if (!elevators.Contains(newElevator))
            {
                elevators.Add(newElevator);
            }
        }

        public void RemoveElevator(Elevator elevatorToRemove)
        {
            if (elevators.Contains(elevatorToRemove))
            {
                elevators.Remove(elevatorToRemove);
            }
        }

        public void CallElevator()
        {

        }
    }
}
