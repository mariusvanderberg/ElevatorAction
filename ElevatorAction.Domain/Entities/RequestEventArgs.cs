using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Domain.Entities
{
    /// <summary>
    /// This class is used for transport between messages
    /// </summary>
    public class RequestEventArgs : EventArgs
    {
        public int Floor { get; set; }
        public ElevatorDirection Direction { get; }

        public RequestEventArgs(int floor, ElevatorDirection direction)
        {
            Floor = floor;
            Direction = direction;
        }
    }
}
