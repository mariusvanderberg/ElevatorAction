using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorAction.Domain.Entities
{
    /// <summary>
    /// This is to handle a block of the building with multiple
    /// floors and elevators. This clsss may not be needed, but
    /// allows to visually represent the elevators per block
    /// </summary>
    public class BuildingBlock : Entity
    {
        public string Name { get; set; }

        // By knowing floors, we could always work our way back to get elevators
        public List<Floor> floors = new();

        public BuildingBlock()
        {
            Id = Guid.NewGuid();
            Name = "Block A";
        }
    }
}
