using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Core.Entities
{
    public class Elevator
    {
        public int CapacityLimit { get; set; }
        public int TimeBetweenFloors { get; set; }
        public int CurrentFloor { get; set; }
        public int? NextStop { get; set; }
        public ElevatorStatus Status { get; set; }
        public Direction Direction { get; set; }
        public List<Passenger> PassengerList { get; set; }
    }
}
