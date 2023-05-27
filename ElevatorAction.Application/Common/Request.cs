using ElevatorAction.Domain.Enums;

namespace ElevatorAction.Application.Common
{
    public class Request
    {
        /// <summary>
        /// Someone pressed a button, so now we need to process the request.
        /// This reresents that request
        /// </summary>
        /// <param name="people">Amount of people ready to climb in</param>
        /// <param name="floor">Floor which elevator is requested from</param>
        /// <param name="direction"><see cref="ElevatorDirection"/> direction required</param>
        public Request(int floor, int people, ElevatorDirection direction)
        {
            Floor = floor;
            Direction = direction;
            People = people;
        }

        public ElevatorDirection Direction { get; }
        public int Floor { get; }
        public int People { get; set; }
    }
}
