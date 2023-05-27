namespace ElevatorAction.Domain.Entities
{
    public class Floor : Entity
    {
        public int Number { get; set; } = default;
        public string Name { get; set; } = null!;
        public string FriendlyName { get; set; } = null!;

        public Floor()
        {
            Id = Guid.NewGuid();
        }
    }
}
